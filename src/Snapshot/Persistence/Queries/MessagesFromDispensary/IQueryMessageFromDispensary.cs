using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;

namespace Persistence.Queries.MessagesFromDispensary
{
    public interface IQueryMessageFromDispensary
    {
        IQueryable<MessageFromDispensary> GetDiagnosises();
        IQueryable<MessageFromDispensary> GetTreatments();
        IQueryable<MessageFromDispensary> GetAdvices();
    }
}
