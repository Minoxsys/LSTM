using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.ConditionManagement.Models.Condition
{
    public class ServiceNeededModel
    {
        public string Code { get; set; }
        public string Keyword { get; set; }
        public string Description { get; set; }
        public Guid Id { get; set; }
    }
}