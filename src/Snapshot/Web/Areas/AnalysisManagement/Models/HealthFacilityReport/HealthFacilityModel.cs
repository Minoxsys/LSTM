using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.AnalysisManagement.Models.HealthFacilityReport
{
    public class HealthFacilityModel
    {
        public string OutpostName { get; set; }
        public string NumberOfPatients { get; set; }
        public string Female { get; set; }
        public string Male { get; set; }
    }
}