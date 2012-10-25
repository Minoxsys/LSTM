using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Areas.AnalysisManagement.Models.PatientsReport;
using Core.Persistence;
using Domain;
using Core.Domain;
using Web.Security;
using System.IO;
using Core.Security;

namespace Web.Areas.AnalysisManagement.Controllers
{
    public class PatientsReportController : Controller
    {
        public IQueryService<MessageFromDrugShop> QueryMessageFromDrugShop { get; set; }
        public IQueryService<MessageFromDispensary> QueryMessageFromDispensary { get; set; }
        public IQueryService<Outpost> QueryOutpost { get; set; }
        public IQueryService<Condition> QueryCondition { get; set; }
        public IQueryService<Diagnosis> QueryDiagnosis { get; set; }
        public IQueryService<Treatment> QueryTreatment { get; set; }
        public IQueryService<Advice> QueryAdvice { get; set; }
        public IQueryService<Country> QueryCountry { get; set; }
        public IQueryService<Region> QueryRegion { get; set; }
        public IQueryService<District> QueryDistrict { get; set; }

        public IQueryService<Client> QueryClients { get; set; }
        public IQueryService<User> QueryUsers { get; set; }

        private Client _client;
        private User _user;

        public IPermissionsService PermissionService { get; set; }
        private const String REPORT_EXPORT_PERMISSION = "ExportToExcel.View";

        [HttpGet]
        [Requires(Permissions = "Report.View")]
        public ActionResult Overview()
        {
            ViewBag.HasNoRightsToExport = (PermissionService.HasPermissionAssigned(REPORT_EXPORT_PERMISSION, User.Identity.Name) == true) ? false.ToString().ToLowerInvariant() : true.ToString().ToLowerInvariant();
            return View();
        }

        public JsonResult GetPatientsReport(PatientReportIndexModel model)
        {
            var listDataReport = GetDataForReport(model);
            
            return Json(new PatientReportIndexOutputModel
            {
                Patients = listDataReport.ToArray(),
                TotalItems = listDataReport.Count()
            }, JsonRequestBehavior.AllowGet);
        }

        private List<PatientReportoutputModel> GetDataForReport(PatientReportIndexModel model)
        {
            var queryMessage = QueryMessageFromDrugShop.Query();

            if (!string.IsNullOrEmpty(model.gender))
                queryMessage = queryMessage.Where(it => it.Gender.ToUpper() == model.gender.ToUpper());
            if (!string.IsNullOrEmpty(model.conditionId))
                queryMessage = queryMessage.Where(it => it.ServicesNeeded.FirstOrDefault(c => c.Id == new Guid(model.conditionId)) != null);
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

            return reportList;
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
        public JsonResult GetCondition()
        {
            LoadUserAndClient();
            var condition = QueryCondition.Query().Where(it => it.Client.Id == this._client.Id);
            int totalItems = condition.Count();

            var listProjection = (from service in condition.ToList()
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

        [HttpPost]
        public void ExportToExcel(PatientReportIndexModel model)
        {
            Response.Clear();
            Response.ContentType = "application/vnd.xls";
            Response.AddHeader("Content-disposition", "attachment; filename=" + "PatientsReport" + DateTime.UtcNow.ToShortDateString() + ".xls");

            var reportData = GetDataForReport(model);
            PatientReportIndexModel outputDataModel = GetFiltersForExcel(model);
            
            StreamWriter writer = new StreamWriter(Response.OutputStream);

            writer.WriteLine("Country:\t" + outputDataModel.countryId + "\t \t");
            writer.WriteLine("Region:\t" + outputDataModel.regionId + "\t \t");
            writer.WriteLine("District:\t" + outputDataModel.districtId + "\t \t");
            writer.WriteLine("Start date:\t" + outputDataModel.startDate + "\t \t");
            writer.WriteLine("End date:\t" + outputDataModel.endDate + "\t \t");
            writer.WriteLine("Condition:\t" + outputDataModel.conditionId + "\t \t");
            writer.WriteLine("Diagnosis:\t" + outputDataModel.diagnosisId + "\t \t");
            writer.WriteLine("Treatment:\t" + outputDataModel.treatmentId + "\t \t");
            writer.WriteLine("Advice:\t" + outputDataModel.adviceId + "\t \t");
            writer.WriteLine("Gender:\t" + outputDataModel.gender + "\t \t");
            writer.WriteLine(" ");
            writer.WriteLine(" ");

            writer.WriteLine("Patient initials\t" + "Patient ID\t" + "Gender\t" + "Age\t" + "Drugshop\t" + "Drugshop Date\t" + "Condition\t" + "Dispensary\t" + "Dispensary Date\t" + "Diagnosis\t" + "Treatment\t" + "Advice\t \t");

            foreach (var item in reportData)
            {
                writer.WriteLine(item.Initials + "\t" + item.PatientID + "\t" + item.Gender + "\t" + item.Age + "\t" + item.Drugshop + "\t" + item.DrugshopDate + "\t" + item.Condition + "\t" + item.Dispensary + "\t" + item.DispensaryDate + "\t" + item.Diagnosis + "\t" + item.Treatment + "\t" + item.Advice + "\t \t");
            }
            writer.Close();

            Response.End();

        }

        private PatientReportIndexModel GetFiltersForExcel(PatientReportIndexModel model)
        {
            PatientReportIndexModel outputModel = new PatientReportIndexModel();

            if (!string.IsNullOrEmpty(model.countryId))
                outputModel.countryId = QueryCountry.Load(new Guid(model.countryId)).Name;
            else
                outputModel.countryId = " ";

            if (!string.IsNullOrEmpty(model.regionId))
                outputModel.regionId = QueryRegion.Load(new Guid(model.regionId)).Name;
            else
                outputModel.regionId = " ";

            if (!string.IsNullOrEmpty(model.districtId))
                outputModel.districtId = QueryDistrict.Load(new Guid(model.districtId)).Name;
            else
                outputModel.districtId = " ";

            if (!string.IsNullOrEmpty(model.conditionId))
                outputModel.conditionId = QueryCondition.Load(new Guid(model.conditionId)).Code;
            else
                outputModel.conditionId = " ";

            if (!string.IsNullOrEmpty(model.diagnosisId))
                outputModel.diagnosisId = QueryDiagnosis.Load(new Guid(model.diagnosisId)).Code;
            else
                outputModel.diagnosisId = " ";

            if (!string.IsNullOrEmpty(model.treatmentId))
                outputModel.treatmentId = QueryTreatment.Load(new Guid(model.treatmentId)).Code;
            else
                outputModel.treatmentId = " ";

            if (!string.IsNullOrEmpty(model.adviceId))
                outputModel.adviceId = QueryAdvice.Load(new Guid(model.adviceId)).Code;
            else
                outputModel.adviceId = " ";

            outputModel.startDate = model.startDate;
            outputModel.endDate = model.endDate;
            outputModel.gender = model.gender;

            return outputModel;
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
