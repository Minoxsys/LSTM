﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.ServicesManagement.Models.ServiceNeeded
{
    public class ServiceNeededIndexOuputModel
    {
        public ServiceNeededModel[] ServiceNeeded { get; set; }
        public int TotalItems { get; set; }
    }
}