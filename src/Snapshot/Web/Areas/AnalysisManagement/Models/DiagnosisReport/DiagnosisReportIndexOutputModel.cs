using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.AnalysisManagement.Models.DiagnosisReport
{
    public class DiagnosisReportIndexOutputModel
    {
        public DiagnosisReportModel[] Diagnosis { get; set; }
        public int TotalItems { get; set; }
    }
}