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

        private string _countryName;
        private string _regionName;
        private string _districtName;
        private string _outpostType;

        [HttpGet]
        [Requires(Permissions = "Report.View")]
        public ActionResult Overview()
        {
            return View();
        }

        public JsonResult GetHealthFacilityReport(HealthFacilityIndexModel inputModel)
        {
            var reportData = GetDataForReport(inputModel);
            
            return Json(new HealthFacilityIndexOutputModel
            {
                Outposts = reportData.ToArray(),
                TotalItems = reportData.Count()
            }, JsonRequestBehavior.AllowGet);
        }

        private List<HealthFacilityModel> GetDataForReport(HealthFacilityIndexModel inputModel)
        {
            LoadUserAndClient();

            var outpostDataQuery = QueryOutpost.Query().Where(it => it.Client == _client);

            if (!string.IsNullOrEmpty(inputModel.countryId))
            {
                outpostDataQuery = outpostDataQuery.Where(it => it.Country.Id == new Guid(inputModel.countryId));
                var outpost = outpostDataQuery.FirstOrDefault<Outpost>();
                _countryName = outpost.Country.Name;
            }
            if (!string.IsNullOrEmpty(inputModel.regionId))
            {
                outpostDataQuery = outpostDataQuery.Where(it => it.Region.Id == new Guid(inputModel.regionId));
                var outpost = outpostDataQuery.FirstOrDefault<Outpost>();
                _regionName = outpost.Region.Name;
            }
            if (!string.IsNullOrEmpty(inputModel.districtId))
            {
                outpostDataQuery = outpostDataQuery.Where(it => it.District.Id == new Guid(inputModel.districtId));
                var outpost = outpostDataQuery.FirstOrDefault<Outpost>();
                _districtName = outpost.District.Name;
            }
            if (!string.IsNullOrEmpty(inputModel.outpostType))
            {
                outpostDataQuery = outpostDataQuery.Where(it => it.OutpostType.Type == Int32.Parse(inputModel.outpostType));
                var outpost = outpostDataQuery.FirstOrDefault<Outpost>();
                _outpostType = outpost.OutpostType.Name;
            }

            var outpostModelListProjection = (from outpost in outpostDataQuery.ToList()
                                              select new HealthFacilityModel
                                              {
                                                  OutpostName = outpost.Name + " (" + outpost.District.Name + ")",
                                                  NumberOfPatients = inputModel.outpostType == null ? "" : GetNumberOfPatientsForOutpostType(inputModel, outpost.Id, "").ToString(),
                                                  Female = inputModel.outpostType == null ? "" : GetNumberOfPatientsForOutpostType(inputModel, outpost.Id, "F").ToString(),
                                                  Male = inputModel.outpostType == null ? "" : GetNumberOfPatientsForOutpostType(inputModel, outpost.Id, "M").ToString(),
                                              }).ToList();

            return outpostModelListProjection;
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
            Response.AddHeader("Content-disposition", "attachment; filename=" + "HealthFacilityLevel"+DateTime.UtcNow.ToShortDateString()+".xls");

            var reportData = GetDataForReport(model);

            StreamWriter writer = new StreamWriter(Response.OutputStream);

            writer.WriteLine("Country:\t" + _countryName + "\t \t");
            writer.WriteLine("Region:\t" + _regionName + "\t \t");
            writer.WriteLine("District:\t" + _districtName + "\t \t");
            writer.WriteLine("Start date:\t" + model.startDate + "\t \t");
            writer.WriteLine("End date:\t" + model.endDate + "\t \t");
            writer.WriteLine("Health facility:\t" + _outpostType + "\t \t");
            writer.WriteLine(" ");
            writer.WriteLine(" ");

            writer.WriteLine("Health facility\t" + "Females\t" + "Males\t" + "Number of patients\t \t");

            foreach (var item in reportData)
            {
                writer.WriteLine(item.OutpostName + "\t" + item.Female + "\t" + item.Male + "\t" + item.NumberOfPatients + "\t \t");
            }
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
