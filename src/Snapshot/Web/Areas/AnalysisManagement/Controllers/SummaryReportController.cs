using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Security;
using Core.Persistence;
using Domain;
using Core.Domain;
using Web.Areas.AnalysisManagement.Models.SummaryReport;
using System.IO;

namespace Web.Areas.AnalysisManagement.Controllers
{
    public class SummaryReportController : Controller
    {
        public IQueryService<MessageFromDispensary> QueryMessageFromDispensary { get; set; }
        public IQueryService<MessageFromDrugShop> QueryMessageFromDrugShop { get; set; }
        public IQueryService<Outpost> QueryOutpost { get; set; }
        public IQueryService<Client> QueryClients { get; set; }
        public IQueryService<User> QueryUsers { get; set; }
        public IQueryService<Country> QueryCountry { get; set; }
        public IQueryService<Region> QueryRegion { get; set; }
        public IQueryService<District> QueryDistrict { get; set; }

        private Client _client;
        private User _user;
        private string NOTREATMENT = "NOT/AV";

        [HttpGet]
        [Requires(Permissions = "Report.View")]
        public ActionResult Overview()
        {
            return View();
        }

        public JsonResult GetSummaryReport(SummaryIndexModel inputModel)
        {
            List<SummaryReportModel> dataList = GetDataForReport(inputModel);

            return Json(new SummaryReportIndexOutputModel
            {
                Patients = dataList.ToArray(),
                TotalItems = dataList.Count()
            }, JsonRequestBehavior.AllowGet);

        }

        private List<SummaryReportModel> GetDataForReport(SummaryIndexModel inputModel)
        {
            LoadUserAndClient();

            var queryOutposts = QueryOutpost.Query().Where(it => it.Client == _client && it.OutpostType.Type > 0);
            if (!string.IsNullOrEmpty(inputModel.countryId))
                queryOutposts = queryOutposts.Where(it => it.Country.Id == new Guid(inputModel.countryId));
            if (!string.IsNullOrEmpty(inputModel.regionId))
                queryOutposts = queryOutposts.Where(it => it.Region.Id == new Guid(inputModel.regionId));
            if (!string.IsNullOrEmpty(inputModel.districtId))
                queryOutposts = queryOutposts.Where(it => it.District.Id == new Guid(inputModel.districtId));

            List<SummaryReportModel> summaryList = new List<SummaryReportModel>();


            foreach (var dispensary in queryOutposts.ToList())
            {
                SummaryReportModel model = new SummaryReportModel();
                model.Outpost = dispensary.Name;
                model.FailedToReport = GetNumberOfPatientsWhoFailedToReportToDispensary(dispensary, inputModel);
                model.NotTreated = GetNumberOfPatientsNotTreatedAt(dispensary, inputModel);
                model.Treated = GetNumberOfPatientsTreatedAt(dispensary, inputModel);
                model.DistinctPatients = GetIndividualPatientsAt(dispensary, inputModel);

                summaryList.Add(model);
            }

            return summaryList;
        }

        private int GetNumberOfPatientsWhoFailedToReportToDispensary(Outpost dispensary, SummaryIndexModel inputModel)
        {
            var drugshopsFrorDispensary = QueryOutpost.Query().Where(it => it.Warehouse.Id == dispensary.Id).ToList();
            var drugshopMessages = QueryMessageFromDrugShop.Query();

            DateTime startDate;
            if (!string.IsNullOrEmpty(inputModel.startDate))
                if (DateTime.TryParse(inputModel.startDate, out startDate))
                    drugshopMessages = drugshopMessages.Where(it => it.SentDate  >= startDate);
            DateTime endDate;
            if (!string.IsNullOrEmpty(inputModel.endDate))
                if (DateTime.TryParse(inputModel.endDate, out endDate))
                    drugshopMessages = drugshopMessages.Where(it => it.SentDate <= endDate);

            int sum = 0;
            foreach(var drugshop in drugshopsFrorDispensary)
            {
                var messageList = drugshopMessages.Where(it => it.OutpostId == drugshop.Id);
                foreach (var message in messageList)
                {
                    if (QueryMessageFromDispensary.Query().Where(it => it.MessageFromDrugShop.Id == message.Id).Any() == false)
                        sum++;
                }
            }
            return sum;
        }

        private int GetNumberOfPatientsNotTreatedAt(Outpost dispensary, SummaryIndexModel inputModel)
        {
            var queryMessage = QueryMessageFromDispensary.Query();

            DateTime startDate;
            if (!string.IsNullOrEmpty(inputModel.startDate))
                if (DateTime.TryParse(inputModel.startDate, out startDate))
                    queryMessage = queryMessage.Where(it => it.SentDate >= startDate);
            DateTime endDate;
            if (!string.IsNullOrEmpty(inputModel.endDate))
                if (DateTime.TryParse(inputModel.endDate, out endDate))
                    queryMessage = queryMessage.Where(it => it.SentDate <= endDate);

            return queryMessage.Where(it => it.OutpostId == dispensary.Id && it.Treatments.FirstOrDefault(c => c.Code == NOTREATMENT) != null).Count();
        }

        private int GetNumberOfPatientsTreatedAt(Outpost dispensary, SummaryIndexModel inputModel)
        {
            var queryMessage = QueryMessageFromDispensary.Query();

            DateTime startDate;
            if (!string.IsNullOrEmpty(inputModel.startDate))
                if (DateTime.TryParse(inputModel.startDate, out startDate))
                    queryMessage = queryMessage.Where(it => it.SentDate >= startDate);
            DateTime endDate;
            if (!string.IsNullOrEmpty(inputModel.endDate))
                if (DateTime.TryParse(inputModel.endDate, out endDate))
                    queryMessage = queryMessage.Where(it => it.SentDate <= endDate);

            return queryMessage.Where(it => it.OutpostId == dispensary.Id && it.Treatments.FirstOrDefault(c => c.Code == NOTREATMENT) == null).Count();
        }

        private int GetIndividualPatientsAt(Outpost dispensary, SummaryIndexModel inputModel)
        {
            var drugshopsFrorDispensary = QueryOutpost.Query().Where(it => it.Warehouse.Id == dispensary.Id).ToList();
            int sum = 0;

            foreach (var drugshop in drugshopsFrorDispensary)
            {
                sum += GetDistinctPatientsFor(drugshop, inputModel);
                
            }
            return sum;
        }

        private int GetDistinctPatientsFor(Outpost outpost, SummaryIndexModel inputModel)
        {
            var queryMessage = QueryMessageFromDrugShop.Query().Where(it => it.OutpostId == outpost.Id);

            DateTime startDate;
            if (!string.IsNullOrEmpty(inputModel.startDate))
                if (DateTime.TryParse(inputModel.startDate, out startDate))
                    queryMessage = queryMessage.Where(it => it.SentDate >= startDate);
            DateTime endDate;
            if (!string.IsNullOrEmpty(inputModel.endDate))
                if (DateTime.TryParse(inputModel.endDate, out endDate))
                    queryMessage = queryMessage.Where(it => it.SentDate <= endDate);

            List<PatientModel> patientsList = new List<PatientModel>();
            foreach (var patient in queryMessage.ToList())
            {
                PatientModel model = new PatientModel { Initials = patient.Initials.ToUpper(), Gender = patient.Gender.ToUpper(), BirthDate = patient.BirthDate.ToShortDateString() };
                if (patientsList.FirstOrDefault(it => it.Initials == model.Initials && it.Gender == model.Gender && it.BirthDate == model.BirthDate) == null)
                    patientsList.Add(model);
            }

            return patientsList.Count();
        }

        [HttpPost]
        public void ExportToExcel(SummaryIndexModel model)
        {
            Response.Clear();
            Response.ContentType = "application/vnd.xls";
            Response.AddHeader("Content-disposition", "attachment; filename=" + "SummaryReport" + DateTime.UtcNow.ToShortDateString() + ".xls");

            var reportData = GetDataForReport(model);
            SummaryIndexModel outputDataModel = GetFiltersForExcel(model);

            StreamWriter writer = new StreamWriter(Response.OutputStream);

            writer.WriteLine("Country:\t" + outputDataModel.countryId + "\t \t");
            writer.WriteLine("Region:\t" + outputDataModel.regionId + "\t \t");
            writer.WriteLine("District:\t" + outputDataModel.districtId + "\t \t");
            writer.WriteLine("Start date:\t" + outputDataModel.startDate + "\t \t");
            writer.WriteLine("End date:\t" + outputDataModel.endDate + "\t \t");
            writer.WriteLine(" ");
            writer.WriteLine(" ");

            writer.WriteLine("Health facility\t" + "Treated\t" + "Not treated\t" + "Failed to report\t" + "Distinct patients\t \t");

            foreach (var item in reportData)
                    writer.WriteLine(item.Outpost + "\t" + item.Treated + "\t" + item.NotTreated + "\t" + item.FailedToReport + "\t" + item.DistinctPatients + "\t \t");

            writer.Close();
            Response.End();

        }

        private SummaryIndexModel GetFiltersForExcel(SummaryIndexModel model)
        {
            SummaryIndexModel outputModel = new SummaryIndexModel();

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

            outputModel.startDate = model.startDate;
            outputModel.endDate = model.endDate;

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
