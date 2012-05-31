using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain;

namespace Domain
{
    public class MessageFromDrugShop : DomainEntity
    {
        public virtual string Initials { get; set; }
        public virtual DateTime BirthDate { get; set; }
        public virtual string Gender { get; set; }
        public virtual Guid OutpostId { get; set; }
        public virtual DateTime SentDate { get; set; }
        public virtual string IDCode { get; set; }
        public virtual IList<ServiceNeeded> ServicesNeeded { get; set; }

        public MessageFromDrugShop()
        {
            ServicesNeeded = new List<ServiceNeeded>();
        }

        public virtual void AddServiceNeeded(ServiceNeeded service)
        {
            ServicesNeeded.Add(service);
        }

        public virtual void RemoveServiceNeeded(ServiceNeeded service)
        {
            service.Messages.Remove(this);
            this.ServicesNeeded.Remove(service);
        }

    }
}
