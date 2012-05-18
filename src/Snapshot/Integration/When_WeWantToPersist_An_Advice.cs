using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;
using NUnit.Framework;
using FluentNHibernate.Testing;

namespace IntegrationTests
{
    public class When_WeWantToPersist_An_Advice : GivenAPersistenceSpecification<Advice>
    {
        const string ADVICE_CODE = "A1";
        const string ADVICE_KEYWORD = "Treated";
        const string ADVICE_DESCRIPTION = "Patient treated and sent home";

        [Test]
        public void It_ShouldSuccessfullyPersist_A_Diagnosis()
        {

            var advice = Specs.CheckProperty(e => e.Description, ADVICE_DESCRIPTION)
                .CheckProperty(c => c.Code, ADVICE_CODE)
                .CheckProperty(c => c.Keyword, ADVICE_KEYWORD)
                .VerifyTheMappings();

            Assert.IsNotNull(advice);
            Assert.IsInstanceOf<Guid>(advice.Id);
            Assert.AreEqual(advice.Description, ADVICE_DESCRIPTION);
            Assert.AreEqual(advice.Code, ADVICE_CODE);
            Assert.AreEqual(advice.Keyword, ADVICE_KEYWORD);

            session.Delete(advice);
        }
    }
}
