using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.AnalysisManagement.Models.AdviceReport
{
    public class AdviceReportIndexOutputModel
    {
        public AdviceReportModel[] Advice { get; set; }
        public int TotalItems { get; set; }
    }
}