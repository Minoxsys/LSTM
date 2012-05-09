using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;
using NUnit.Framework;
using FluentNHibernate.Testing;

namespace IntegrationTests
{
    public class When_WeWantToPersist_A_Diagnosis : GivenAPersistenceSpecification<Diagnosis>
    {
        const string DIAGNOSIS_NAME = "Itching";
        const string DIAGNOSIS_CODEDS = "Symp-1";
        const string DIAGNOSIS_DISPLAY = "CG+; CG-";

        [Test]
        public void It_ShouldSuccessfullyPersist_A_Country()
        {

            var diagnosis = Specs.CheckProperty(e => e.Name, DIAGNOSIS_NAME)
                .CheckProperty(c => c.CodeDS, DIAGNOSIS_CODEDS)
                .CheckProperty(c => c.Display, DIAGNOSIS_DISPLAY)
                .VerifyTheMappings();

            Assert.IsNotNull(diagnosis);
            Assert.IsInstanceOf<Guid>(diagnosis.Id);
            Assert.AreEqual(diagnosis.Name, DIAGNOSIS_NAME);
            Assert.AreEqual(diagnosis.CodeDS, DIAGNOSIS_CODEDS);
            Assert.AreEqual(diagnosis.Display, DIAGNOSIS_DISPLAY);

            session.Delete(diagnosis);
            session.Flush();


        }
    }
}
