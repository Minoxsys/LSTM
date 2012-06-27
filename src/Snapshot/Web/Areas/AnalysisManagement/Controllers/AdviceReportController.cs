using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.Persistence;
using Domain;
using Core.Domain;
using Web.Areas.AnalysisManagement.Models.AdviceReport;
using Web.Security;
using System.IO;

namespace Web.Areas.AnalysisManagement.Controllers
{
    public class AdviceReportController : Controller
    {
        public IQueryService<MessageFromDispensary> QueryMessageFromDispensary { get; set; }
        public IQueryService<Outpost> QueryOutpost { get; set; }
        public IQueryService<Advice> QueryAdvice { get; set; }
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

        public JsonResult GetAdviceReport(AdviceIndexModel inputModel)
        {

            List<AdviceReportModel> listOfDataForReport = GetDataForReport(inputModel);

            return Json(new AdviceReportIndexOutputModel
            {
                Advice = listOfDataForReport.ToArray(),
                TotalItems = listOfDataForReport.Count()
            }, JsonRequestBehavior.AllowGet);
        }

        private List<AdviceReportModel> GetDataForReport(AdviceIndexModel inputModel)
        {
            LoadUserAndClient();

            var queryAdvice = QueryAdvice.Query().Where(it => it.Client == _client);
            List<Outpost> outpostList = GetListOfOutpostsAfterFiltering(inputModel);
            List<AdviceReportModel> adviceList = new List<AdviceReportModel>();

            foreach (var advice in queryAdvice.ToList())
            {
                foreach (var dispensary in outpostList)
                {
                    AdviceReportModel model = new AdviceReportModel();
                    model.Advice = advice.Code;
                    model.Outpost = dispensary.Name + " (" + dispensary.District.Name + ")";

                    PatientsModel patients = GetNumberOfPatients(dispensary, inputModel.startDate, inputModel.endDate, advice);
                    model.Female = patients.Female;
                    model.Male = patients.Male;
                    model.NumberOfPatients = patients.TotalNumber;

                    adviceList.Add(model);
                }
            }

            return adviceList;
        }

        private List<Outpost> GetListOfOutpostsAfterFiltering(AdviceIndexModel inputModel)
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

        private PatientsModel GetNumberOfPatients(Outpost outpost, string startDate, string endDate, Advice advice)
        {
            var dispensaryQuery = QueryMessageFromDispensary.Query().Where(it => it.Advices.Contains(advice));
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

        public JsonResult GetChartData(AdviceIndexModel inputModel)
        {
            var listDataForChart = GetDataForChart(inputModel);

            return Json(new AdviceReportIndexOutputModel
            {
                Advice = listDataForChart.ToArray(),
                TotalItems = listDataForChart.Count()
            }, JsonRequestBehavior.AllowGet);
        }

        private List<AdviceReportModel> GetDataForChart(AdviceIndexModel inputModel)
        {
            LoadUserAndClient();

            var queryAdvice = QueryAdvice.Query().Where(it => it.Client == _client);
            List<Outpost> outpostList = GetListOfOutpostsAfterFiltering(inputModel);
            List<AdviceReportModel> adviceList = new List<AdviceReportModel>();

            foreach (var advice in queryAdvice.ToList())
            {
                int female = 0;
                int male = 0;
                int total = 0;

                foreach (var dispensary in outpostList)
                {
                    PatientsModel patients = GetNumberOfPatients(dispensary, inputModel.startDate, inputModel.endDate, advice);
                    female += patients.Female;
                    male += patients.Male;
                    total += patients.TotalNumber;
                }

                AdviceReportModel model = new AdviceReportModel();
                model.Advice = advice.Code;
                model.Female = female;
                model.Male = male;
                model.NumberOfPatients = total;

                adviceList.Add(model);
            }

            return adviceList;
        }

        [HttpPost]
        public void ExportToExcel(AdviceIndexModel model)
        {
            Response.Clear();
            Response.ContentType = "application/vnd.xls";
            Response.AddHeader("Content-disposition", "attachment; filename=" + "PatientsReport" + DateTime.UtcNow.ToShortDateString() + ".xls");

            var reportData = GetDataForReport(model);
            var chartData = GetDataForChart(model);
            AdviceIndexModel outputDataModel = GetFiltersForExcel(model);

            StreamWriter writer = new StreamWriter(Response.OutputStream);

            writer.WriteLine("Country:\t" + outputDataModel.countryId + "\t \t");
            writer.WriteLine("Region:\t" + outputDataModel.regionId + "\t \t");
            writer.WriteLine("District:\t" + outputDataModel.districtId + "\t \t");
            writer.WriteLine("Start date:\t" + outputDataModel.startDate + "\t \t");
            writer.WriteLine("End date:\t" + outputDataModel.endDate + "\t \t");
            writer.WriteLine(" ");
            writer.WriteLine(" ");

            writer.WriteLine("Advice\t" + "Health facility\t" + "Females\t" + "Males\t" + "Number of patients\t \t");

            foreach (var advice in chartData)
            {
                writer.WriteLine(advice.Advice + "\t \t" + advice.Female + "\t" + advice.Male + "\t" + advice.NumberOfPatients + "\t \t");

                foreach (var item in reportData)
                {
                    if (item.Advice.ToUpper() == advice.Advice.ToUpper())
                        writer.WriteLine(" \t" + item.Outpost + "\t" + item.Female + "\t" + item.Male + "\t" + item.NumberOfPatients + "\t \t");
                }
            }
            writer.Close();

            Response.End();

        }

        private AdviceIndexModel GetFiltersForExcel(AdviceIndexModel model)
        {
            AdviceIndexModel outputModel = new AdviceIndexModel();

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
