﻿using System;
using System.Collections.Generic;
using Web.Areas.LocationManagement.Models.Region;
using Web.Areas.LocationManagement.Models.District;
using Web.Areas.LocationManagement.Models.Country;
using Web.Areas.LocationManagement.Models.Client;
using Web.Areas.LocationManagement.Models.Contact;

namespace Web.Areas.LocationManagement.Models.Outpost
{
    public class OutpostModel
    {
        public string Name { get; set; }
        public string OutpostType { get; set; }
        public string DetailMethod { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public RegionModel Region { get; set; }
        public DistrictModel District { get; set; }
        public ClientModel Client { get; set; }
        public ContactModel Contact { get; set; }
        public OutpostModel Warehouse { get; set; }
        public List<OutpostModel> Outpost { get; set; }
        public List<Domain.Contact> Contacts { get; set; }
        public int ProductsNo { get; set; }
        public Guid Id { get; set; }
   }
}