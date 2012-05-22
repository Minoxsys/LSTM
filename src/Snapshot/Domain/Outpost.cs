using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain;

namespace Domain
{
    public class Outpost : DomainEntity
    {
        public virtual string Name { get; set; }
        public virtual string DetailMethod { get; set; }
        public virtual string Longitude { get; set; }
        public virtual string Latitude { get; set; }
        public virtual Outpost Warehouse { get; set; }
        public virtual IList<Domain.Contact> Contacts { get; set; }
        public virtual OutpostType OutpostType { get; set; }
        public virtual Country Country { get; set; }
        public virtual Region Region { get; set; }
        public virtual District District { get; set; }
        public virtual Client Client { get; set; }


        public Outpost()
        {
            Contacts = new List<Contact>();
        }

        public virtual void AddContact(Contact contact)
        {
            Contacts.Add(contact);
        }


    }
}
