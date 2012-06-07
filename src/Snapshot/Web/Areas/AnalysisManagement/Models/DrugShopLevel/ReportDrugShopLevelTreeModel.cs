using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.AnalysisManagement.Models.DrugShopLevel
{
    public class ReportDrugShopLevelTreeModel
    {
        public String Name { get; set; }
        public Guid Id { get; set; }

        public bool leaf { get; set; }
        public bool expanded { get; set; }

        public List<ReportDrugShopLevelTreeModel> children { get; set; }

        public ReportDrugShopLevelTreeModel()
        {
            this.children = new List<ReportDrugShopLevelTreeModel>();
        }
    }
}