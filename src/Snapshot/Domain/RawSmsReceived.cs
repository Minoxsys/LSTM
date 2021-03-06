﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain;

namespace Domain
{
    public class RawSmsReceived : DomainEntity
    {
        public virtual DateTime ReceivedDate { get; set; }
        public virtual string Sender { get; set; }
        public virtual string Content { get; set; }
        
        public virtual Guid OutpostId { get; set; }
        public virtual int OutpostType { get; set; }
        public virtual bool ParseSucceeded { get; set; }
        public virtual string ParseErrorMessage { get; set; }
    }
}
