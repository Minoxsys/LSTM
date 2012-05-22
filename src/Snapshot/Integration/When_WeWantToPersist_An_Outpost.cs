using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Testing;
using NUnit.Framework;
using Core.Domain;
using Domain;
using Persistence;
using Persistence.Queries.Employees;
using Persistence.Conventions;
using NHibernate.Linq;

namespace IntegrationTests
{
    [TestFixture]

    class When_WeWantToPersist_An_Outpost : GivenAPersistenceSpecification<Outpost>
    {
        private const string OUTPOST_NAME = "Cluj";
        private const string OUTPOST_DETAILMETHOD = "EMAIL";
        private Country COUNTRY = new Country { Name = "Romania" };
        private Region REGION = new Region { Name = "TRANSILVANIA" };
        private District DISTRICT = new District { Name = "NORD" };
        private Outpost WAREHOUSE = new Outpost { Name = "BAIA MARE" };
        private OutpostType OUTPOSTTYPE = new OutpostType { Name = "DRUG SHOP" };

        [Test]
        public void It_Should_Successfully_Persist_AN_Outpost()
        {
            var outpost = Specs
                .CheckProperty(e => e.Name, OUTPOST_NAME)
                .CheckProperty(c => c.DetailMethod, OUTPOST_DETAILMETHOD)
                .CheckReference(c => c.Country, COUNTRY)
                .CheckReference(c => c.Region, REGION)
                .CheckReference(c => c.District, DISTRICT)
                .CheckReference(c => c.Warehouse, WAREHOUSE)
                .CheckReference(c => c.OutpostType, OUTPOSTTYPE)
                .CheckReference(c => c.Country, COUNTRY)
                .VerifyTheMappings();

            Assert.IsNotNull(outpost);
            Assert.IsInstanceOf<Guid>(outpost.Id);
            Assert.AreEqual(outpost.Name, OUTPOST_NAME);
            Assert.AreEqual(outpost.DetailMethod, OUTPOST_DETAILMETHOD);
            Assert.IsInstanceOf<Country>(outpost.Country);
            Assert.AreEqual(outpost.Country.Name, COUNTRY.Name);
            Assert.IsInstanceOf<Region>(outpost.Region);
            Assert.AreEqual(outpost.Region.Name, REGION.Name);
            Assert.IsInstanceOf<District>(outpost.District);
            Assert.AreEqual(outpost.District.Name, DISTRICT.Name);
            Assert.IsInstanceOf<Outpost>(outpost.Warehouse);
            Assert.AreEqual(outpost.Warehouse.Name, WAREHOUSE.Name);
            Assert.IsInstanceOf<OutpostType>(outpost.OutpostType);
            Assert.AreEqual(outpost.OutpostType.Name, OUTPOSTTYPE.Name);

            session.Delete(outpost);
            session.Flush();
        }
    }
}
