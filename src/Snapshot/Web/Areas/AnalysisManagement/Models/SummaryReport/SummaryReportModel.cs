using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.AnalysisManagement.Models.SummaryReport
{
    public class SummaryReportModel
    {
        public string Outpost { get; set; }
        public int FailedToReport { get; set; }
        public int Treated { get; set; }
        public int NotTreated { get; set; }
        public int DistinctPatients { get; set; }
    }
}