﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.ServicesManagement.Models.Diagnosis
{
    public class DiagnosisIndexOuputModel
    {
        public DiagnosisModel[] Diagnosis { get; set; }
        public int TotalItems { get; set; }
    }
}