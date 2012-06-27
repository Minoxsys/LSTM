using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Areas.AnalysisManagement.Models.DiagnosisReport;
using Core.Persistence;
using Domain;
using Core.Domain;
using Web.Security;
using System.IO;

namespace Web.Areas.AnalysisManagement.Controllers
{
    public class DiagnosisReportController : Controller
    {
        public IQueryService<MessageFromDispensary> QueryMessageFromDispensary { get; set; }
        public IQueryService<Outpost> QueryOutpost { get; set; }
        public IQueryService<Diagnosis> QueryDiagnosis { get; set; }
        public IQueryService<Client> QueryClients { get; set; }
        public IQueryService<User> QueryUsers { get; set; }
        public IQueryService<Country> QueryCountry { get; set; }
        public IQueryService<Region> QueryRegion { get; set; }
        public IQueryService<District> QueryDistrict { get; set; }

        private Client _client;
        private User _user;

        [HttpGet]
        [Requires(Permissions = "Report.View")]
        public ActionResult Overview()
        {
            return View();
        }

        public JsonResult GetDiagnosisReport(DiagnosisIndexModel inputModel)
        {

            List<DiagnosisReportModel> listOfDataForReport = GetDataForReport(inputModel);

            return Json(new DiagnosisReportIndexOutputModel
            {
                Diagnosis = listOfDataForReport.ToArray(),
                TotalItems = listOfDataForReport.Count()
            }, JsonRequestBehavior.AllowGet);
        }

        private List<DiagnosisReportModel> GetDataForReport(DiagnosisIndexModel inputModel)
        {
            LoadUserAndClient();

            var queryDiagnosis = QueryDiagnosis.Query().Where(it => it.Client == _client);
            List<Outpost> outpostList = GetListOfOutpostsAfterFiltering(inputModel);
            List<DiagnosisReportModel> diagnosisList = new List<DiagnosisReportModel>();

            foreach (var diagnosis in queryDiagnosis.ToList())
            {
                foreach (var dispensary in outpostList)
                {
                    DiagnosisReportModel model = new DiagnosisReportModel();
                    model.Diagnosis = diagnosis.Code;
                    model.Outpost = dispensary.Name + " (" + dispensary.District.Name + ")";

                    PatientsModel patients = GetNumberOfPatients(dispensary, inputModel.startDate, inputModel.endDate, diagnosis);
                    model.Female = patients.Female;
                    model.Male = patients.Male;
                    model.NumberOfPatients = patients.TotalNumber;

                    diagnosisList.Add(model);
                }
            }

            return diagnosisList;
        }

        private List<Outpost> GetListOfOutpostsAfterFiltering(DiagnosisIndexModel inputModel)
        {
            var queryOutposts = QueryOutpost.Query().Where(it => it.Client == _client && it.OutpostType.Type > 0);
            if (!string.IsNullOrEmpty(inputModel.countryId))
                queryOutposts = queryOutposts.Where(it => it.Country.Id == new Guid(inputModel.countryId));
            if (!string.IsNullOrEmpty(inputModel.regionId))
                queryOutposts = queryOutposts.Where(it => it.Region.Id == new Guid(inputModel.regionId));
            if (!string.IsNullOrEmpty(inputModel.districtId))
                queryOutposts = queryOutposts.Where(it => it.District.Id == new Guid(inputModel.districtId));

            return queryOutposts.ToList();
        }

        private PatientsModel GetNumberOfPatients(Outpost outpost, string startDate, string endDate, Diagnosis diagnosis)
        {
            var dispensaryQuery = QueryMessageFromDispensary.Query().Where(it => it.Diagnosises.Contains(diagnosis));
            if (outpost != null)
                dispensaryQuery = dispensaryQuery.Where(it => it.OutpostId == outpost.Id);

            DateTime outputStartDate;
            if (!string.IsNullOrEmpty(startDate))
                if (DateTime.TryParse(startDate, out outputStartDate))
                    dispensaryQuery = dispensaryQuery.Where(it => it.SentDate >= outputStartDate);

            DateTime outputEndDate;
            if (!string.IsNullOrEmpty(endDate))
                if (DateTime.TryParse(endDate, out outputEndDate))
                    dispensaryQuery = dispensaryQuery.Where(it => it.SentDate <= outputEndDate);

            int numberOfFemales = dispensaryQuery.Where(it => it.MessageFromDrugShop.Gender.ToUpper() == "F").Count();
            int numberOfMales = dispensaryQuery.Where(it => it.MessageFromDrugShop.Gender.ToUpper() == "M").Count();
            int numberOfPatients = dispensaryQuery.Count();

            return new PatientsModel { Female = numberOfFemales, Male = numberOfMales, TotalNumber = numberOfPatients };
        }

        public JsonResult GetChartData(DiagnosisIndexModel inputModel)
        {
            var listDataForChart = GetDataForChart(inputModel);

            return Json(new DiagnosisReportIndexOutputModel
            {
                Diagnosis = listDataForChart.ToArray(),
                TotalItems = listDataForChart.Count()
            }, JsonRequestBehavior.AllowGet);
        }

        private List<DiagnosisReportModel> GetDataForChart(DiagnosisIndexModel inputModel)
        {
            LoadUserAndClient();

            var queryDiagnosis = QueryDiagnosis.Query().Where(it => it.Client == _client);
            List<Outpost> outpostList = GetListOfOutpostsAfterFiltering(inputModel);
            List<DiagnosisReportModel> diagnosisList = new List<DiagnosisReportModel>();

            foreach (var diagnosis in queryDiagnosis.ToList())
            {
                int female = 0;
                int male = 0;
                int total = 0;

                foreach (var dispensary in outpostList)
                {
                    PatientsModel patients = GetNumberOfPatients(dispensary, inputModel.startDate, inputModel.endDate, diagnosis);
                    female += patients.Female;
                    male += patients.Male;
                    total += patients.TotalNumber;
                }

                DiagnosisReportModel model = new DiagnosisReportModel();
                model.Diagnosis = diagnosis.Code;
                model.Female = female;
                model.Male = male;
                model.NumberOfPatients = total;

                diagnosisList.Add(model);
            }

            return diagnosisList;
        }

        [HttpPost]
        public void ExportToExcel(DiagnosisIndexModel model)
        {
            Response.Clear();
            Response.ContentType = "application/vnd.xls";
            Response.AddHeader("Content-disposition", "attachment; filename=" + "DiagnosisReport" + DateTime.UtcNow.ToShortDateString() + ".xls");

            var reportData = GetDataForReport(model);
            var chartData = GetDataForChart(model);
            DiagnosisIndexModel outputDataModel = GetFiltersForExcel(model);

            StreamWriter writer = new StreamWriter(Response.OutputStream);

            writer.WriteLine("Country:\t" + outputDataModel.countryId + "\t \t");
            writer.WriteLine("Region:\t" + outputDataModel.regionId + "\t \t");
            writer.WriteLine("District:\t" + outputDataModel.districtId + "\t \t");
            writer.WriteLine("Start date:\t" + outputDataModel.startDate + "\t \t");
            writer.WriteLine("End date:\t" + outputDataModel.endDate + "\t \t");
            writer.WriteLine(" ");
            writer.WriteLine(" ");

            writer.WriteLine("Diagnosis\t" + "Health facility\t" + "Females\t" + "Males\t" + "Number of patients\t \t");

            foreach (var diagnosis in chartData)
            {
                writer.WriteLine(diagnosis.Diagnosis + "\t \t" + diagnosis.Female + "\t" + diagnosis.Male + "\t" + diagnosis.NumberOfPatients + "\t \t");

                foreach (var item in reportData)
                {
                    if (item.Diagnosis.ToUpper() == diagnosis.Diagnosis.ToUpper())
                        writer.WriteLine(" \t" + item.Outpost + "\t" + item.Female + "\t" + item.Male + "\t" + item.NumberOfPatients + "\t \t");
                }
            }
            writer.Close();

            Response.End();

        }

        private DiagnosisIndexModel GetFiltersForExcel(DiagnosisIndexModel model)
        {
            DiagnosisIndexModel outputModel = new DiagnosisIndexModel();

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
