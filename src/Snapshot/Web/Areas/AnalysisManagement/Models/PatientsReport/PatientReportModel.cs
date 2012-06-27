using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Areas.LocationManagement.Models.Outpost;
using Domain;

namespace Web.Areas.AnalysisManagement.Models.PatientsReport
{
    public class PatientReportModel
    {
        public PatientReportoutputModel ReportModel { get; set; }

        public Outpost Outpost { get; set; }
        public IList<ServiceNeeded> ConditionList { get; set; }
        public IList<Diagnosis> DiagnosisList { get; set; }
        public IList<Treatment> TreatmentList { get; set; }
        public IList<Advice> AdviceList { get; set; }

        public PatientReportModel()
        {
            ReportModel = new PatientReportoutputModel();
            Outpost = new Outpost();
            ConditionList = new List<ServiceNeeded>();
            DiagnosisList = new List<Diagnosis>();
            TreatmentList = new List<Treatment>();
            AdviceList = new List<Advice>();
        }

    }
}