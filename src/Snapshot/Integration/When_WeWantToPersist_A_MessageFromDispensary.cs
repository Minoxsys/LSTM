using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;
using NUnit.Framework;
using FluentNHibernate.Testing;

namespace IntegrationTests
{
    public class When_WeWantToPersist_A_MessageFromDispensary : GivenAPersistenceSpecification<MessageFromDispensary>
    {
        [Test]
        public void It_ShouldBeAbleTo_Save_ThenLoad_ThenDelete_A_MessageFromDispensary()
        {
            MessageFromDrugShop MESSAGE = new MessageFromDrugShop { Initials = "XY", BirthDate = DateTime.UtcNow, Gender = "F", IDCode = "123456", OutpostId = Guid.NewGuid(), SentDate = DateTime.UtcNow};
            Guid OUTPOSTID = Guid.NewGuid();
            DateTime SENTDATE = DateTime.UtcNow.AddDays(-1);
            Diagnosis DIAGNOSIS = new Diagnosis { Code = "D1" };
            Treatment TREATMENT = new Treatment { Code = "T1" };
            Advice ADVICE = new Advice { Code = "A1" };

            var message = Specs
                .CheckReference(p => p.MessageFromDrugShop, MESSAGE)
                .CheckProperty(p => p.OutpostId, OUTPOSTID)
                .CheckList(p => p.Diagnosises, new List<Diagnosis> { DIAGNOSIS })
                .CheckList(p => p.Treatments, new List<Treatment> { TREATMENT })
                .CheckList(p => p.Advices, new List<Advice> { ADVICE })
                .VerifyTheMappings();

            Assert.IsNotNull(message);
            Assert.IsInstanceOf<Guid>(message.Id);
            Assert.AreNotEqual(message.Id, Guid.Empty);

            Assert.IsInstanceOf<MessageFromDrugShop>(message.MessageFromDrugShop);
            Assert.AreEqual(message.MessageFromDrugShop.Initials, MESSAGE.Initials);

            Assert.AreEqual(OUTPOSTID, message.OutpostId);

            Assert.IsNotNull(message.Diagnosises);
            Assert.AreEqual(1, message.Diagnosises.Count);
            Assert.IsNotNull(message.Treatments);
            Assert.AreEqual(1, message.Treatments.Count);
            Assert.IsNotNull(message.Advices);
            Assert.AreEqual(1, message.Advices.Count);

            session.Delete(message);
            session.Flush();

        }
    }
}
