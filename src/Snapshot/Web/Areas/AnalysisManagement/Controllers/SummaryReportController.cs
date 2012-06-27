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

namespace Web.Areas.AnalysisManagement.Controllers
{
    public class SummaryReportController : Controller
    {
        public IQueryService<MessageFromDispensary> QueryMessageFromDispensary { get; set; }
        public IQueryService<MessageFromDrugShop> QueryMessageFromDrugShop { get; set; }
        public IQueryService<Outpost> QueryOutpost { get; set; }
        public IQueryService<Client> QueryClients { get; set; }
        public IQueryService<User> QueryUsers { get; set; }

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
                model.FailedToReport = GetNumberOfPatientsWhoFailedToReportToDispensary(dispensary);
                model.NotTreated = GetNumberOfPatientsNotTreatedAt(dispensary);
                model.Treated = GetNumberOfPatientsTreatedAt(dispensary);
                model.DistinctPatients = GetIndividualPatientsAt(dispensary);

                summaryList.Add(model);
            }


            return Json(new SummaryReportIndexOutputModel
            {
                Patients = summaryList.ToArray(),
                TotalItems = summaryList.Count()
            }, JsonRequestBehavior.AllowGet);

        }

        private int GetNumberOfPatientsWhoFailedToReportToDispensary(Outpost dispensary)
        {
            var drugshopsFrorDispensary = QueryOutpost.Query().Where(it => it.Warehouse.Id == dispensary.Id).ToList();
            int sum = 0;
            foreach(var drugshop in drugshopsFrorDispensary)
            {
                var drugshopMessages = QueryMessageFromDrugShop.Query().Where(it => it.OutpostId == drugshop.Id);
                foreach (var message in drugshopMessages)
                {
                    if (QueryMessageFromDispensary.Query().Where(it => it.MessageFromDrugShop.Id == message.Id).Any() == false)
                        sum++;
                }
            }
            return sum;
        }

        private int GetNumberOfPatientsNotTreatedAt(Outpost dispensary)
        {
            return QueryMessageFromDispensary.Query().Where(it => it.OutpostId == dispensary.Id && it.Treatments.FirstOrDefault(c => c.Code == NOTREATMENT) != null).Count();
        }

        private int GetNumberOfPatientsTreatedAt(Outpost dispensary)
        {
            return QueryMessageFromDispensary.Query().Where(it => it.OutpostId == dispensary.Id && it.Treatments.FirstOrDefault(c => c.Code == NOTREATMENT) == null).Count();
        }

        private int GetIndividualPatientsAt(Outpost dispensary)
        {
            var drugshopsFrorDispensary = QueryOutpost.Query().Where(it => it.Warehouse.Id == dispensary.Id).ToList();
            int sum = 0;
            foreach (var drugshop in drugshopsFrorDispensary)
            {
                sum += QueryMessageFromDrugShop.Query().Where(it => it.OutpostId == drugshop.Id).Distinct().Count();
                
            }
            return sum;
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
