﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.ConditionManagement.Models.Advice
{
    public class AdviceIndexOuputModel
    {
        public AdviceModel[] Advices { get; set; }
        public int TotalItems { get; set; }
    }
}