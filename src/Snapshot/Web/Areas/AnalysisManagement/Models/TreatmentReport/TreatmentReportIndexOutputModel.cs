using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.AnalysisManagement.Models.TreatmentReport
{
    public class TreatmentReportIndexOutputModel
    {
        public TreatmentReportModel[] Treatment { get; set; }
        public int TotalItems { get; set; }
    }
}