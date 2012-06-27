using System;
using Web.Areas.LocationManagement.Models.Client;

namespace Web.Areas.LocationManagement.Models.Country
{
    public class CountryOutputModel
    {
        public string Name { get; set; }
        public string ISOCode { get; set; }
        public string PhonePrefix { get; set; }
        public ClientModel Client { get; set; }
        public Guid Id { get; set; }
    }
}