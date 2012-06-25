using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Areas.AnalysisManagement.Models.PatientsReport;
using Core.Persistence;
using Domain;
using Core.Domain;

namespace Web.Areas.AnalysisManagement.Controllers
{
    public class PatientsReportController : Controller
    {
        public IQueryService<MessageFromDrugShop> QueryMessageFromDrugShop { get; set; }
        public IQueryService<MessageFromDispensary> QueryMessageFromDispensary { get; set; }
        public IQueryService<Outpost> QueryOutpost { get; set; }
        public IQueryService<ServiceNeeded> QueryServiceNeeded { get; set; }
        public IQueryService<Diagnosis> QueryDiagnosis { get; set; }
        public IQueryService<Treatment> QueryTreatment { get; set; }
        public IQueryService<Advice> QueryAdvice { get; set; }

        public IQueryService<Client> QueryClients { get; set; }
        public IQueryService<User> QueryUsers { get; set; }

        private Client _client;
        private User _user;

        public ActionResult Overview()
        {
            return View();
        }

        public JsonResult GetPatientsReport(PatientReportIndexModel model)
        {
            var queryMessage = QueryMessageFromDrugShop.Query();

            if (!string.IsNullOrEmpty(model.gender))
                queryMessage = queryMessage.Where(it => it.Gender.ToUpper() == model.gender.ToUpper());
            if (!string.IsNullOrEmpty(model.serviceNeededId))
                queryMessage = queryMessage.Where(it => it.ServicesNeeded.FirstOrDefault(c => c.Id == new Guid(model.serviceNeededId)) != null);
            DateTime startDate;
            if (!string.IsNullOrEmpty(model.startDate))
                if (DateTime.TryParse(model.startDate, out startDate))
                    queryMessage = queryMessage.Where(it => it.SentDate >= startDate);
            DateTime endDate;
            if (!string.IsNullOrEmpty(model.endDate))
                if (DateTime.TryParse(model.endDate, out endDate))
                    queryMessage = queryMessage.Where(it => it.SentDate <= endDate);

            List<PatientReportoutputModel> reportList = new List<PatientReportoutputModel>();

            foreach (var message in queryMessage.ToList())
            {
                PatientReportModel patientmodel = CreatePatientReportModel(message);
                if (ContainsFilterProperties(patientmodel, model))
                    reportList.Add(patientmodel.ReportModel);
            }

            var a = reportList.ToArray();

            return Json(new PatientReportIndexOutputModel
            {
                Patients = reportList.ToArray(),
                TotalItems = reportList.Count()
            }, JsonRequestBehavior.AllowGet);
        }

        private bool ContainsFilterProperties(PatientReportModel patientmodel, PatientReportIndexModel model)
        {
            if (!string.IsNullOrEmpty(model.countryId))
            {
                if (patientmodel.Outpost == null)
                    return false;
                if (patientmodel.Outpost.Country.Id != new Guid(model.countryId))
                    return false;
            }
            if (!string.IsNullOrEmpty(model.regionId))
            {
                if (patientmodel.Outpost == null)
                    return false;
                if (patientmodel.Outpost.Region.Id != new Guid(model.regionId))
                    return false;
            }
            if (!string.IsNullOrEmpty(model.districtId))
            {
                if (patientmodel.Outpost == null)
                    return false;
                if (patientmodel.Outpost.District.Id != new Guid(model.districtId))
                    return false;
            }
            if (!string.IsNullOrEmpty(model.diagnosisId))
            {
                if (patientmodel.DiagnosisList == null)
                    return false;
                if (patientmodel.DiagnosisList.FirstOrDefault(it => it.Id == new Guid(model.diagnosisId)) == null)
                    return false;
            }
            if (!string.IsNullOrEmpty(model.treatmentId))
            {
                if (patientmodel.TreatmentList == null)
                    return false;
                if (patientmodel.TreatmentList.FirstOrDefault(it => it.Id == new Guid(model.treatmentId)) == null)
                    return false;
            }
            if (!string.IsNullOrEmpty(model.adviceId))
            {
                if (patientmodel.AdviceList == null)
                    return false;
                if (patientmodel.AdviceList.FirstOrDefault(it => it.Id == new Guid(model.adviceId)) == null)
                    return false;
            }
            return true;
        }

        private PatientReportModel CreatePatientReportModel(MessageFromDrugShop message)
        {
            PatientReportModel patientmodel = new PatientReportModel();
            patientmodel.ReportModel.Initials = message.Initials;
            patientmodel.ReportModel.PatientID = message.IDCode;
            patientmodel.ReportModel.Gender = message.Gender;
            patientmodel.ReportModel.Age = DateTime.UtcNow.Date < message.BirthDate.AddYears((DateTime.UtcNow.Year - message.BirthDate.Year)) ? (DateTime.UtcNow.Year - message.BirthDate.Year - 1).ToString() : (DateTime.UtcNow.Year - message.BirthDate.Year).ToString();
            patientmodel.Outpost = QueryOutpost.Load(message.OutpostId);
            if (patientmodel.Outpost != null)
                patientmodel.ReportModel.Drugshop = patientmodel.Outpost.Name;
            patientmodel.ReportModel.DrugshopDate = message.SentDate.ToString("dd MM yyyy");
            patientmodel.ConditionList = message.ServicesNeeded;
            patientmodel.ReportModel.Condition = message.ServicesNeeded.ToString();

            var dispensary = QueryMessageFromDispensary.Query().Where(it => it.MessageFromDrugShop.Id == message.Id);
            if (dispensary.Any())
            {
                MessageFromDispensary messageDispensary = dispensary.FirstOrDefault();
                var outpostDispensary = QueryOutpost.Load(messageDispensary.OutpostId);
                if (outpostDispensary != null)
                    patientmodel.ReportModel.Dispensary = outpostDispensary.Name;
                patientmodel.ReportModel.DispensaryDate = messageDispensary.SentDate.ToString("dd MM yyyy");
                patientmodel.ReportModel.Diagnosis = messageDispensary.Diagnosises.ToString();
                patientmodel.DiagnosisList = messageDispensary.Diagnosises;
                patientmodel.ReportModel.Treatment = messageDispensary.Treatments.ToString();
                patientmodel.TreatmentList = messageDispensary.Treatments;
                patientmodel.ReportModel.Advice = messageDispensary.Advices.ToString();
                patientmodel.AdviceList = messageDispensary.Advices;
            }

            return patientmodel;
        }

        [HttpGet]
        public JsonResult GetServiceNeeded()
        {
            LoadUserAndClient();
            var serviceNeeded = QueryServiceNeeded.Query().Where(it => it.Client.Id == this._client.Id);
            int totalItems = serviceNeeded.Count();

            var listProjection = (from service in serviceNeeded.ToList()
                                  select new ServiceModel
                                  {
                                      Id = service.Id,
                                      Code = service.Code
                                  }).ToArray();


            return Json(new ServiceIndexOutputModel
            {
                Service = listProjection,
                TotalItems = totalItems
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetDiagnosis()
        {
            LoadUserAndClient();
            var diagnosis = QueryDiagnosis.Query().Where(it => it.Client.Id == this._client.Id);
            int totalItems = diagnosis.Count();

            var listProjection = (from service in diagnosis.ToList()
                                  select new ServiceModel
                                  {
                                      Id = service.Id,
                                      Code = service.Code
                                  }).ToArray();


            return Json(new ServiceIndexOutputModel
            {
                Service = listProjection,
                TotalItems = totalItems
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetTreatment()
        {
            LoadUserAndClient();
            var treatment = QueryTreatment.Query().Where(it => it.Client.Id == this._client.Id);
            int totalItems = treatment.Count();

            var listProjection = (from service in treatment.ToList()
                                  select new ServiceModel
                                  {
                                      Id = service.Id,
                                      Code = service.Code
                                  }).ToArray();


            return Json(new ServiceIndexOutputModel
            {
                Service = listProjection,
                TotalItems = totalItems
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAdvice()
        {
            LoadUserAndClient();
            var advice = QueryAdvice.Query().Where(it => it.Client.Id == this._client.Id);
            int totalItems = advice.Count();

            var listProjection = (from service in advice.ToList()
                                  select new ServiceModel
                                  {
                                      Id = service.Id,
                                      Code = service.Code
                                  }).ToArray();


            return Json(new ServiceIndexOutputModel
            {
                Service = listProjection,
                TotalItems = totalItems
            }, JsonRequestBehavior.AllowGet);
        }

        private void LoadUserAndClient()
        {
            var loggedUser = User.Identity.Name;
            this._user = QueryUsers.Query().FirstOrDefault(m => m.UserName == loggedUser);

            if (_user == null)
                throw new NullReferenceException("User is not logged in");

            var clientId = Client.DEFAULT_ID;
            if (_user.ClientId != Guid.Empty)
                clientId = _user.ClientId;

            this._client = QueryClients.Load(clientId);
        }
    }
}
