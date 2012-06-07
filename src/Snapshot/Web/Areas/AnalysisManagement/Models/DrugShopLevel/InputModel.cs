using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.AnalysisManagement.Models.DrugShopLevel
{
    public class InputModel
    {
        public Guid CountryId { get; set; }
        public Guid RegionId { get; set; }
        public Guid DistrictId { get; set; }
    }
}