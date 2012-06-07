using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain;

namespace Domain
{
    public class OutpostType : DomainEntity
    {
        public virtual string Name { get; set; }

        // 0 - Drug shop
        // 1 - Dispensary
        // 2 - Health Center
        public virtual int Type { get; set; } 
    }
}
