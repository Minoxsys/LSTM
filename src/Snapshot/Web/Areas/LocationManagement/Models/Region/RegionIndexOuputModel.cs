﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.LocationManagement.Models.Region
{
    public class RegionIndexOuputModel
    {
        public RegionModel[] Regions { get; set; }
        public int TotalItems { get; set; }
    }
}