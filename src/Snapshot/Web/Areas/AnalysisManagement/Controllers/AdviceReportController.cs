using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.Persistence;
using Domain;
using Core.Domain;
using Web.Areas.AnalysisManagement.Models.AdviceReport;

namespace Web.Areas.AnalysisManagement.Controllers
{
    public class AdviceReportController : Controller
    {
        public IQueryService<MessageFromDispensary> QueryMessageFromDispensary { get; set; }
        public IQueryService<Outpost> QueryOutpost { get; set; }
        public IQueryService<Advice> QueryAdvice { get; set; }
        public IQueryService<Client> QueryClients { get; set; }
        public IQueryService<User> QueryUsers { get; set; }

        private Client _client;
        private User _user;


        public ActionResult Overview()
        {
            return View();
        }

        public JsonResult GetAdviceReport(AdviceIndexModel inputModel)
        {
            LoadUserAndClient();

            var queryAdvice = QueryAdvice.Query().Where(it => it.Client == _client);
            var queryOutposts = QueryOutpost.Query().Where(it => it.Client == _client && it.OutpostType.Type > 0);
            if (!string.IsNullOrEmpty(inputModel.countryId))
                queryOutposts = queryOutposts.Where(it => it.Country.Id == new Guid(inputModel.countryId));
            if (!string.IsNullOrEmpty(inputModel.regionId))
                queryOutposts = queryOutposts.Where(it => it.Region.Id == new Guid(inputModel.regionId));
            if (!string.IsNullOrEmpty(inputModel.districtId))
                queryOutposts = queryOutposts.Where(it => it.District.Id == new Guid(inputModel.districtId));

            List<AdviceReportModel> adviceList = new List<AdviceReportModel>();

            foreach (var advice in queryAdvice.ToList())
            {
                foreach (var dispensary in queryOutposts.ToList())
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

            return Json(new AdviceReportIndexOutputModel
            {
                Advice = adviceList.ToArray(),
                TotalItems = adviceList.Count()
            }, JsonRequestBehavior.AllowGet);
        }

        private PatientsModel GetNumberOfPatients(Outpost outpost, string startDate, string endDate, Advice advice)
        {
            var dispensaryQuery = QueryMessageFromDispensary.Query().Where(it => it.OutpostId == outpost.Id && it.Advices.Contains(advice));

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
