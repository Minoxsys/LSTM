﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.AnalysisManagement.Models.PatientsReport
{
    public class ServiceIndexOutputModel
    {
        public ServiceModel[] Service { get; set; }
        public int TotalItems { get; set; }
    }
}