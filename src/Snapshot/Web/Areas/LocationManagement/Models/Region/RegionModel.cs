using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Areas.LocationManagement.Models.Country;
using Web.Areas.LocationManagement.Models.Client;

namespace Web.Areas.LocationManagement.Models.Region
{
    public class RegionModel
    {
        public string Name { get; set; }
        public string Coordinates { get; set; }
        public Guid CountryId { get; set; }
        public ClientModel Client { get; set; }
        public int DistrictNo { get; set; }
        public Guid Id { get; set; }
    }
}