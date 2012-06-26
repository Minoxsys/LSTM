using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain;
using Core.Persistence;
using Web.Areas.AnalysisManagement.Models.HealthFacilityReport;
using System.Globalization;
using Core.Domain;
using Microsoft.Office.Interop;
using Microsoft.Office.Interop.Excel;
using System.IO;
using System.Web.UI;
using System.Net;
using System.Web.UI.WebControls;
using Web.Security;

namespace Web.Areas.AnalysisManagement.Controllers
{
    public class HealthFacilityReportController : Controller
    {
        public IQueryService<MessageFromDrugShop> QueryMessageFromDrugShop { get; set; }
        public IQueryService<MessageFromDispensary> QueryMessageFromDispensary { get; set; }
        public IQueryService<Outpost> QueryOutpost { get; set; }
        public IQueryService<Client> QueryClients { get; set; }
        public IQueryService<User> QueryUsers { get; set; }

        private Client _client;
        private User _user;

        [HttpGet]
        [Requires(Permissions = "Report.View")]
        public ActionResult Overview()
        {
            return View();
        }

        public JsonResult GetHealthFacilityReport(HealthFacilityIndexModel inputModel)
        {
            LoadUserAndClient();

            var outpostDataQuery = QueryOutpost.Query().Where(it => it.Client == _client);

            if (!string.IsNullOrEmpty(inputModel.countryId))
                outpostDataQuery = outpostDataQuery.Where(it => it.Country.Id == new Guid(inputModel.countryId));
            if (!string.IsNullOrEmpty(inputModel.regionId))
                outpostDataQuery = outpostDataQuery.Where(it => it.Region.Id == new Guid(inputModel.regionId));
            if (!string.IsNullOrEmpty(inputModel.districtId))
                outpostDataQuery = outpostDataQuery.Where(it => it.District.Id == new Guid(inputModel.districtId));
            if (!string.IsNullOrEmpty(inputModel.outpostType))
                outpostDataQuery = outpostDataQuery.Where(it => it.OutpostType.Type == Int32.Parse(inputModel.outpostType));

            var totalItems = outpostDataQuery.Count();

            var outpostModelListProjection = (from outpost in outpostDataQuery.ToList()
                                             select new HealthFacilityModel
                                             {
                                                 OutpostName = outpost.Name + " (" + outpost.District.Name + ")",
                                                 NumberOfPatients = inputModel.outpostType == null ? "" : GetNumberOfPatientsForOutpostType(inputModel, outpost.Id, "").ToString(),
                                                 Female = inputModel.outpostType == null ? "" : GetNumberOfPatientsForOutpostType(inputModel, outpost.Id, "F").ToString(),
                                                 Male = inputModel.outpostType == null ? "" : GetNumberOfPatientsForOutpostType(inputModel, outpost.Id, "M").ToString(),
                                             }).ToArray();
            return Json(new HealthFacilityIndexOutputModel
            {
                Outposts = outpostModelListProjection,
                TotalItems = totalItems
            }, JsonRequestBehavior.AllowGet);
        }

        private int GetNumberOfPatientsForOutpostType(HealthFacilityIndexModel inputModel, Guid id, string gender)
        {
            var drugShopQuery = QueryMessageFromDrugShop.Query();
            var dispensaryQuery = QueryMessageFromDispensary.Query();
            DateTime startDate, endDate;
            int type = Int32.Parse(inputModel.outpostType);

            if (type == 0)
            {
                if (!string.IsNullOrEmpty(inputModel.startDate)) 
                    if (DateTime.TryParse(inputModel.startDate, out startDate))
                        drugShopQuery = drugShopQuery.Where(it => it.SentDate >= startDate);
                if (!string.IsNullOrEmpty(inputModel.endDate))
                    if (DateTime.TryParse(inputModel.endDate, out endDate))
                        drugShopQuery = drugShopQuery.Where(it => it.SentDate <= endDate);

                if (!string.IsNullOrEmpty(gender))
                    drugShopQuery = drugShopQuery.Where(it => it.Gender.ToUpper() == gender.ToUpper());

                return drugShopQuery.Where(it => it.OutpostId == id).Count();
            }

            if (type == 1 || type == 2)
            {
                if (!string.IsNullOrEmpty(inputModel.startDate))
                    if (DateTime.TryParse(inputModel.startDate, out startDate))
                        dispensaryQuery = dispensaryQuery.Where(it => it.SentDate >= startDate);
                if(!string.IsNullOrEmpty(inputModel.endDate))
                    if (DateTime.TryParse(inputModel.endDate, out endDate))
                        dispensaryQuery = dispensaryQuery.Where(it => it.SentDate <= endDate);

                if (!string.IsNullOrEmpty(gender))
                    dispensaryQuery = dispensaryQuery.Where(it => it.MessageFromDrugShop.Gender.ToUpper() == gender.ToUpper());
                return dispensaryQuery.Where(it => it.OutpostId == id && it.OutpostType == type).Count();
            }
            return 0;
        }

        [HttpPost]
        public void ExportToExcel(HealthFacilityIndexModel model)
        {

            Response.Clear();
            Response.ContentType = "application/vnd.xls";
            Response.AddHeader("Content-disposition", "attachment; filename=" + "report.xls");

            StreamWriter writer = new StreamWriter(Response.OutputStream);
            writer.WriteLine(model.countryId);
            writer.WriteLine(model.regionId);
            writer.WriteLine(model.districtId);
            writer.Close();

            Response.End();

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
