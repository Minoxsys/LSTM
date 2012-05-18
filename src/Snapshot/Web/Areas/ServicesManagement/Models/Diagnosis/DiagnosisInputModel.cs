using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Web.Areas.ServicesManagement.Models.Diagnosis
{
    public class DiagnosisInputModel
    {
        [Required(ErrorMessage = "Code for diagnosis is required")]
        public string Code { get; set; }
        public string Keyword { get; set; }
        public string Description { get; set; }
        public Guid Id { get; set; }
    }
}