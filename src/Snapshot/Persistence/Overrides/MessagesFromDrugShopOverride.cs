using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;
using FluentNHibernate.Automapping.Alterations;

namespace Persistence.Overrides
{
    public class MessagesFromDrugShopOverride : IAutoMappingOverride<MessageFromDrugShop>
    {
        public void Override(FluentNHibernate.Automapping.AutoMapping<MessageFromDrugShop> mapping)
        {
            mapping.HasManyToMany(message => message.ServicesNeeded).Cascade.SaveUpdate();
            mapping.HasManyToMany(message => message.Appointments).Cascade.SaveUpdate();
            mapping.Map(p => p.IDCode).Unique();
        }
    }
}
