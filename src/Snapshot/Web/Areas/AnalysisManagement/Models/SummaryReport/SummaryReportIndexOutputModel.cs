using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.AnalysisManagement.Models.SummaryReport
{
    public class SummaryReportIndexOutputModel
    {
        public SummaryReportModel[] Patients { get; set; }
        public int TotalItems { get; set; }
    }
}