﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.OutpostManagement.Models.Outpost
{
    public class OutpostTypesIndexOutputModel
    {
        public OutpostTypeModel[] OutpostTypes { get; set; }
        public int TotalItems { get; set; }
    }
}