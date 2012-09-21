namespace Web.Areas.AnalysisManagement.Models.SymptomsReport
{
    public class SymptomsReportModel
    {
        public string Symptom { get; set; }
        public string Outpost { get; set; }
        public int Female { get; set; }
        public int Male { get; set; }
        public int NumberOfPatients { get; set; }
    }
}