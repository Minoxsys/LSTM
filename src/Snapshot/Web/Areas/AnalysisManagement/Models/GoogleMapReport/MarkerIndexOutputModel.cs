﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.AnalysisManagement.Models.GoogleMapReport
{
    public class MarkerIndexOutputModel
    {
        public MarkerModel[] Markers { get; set; }
        public int TotalItems { get; set; }
    }
}