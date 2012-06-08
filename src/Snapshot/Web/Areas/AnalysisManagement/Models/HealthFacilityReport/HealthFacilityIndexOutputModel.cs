using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.AnalysisManagement.Models.HealthFacilityReport
{
    public class HealthFacilityIndexOutputModel
    {
        public HealthFacilityModel[] Outposts { get; set; }
        public int TotalItems { get; set; }
    }
}