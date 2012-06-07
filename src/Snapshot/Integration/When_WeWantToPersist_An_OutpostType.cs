using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;
using NUnit.Framework;
using FluentNHibernate.Testing;

namespace IntegrationTests
{
    public class When_WeWantToPersist_An_OutpostType : GivenAPersistenceSpecification<OutpostType>
    {
        const string NAME = "Drug shop";
        const int TYPE = 0;


        [Test]
        public void It_ShouldSuccessfullyPersist_An_OutpostType()
        {

            var outpostType = Specs.CheckProperty(e => e.Name, NAME)
                .CheckProperty(c => c.Type, TYPE)
                .VerifyTheMappings();

            Assert.IsNotNull(outpostType);
            Assert.IsInstanceOf<Guid>(outpostType.Id);
            Assert.AreEqual(outpostType.Name, NAME);
            Assert.AreEqual(outpostType.Type, TYPE);

            session.Delete(outpostType);
            session.Flush();


        }
    }
}
