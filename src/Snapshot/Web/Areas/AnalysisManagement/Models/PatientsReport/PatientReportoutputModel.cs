using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.AnalysisManagement.Models.PatientsReport
{
    public class PatientReportoutputModel
    {
        public string Initials { get; set; }
        public string PatientID { get; set; }
        public string Gender { get; set; }
        public string Age { get; set; }
        public string Drugshop { get; set; }
        public string DrugshopDate { get; set; }
        public string Condition { get; set; }
        public string Dispensary { get; set; }
        public string DispensaryDate { get; set; }
        public string Diagnosis { get; set; }
        public string Treatment { get; set; }
        public string Advice { get; set; }
        public string DidNotAttendReason { get; set; }
    }
}