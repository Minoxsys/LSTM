using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.AnalysisManagement.Models.GoogleMapReport
{
    public class MarkerModel
    {
        public string Name { get; set; }
        public string Number { get; set; }
        public string Type { get; set; }
        public string Coordonates { get; set; }
        public string FemaleYounger { get; set; }
        public string FemaleOlder { get; set; }
        public string MaleYounger { get; set; }
        public string MaleOlder { get; set; }
    }
}