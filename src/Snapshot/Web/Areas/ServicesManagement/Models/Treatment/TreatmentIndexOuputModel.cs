using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.ServicesManagement.Models.Treatment
{
    public class TreatmentIndexOuputModel
    {
        public TreatmentModel[] Treatments { get; set; }
        public int TotalItems { get; set; }
    }
}