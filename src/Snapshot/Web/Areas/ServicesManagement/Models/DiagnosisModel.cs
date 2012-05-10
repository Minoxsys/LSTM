using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.ServicesManagement.Models
{
    public class DiagnosisModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string ServiceNeeded { get; set; }
        public string Description { get; set; }

    }
}