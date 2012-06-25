using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain;

namespace Domain
{
    public class Treatment : DomainEntity
    {
        public virtual string Code { get; set; }
        public virtual string Keyword { get; set; }
        public virtual string Description { get; set; }
        public virtual Client Client { get; set; }

        public virtual IList<MessageFromDispensary> Messages { get; set; }

        public Treatment()
        {
            Messages = new List<MessageFromDispensary>();
        }

        public virtual void AddMessageFromDispensary(MessageFromDispensary message)
        {
            Messages.Add(message);
        }

        public override string ToString()
        {
            return Code;
        }
    }
}
