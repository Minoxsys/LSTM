using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.AnalysisManagement.Models.TreatmentReport
{
    public class TreatmentIndexModel
    {
        public string countryId { get; set; }
        public string regionId { get; set; }
        public string districtId { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
    }
}