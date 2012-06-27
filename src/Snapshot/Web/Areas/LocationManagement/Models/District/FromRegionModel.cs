using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.LocationManagement.Models.District
{
    public class FromRegionModel
    {
        public Guid CountryId { get; set; }
        public Guid RegionId { get; set; }
    }
}