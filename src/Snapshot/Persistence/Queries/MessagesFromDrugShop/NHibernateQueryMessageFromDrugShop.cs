using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Persistence;
using Domain;
using NHibernate.Linq;

namespace Persistence.Queries.MessagesFromDrugShop
{
    public class NHibernateQueryMessageFromDrugShop : IQueryMessageFromDrugShop
    {
        public IQueryService<MessageFromDrugShop> queryMessage;

        public NHibernateQueryMessageFromDrugShop(IQueryService<MessageFromDrugShop> query)
        {
            this.queryMessage = query;
        }
        public IQueryable<MessageFromDrugShop> GetServicesNeeded()
        {
            return queryMessage.Query().Fetch(it => it.ServicesNeeded);
        }
    }
}
