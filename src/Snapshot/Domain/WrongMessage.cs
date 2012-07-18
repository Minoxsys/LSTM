using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain;

namespace Domain
{
    public class WrongMessage : DomainEntity
    {
        public virtual string PhoneNumber { get; set; }
        public virtual int NoOfWrongMessages { get; set; }
    }
}
