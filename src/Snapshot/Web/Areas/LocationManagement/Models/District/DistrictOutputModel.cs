using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Areas.LocationManagement.Models.Region;
using System.Web.Mvc;
using Core.Persistence;
using Web.Areas.LocationManagement.Models.Client;

namespace Web.Areas.LocationManagement.Models.District
{
    public class DistrictOutputModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid ClientId { get; set; }
        public Guid RegionId { get; set; }
       
        public DistrictOutputModel() { }
       
    }
}