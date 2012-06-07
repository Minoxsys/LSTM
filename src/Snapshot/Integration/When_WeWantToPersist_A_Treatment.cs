using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;
using NUnit.Framework;
using FluentNHibernate.Testing;

namespace IntegrationTests
{
    public class When_WeWantToPersist_A_Treatment : GivenAPersistenceSpecification<Treatment>
    {
        const string TREATMENT_CODE = "RX1";
        const string TREATMENT_KEYWORD = "Clamidya";
        const string TREATMENT_DESCRIPTION = "Itching";

        [Test]
        public void It_ShouldSuccessfullyPersist_A_Treatment()
        {

            var treatment = Specs.CheckProperty(e => e.Description, TREATMENT_DESCRIPTION)
                .CheckProperty(c => c.Code, TREATMENT_CODE)
                .CheckProperty(c => c.Keyword, TREATMENT_KEYWORD)
                .VerifyTheMappings();

            Assert.IsNotNull(treatment);
            Assert.IsInstanceOf<Guid>(treatment.Id);
            Assert.AreEqual(treatment.Description, TREATMENT_DESCRIPTION);
            Assert.AreEqual(treatment.Code, TREATMENT_CODE);
            Assert.AreEqual(treatment.Keyword, TREATMENT_KEYWORD);

            session.Delete(treatment);
            session.Flush();


        }
    }
}
