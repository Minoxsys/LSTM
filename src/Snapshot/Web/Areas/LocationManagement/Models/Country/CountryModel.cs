using System;
using Web.Areas.LocationManagement.Models.Client;

namespace Web.Areas.LocationManagement.Models.Country
{
    public class CountryModel
    {
        public string Name { get; set; }
        public string ISOCode { get; set; }
        public string PhonePrefix { get; set; }
        public ClientModel Client { get; set; }
        public int RegionNo { get; set; }
        public Guid Id { get; set; }
    }
}