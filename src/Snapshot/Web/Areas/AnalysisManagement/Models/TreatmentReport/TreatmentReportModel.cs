using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.AnalysisManagement.Models.TreatmentReport
{
    public class TreatmentReportModel
    {
        public string Treatment { get; set; }
        public string Outpost { get; set; }
        public int Female { get; set; }
        public int Male { get; set; }
        public int NumberOfPatients { get; set; }
    }
}