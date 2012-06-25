using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain;

namespace Domain
{
    public class ServiceNeeded : DomainEntity
    {
        public virtual string Keyword { get; set; }
        public virtual string Code { get; set; }
        public virtual string Description { get; set; }
        public virtual Client Client { get; set; }

        public virtual IList<MessageFromDrugShop> Messages { get; set; }

        public ServiceNeeded()
        {
            Messages = new List<MessageFromDrugShop>();
        }

        public virtual void AddMessageFromDrugShop(MessageFromDrugShop message)
        {
            Messages.Add(message);
        }

        public override string ToString()
        {
            return Code;
        }

    }
}
