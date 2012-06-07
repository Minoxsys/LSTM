using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain;
using Core.Persistence;
using Web.Areas.AnalysisManagement.Models.DrugShopLevel;

namespace Web.Areas.AnalysisManagement.Controllers
{
    public class DrugShopLevelController : Controller
    {
        public IQueryService<MessageFromDrugShop> QueryMessageFromDrugShop { get; set; }
        public IQueryService<ServiceNeeded> QueryServiceNeeded { get; set; }
        public IQueryService<Outpost> QueryOutpost { get; set; }

        public ActionResult Overview()
        {
            return View();
        }

        public JsonResult GetReports(InputModel inputModel)
        {
            var reportDrugShopLevel = QueryOutpost.Query().Where(it => it.Country.Id == inputModel.CountryId &&
                                                                        it.Region.Id == inputModel.RegionId &&
                                                                        it.District.Id == inputModel.DistrictId &&
                                                                        it.OutpostType.Name == "Drug Shop").ToList();

            var reportRegionLevelTreeModel = GetDrugShopTreeModel(reportDrugShopLevel);
            return Json(reportRegionLevelTreeModel, JsonRequestBehavior.AllowGet);
        }

        private ReportDrugShopLevelTreeModel GetDrugShopTreeModel(List<Outpost> reportDrugShopLevel)
        {
            var reportLevelTreeModel = new ReportDrugShopLevelTreeModel { Name = "root" };

            foreach (var group in reportDrugShopLevel)
            {
                var node = ToDrugShopNode(group);
                //reportLevelTreeModel.children.Add(node);
            }
            return reportLevelTreeModel;
        }

        private object ToDrugShopNode(Outpost group)
        {
            throw new NotImplementedException();
        }

        public List<MessageFromDrugShop> GetMessagesFor(Guid serviceNeededId)
        {
            ServiceNeeded service = QueryServiceNeeded.Load(serviceNeededId);
            var messages = QueryMessageFromDrugShop.Query().Where(it => it.ServicesNeeded.Contains(service));
            return messages.ToList<MessageFromDrugShop>();
        }




    }
}
