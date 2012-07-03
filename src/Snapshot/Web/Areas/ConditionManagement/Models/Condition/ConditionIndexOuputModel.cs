using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.ConditionManagement.Models.Condition
{
    public class ConditionIndexOuputModel
    {
        public ConditionModel[] Condition { get; set; }
        public int TotalItems { get; set; }
    }
}