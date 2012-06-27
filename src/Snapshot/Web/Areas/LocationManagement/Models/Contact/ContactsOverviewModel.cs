
using System;
using System.Collections.Generic;

namespace Web.Areas.LocationManagement.Models.Contact
{
    public class ContactsOverviewModel
    {
        public Guid OutpostId { get; set; }
        public List<ContactModel> Items { get; set; }
        public string Error { get; set; }
    }
}