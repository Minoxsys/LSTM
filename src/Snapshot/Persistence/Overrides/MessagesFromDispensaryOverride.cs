using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping.Alterations;
using Domain;

namespace Persistence.Overrides
{
    public class MessagesFromDispensaryOverride : IAutoMappingOverride<MessageFromDispensary>
    {
        public void Override(FluentNHibernate.Automapping.AutoMapping<MessageFromDispensary> mapping)
        {
            mapping.HasManyToMany(message => message.Diagnosises).Cascade.SaveUpdate();
            mapping.HasManyToMany(message => message.Treatments).Cascade.SaveUpdate();
            mapping.HasManyToMany(message => message.Advices).Cascade.SaveUpdate();
        }
    }
}
