using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Web.Areas.ServicesManagement.Models.Treatment
{
    public class TreatmentInputModel
    {
        [Required(ErrorMessage = "Code for treatment is required")]
        public string Code { get; set; }
        public string Advice { get; set; }
        public string Description { get; set; }
        public Guid Id { get; set; }
    }
}