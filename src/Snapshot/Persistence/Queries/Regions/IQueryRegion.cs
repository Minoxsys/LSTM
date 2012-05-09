using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;

namespace Persistence.Queries.Countries
{
    public interface IQueryRegion
    {
        IQueryable<Region> GetAll();
    }
}
