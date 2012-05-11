using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;
using NUnit.Framework;
using FluentNHibernate.Testing;

namespace IntegrationTests
{
    public class When_WeWantToPersisi_A_Treatment : GivenAPersistenceSpecification<Treatment>
    {
        const string TREATMENT_CODE = "RX1; RX2; NOT/AV.";
        const string TREATMENT_ADVICE = "A1; A2; A3; A4";
        const string TREATMENT_DESCRIPTION = "Itching";

        [Test]
        public void It_ShouldSuccessfullyPersist_A_Treatment()
        {

            var treatment = Specs.CheckProperty(e => e.Description, TREATMENT_DESCRIPTION)
                .CheckProperty(c => c.Code, TREATMENT_CODE)
                .CheckProperty(c => c.Advice, TREATMENT_ADVICE)
                .VerifyTheMappings();

            Assert.IsNotNull(treatment);
            Assert.IsInstanceOf<Guid>(treatment.Id);
            Assert.AreEqual(treatment.Description, TREATMENT_DESCRIPTION);
            Assert.AreEqual(treatment.Code, TREATMENT_CODE);
            Assert.AreEqual(treatment.Advice, TREATMENT_ADVICE);

            session.Delete(treatment);
            session.Flush();


        }
    }
}
