using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Areas.AnalysisManagement.Models.DiagnosisReport;
using Web.Areas.AnalysisManagement.Models.SymptomsReport;
using Core.Persistence;
using Domain;
using Core.Domain;
using Web.Security;
using System.IO;

namespace Web.Areas.AnalysisManagement.Controllers
{
    public class SymptomsReportController : Controller
    {
        public IQueryService<MessageFromDrugShop> QueryMessageFromDrugShop { get; set; }
        public IQueryService<Outpost> QueryOutpost { get; set; }
        public IQueryService<Condition> QuerySymptoms { get; set; }
        public IQueryService<Client> QueryClients { get; set; }
        public IQueryService<User> QueryUsers { get; set; }
        public IQueryService<Country> QueryCountry { get; set; }
        public IQueryService<Region> QueryRegion { get; set; }
        public IQueryService<District> QueryDistrict { get; set; }

        public Client _client;
        public User _user;

        public JsonResult GetSymptomsReport(SymptomsIndexModel inputModel)
        {

            List<SymptomsReportModel> listOfDataForReport = GetDataForReport(inputModel);

            return Json(new SymptomsReportIndexOutputModel
            {
                Symptoms = listOfDataForReport.ToArray(),
                TotalItems = listOfDataForReport.Count()
            }, JsonRequestBehavior.AllowGet);
        }

        private List<SymptomsReportModel> GetDataForReport(SymptomsIndexModel inputModel)
        {
            LoadUserAndClient();

            var querySymptoms = QuerySymptoms.Query().Where(it => it.Client == _client);
            List<Outpost> outpostList = GetListOfOutpostsAfterFiltering(inputModel);
            List<SymptomsReportModel> SymptomsList = new List<SymptomsReportModel>();

            foreach (var symptom in querySymptoms.ToList())
            {
                foreach (var drugshop in outpostList)
                {
                    if (drugshop != null)
                    {
                        SymptomsReportModel model = new SymptomsReportModel();
                        model.Symptom = symptom.Code;
                        model.Outpost = drugshop.Name + " (" + drugshop.District.Name + ")";

                        PatientsModel patients = GetNumberOfPatients(drugshop, inputModel.startDate, inputModel.endDate, symptom);
                        model.Female = patients.Female;
                        model.Male = patients.Male;
                        model.NumberOfPatients = patients.TotalNumber;

                        SymptomsList.Add(model);
                    }
                }
            }

            return SymptomsList;
        }

        private List<Outpost> GetListOfOutpostsAfterFiltering(SymptomsIndexModel inputModel)
        {
            var queryOutposts = QueryOutpost.Query().Where(it => it.Client == _client && it.OutpostType.Type == 0); //get outposts of type "drug shop"
            if (!string.IsNullOrEmpty(inputModel.countryId))
                queryOutposts = queryOutposts.Where(it => it.Country.Id == new Guid(inputModel.countryId));
            if (!string.IsNullOrEmpty(inputModel.regionId))
                queryOutposts = queryOutposts.Where(it => it.Region.Id == new Guid(inputModel.regionId));
            if (!string.IsNullOrEmpty(inputModel.districtId))
                queryOutposts = queryOutposts.Where(it => it.District.Id == new Guid(inputModel.districtId));

            return queryOutposts.ToList();
        }

        private PatientsModel GetNumberOfPatients(Outpost outpost, string startDate, string endDate, Condition symptom)
        {
            
            var drugshopQuery = QueryMessageFromDrugShop.Query().Where(it => it.ServicesNeeded.Contains(symptom));
            if (outpost != null)
                drugshopQuery = drugshopQuery.Where(it => it.OutpostId == outpost.Id);
            
            //var drugshopQuery = symptom.Messages.Where(it=>it.OutpostId==outpost.Id);

            DateTime outputStartDate;
            if (!string.IsNullOrEmpty(startDate))
                if (DateTime.TryParse(startDate, out outputStartDate))
                    drugshopQuery = drugshopQuery.Where(it => it.SentDate >= outputStartDate);

            DateTime outputEndDate;
            if (!string.IsNullOrEmpty(endDate))
                if (DateTime.TryParse(endDate, out outputEndDate))
                    drugshopQuery = drugshopQuery.Where(it => it.SentDate <= outputEndDate);

            int numberOfFemales = drugshopQuery.Count(it => it.Gender.ToUpper() == "F");
            int numberOfMales = drugshopQuery.Count(it => it.Gender.ToUpper() == "M");
            int numberOfPatients = drugshopQuery.Count();

            return new PatientsModel { Female = numberOfFemales, Male = numberOfMales, TotalNumber = numberOfPatients };
        }

        public JsonResult GetChartData(SymptomsIndexModel inputModel)
        {
            var listDataForChart = GetDataForChart(inputModel);

            return Json(new SymptomsReportIndexOutputModel
            {
                Symptoms = listDataForChart.ToArray(),
                TotalItems = listDataForChart.Count()
            }, JsonRequestBehavior.AllowGet);
        }

        private List<SymptomsReportModel> GetDataForChart(SymptomsIndexModel inputModel)
        {
            LoadUserAndClient();

            var querySymptoms = QuerySymptoms.Query().Where(it => it.Client == _client);
            List<Outpost> outpostList = GetListOfOutpostsAfterFiltering(inputModel);
            List<SymptomsReportModel> symptomsList = new List<SymptomsReportModel>();

            foreach (var symptom in querySymptoms.ToList())
            {
                int female = 0;
                int male = 0;
                int total = 0;

                foreach (var drugshop in outpostList)
                {
                    if (drugshop != null)
                    {
                        PatientsModel patients = GetNumberOfPatients(drugshop, inputModel.startDate, inputModel.endDate, symptom);
                        female += patients.Female;
                        male += patients.Male;
                        total += patients.TotalNumber;
                    }
                }

                SymptomsReportModel model = new SymptomsReportModel();
                model.Symptom = symptom.Code;
                model.Female = female;
                model.Male = male;
                model.NumberOfPatients = total;

                symptomsList.Add(model);
            }

            return symptomsList;
        }

        [HttpPost]
        public void ExportToExcel(SymptomsIndexModel model)
        {
            Response.Clear();
            Response.ContentType = "application/vnd.xls";
            Response.AddHeader("Content-disposition", "attachment; filename=" + "SymptomsReport" + DateTime.UtcNow.ToShortDateString() + ".xls");

            var reportData = GetDataForReport(model);
            var chartData = GetDataForChart(model);
            SymptomsIndexModel outputDataModel = GetFiltersForExcel(model);

            StreamWriter writer = new StreamWriter(Response.OutputStream);

            writer.WriteLine("Country:\t" + outputDataModel.countryId + "\t \t");
            writer.WriteLine("Region:\t" + outputDataModel.regionId + "\t \t");
            writer.WriteLine("District:\t" + outputDataModel.districtId + "\t \t");
            writer.WriteLine("Start date:\t" + outputDataModel.startDate + "\t \t");
            writer.WriteLine("End date:\t" + outputDataModel.endDate + "\t \t");
            writer.WriteLine(" ");
            writer.WriteLine(" ");

            writer.WriteLine("Symptom\t" + "Health facility\t" + "Females\t" + "Males\t" + "Number of patients\t \t");

            foreach (var symptom in chartData)
            {
                writer.WriteLine(symptom.Symptom + "\t \t" + symptom.Female + "\t" + symptom.Male + "\t" + symptom.NumberOfPatients + "\t \t");

                foreach (var item in reportData)
                {
                    if (item.Symptom.ToUpper() == symptom.Symptom.ToUpper())
                        writer.WriteLine(" \t" + item.Outpost + "\t" + item.Female + "\t" + item.Male + "\t" + item.NumberOfPatients + "\t \t");
                }
            }
            writer.Close();

            Response.End();

        }

        private SymptomsIndexModel GetFiltersForExcel(SymptomsIndexModel model)
        {
            SymptomsIndexModel outputModel = new SymptomsIndexModel();

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
