using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.OutpostManagement.Models.Outpost
{
    public class OutpostTypeModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
    }
}