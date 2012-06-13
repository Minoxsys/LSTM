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
        public int Type { get; set; }
        public string Coordonates { get; set; }
    }
}