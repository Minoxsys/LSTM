﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.ServicesManagement.Models.Treatment
{
    public class TreatmentModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Advice { get; set; }
        public string Description { get; set; }
    }
}