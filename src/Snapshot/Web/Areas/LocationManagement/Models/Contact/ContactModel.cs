using System;
using Web.Areas.LocationManagement.Models.Client;

namespace Web.Areas.LocationManagement.Models.Contact
{
    public class ContactModel
    {
        public Guid? Id { get; set; }
        public string ContactType { get; set; }
        public string ContactDetail { get; set; }
        public bool IsMainContact { get; set; }
        public Guid? OutpostId { get; set; }
    }
}