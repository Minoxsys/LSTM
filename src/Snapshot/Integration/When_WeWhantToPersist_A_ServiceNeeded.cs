using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;
using NUnit.Framework;
using FluentNHibernate.Testing;

namespace IntegrationTests
{
    public class When_WeWhantToPersist_A_ServiceNeeded : GivenAPersistenceSpecification<ServiceNeeded>
    {
        const string SERVICENEEDED_CODE = "Symp-1";
        const string SERVICENEEDED_KEYWORD = "Itching";
        const string SERVICENEEDED_DESCRIPTION = "Itching genital areas";

        [Test]
        public void It_ShouldSuccessfullyPersist_A_ServiceNeeded()
        {

            var diagnosis = Specs.CheckProperty(e => e.Description, SERVICENEEDED_DESCRIPTION)
                .CheckProperty(c => c.Code, SERVICENEEDED_CODE)
                .CheckProperty(c => c.Keyword, SERVICENEEDED_KEYWORD)
                .VerifyTheMappings();

            Assert.IsNotNull(diagnosis);
            Assert.IsInstanceOf<Guid>(diagnosis.Id);
            Assert.AreEqual(diagnosis.Description, SERVICENEEDED_DESCRIPTION);
            Assert.AreEqual(diagnosis.Code, SERVICENEEDED_CODE);
            Assert.AreEqual(diagnosis.Keyword, SERVICENEEDED_KEYWORD);

            session.Delete(diagnosis);
            session.Flush();


        }
    }
}
