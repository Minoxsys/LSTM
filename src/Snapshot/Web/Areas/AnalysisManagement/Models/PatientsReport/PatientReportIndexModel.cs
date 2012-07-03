using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.AnalysisManagement.Models.PatientsReport
{
    public class PatientReportIndexModel
    {
        public string countryId { get; set; }
        public string regionId { get; set; }
        public string districtId { get; set; }

        public string startDate { get; set; }
        public string endDate { get; set; }

        public string conditionId { get; set; }
        public string diagnosisId { get; set; }
        public string treatmentId { get; set; }
        public string adviceId { get; set; }

        public string gender { get; set; }
        public string ageInterval { get; set; }
    }
}