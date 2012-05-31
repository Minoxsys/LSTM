using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;
using Core.Persistence;
using NHibernate.Linq;

namespace Persistence.Queries.MessagesFromDispensary
{
    public class NHibernateQueryMessageFromDispensary : IQueryMessageFromDispensary
    {
        public IQueryService<MessageFromDispensary> queryMessage;

        public NHibernateQueryMessageFromDispensary(IQueryService<MessageFromDispensary> query)
        {
            this.queryMessage = query;
        }

        public IQueryable<MessageFromDispensary> GetDiagnosises()
        {
            return queryMessage.Query().Fetch(it => it.Diagnosises);
        }

        public IQueryable<MessageFromDispensary> GetTreatments()
        {
            return queryMessage.Query().Fetch(it => it.Treatments);
        }

        public IQueryable<MessageFromDispensary> GetAdvices()
        {
            return queryMessage.Query().Fetch(it => it.Advices);
        }
    }
}
