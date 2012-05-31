using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;
using NUnit.Framework;
using FluentNHibernate.Testing;

namespace IntegrationTests
{
    public class When_WeWantToPersis_A_MessageFromDrugShop : GivenAPersistenceSpecification<MessageFromDrugShop>
    {
        [Test]
        public void It_ShouldBeAbleTo_Save_ThenLoad_ThenDelete_A_MesageFromDrugShop()
        {
            const string INITIALS = "xy";
            DateTime BIRTHDATE = DateTime.UtcNow;
            const string GENDER = "F";
            Guid OUTPOSTID = Guid.NewGuid();
            DateTime SENTDATE = DateTime.UtcNow.AddDays(-1);
            const string IDCODE = "123456";
            ServiceNeeded SERVICENEEDED = new ServiceNeeded() { Keyword = "Hiv", Code = "D3" };

            var message = Specs
                .CheckProperty(p => p.Initials, INITIALS)
                .CheckProperty(p => p.Gender, GENDER)
                .CheckProperty(p => p.OutpostId, OUTPOSTID)
                .CheckProperty(p => p.IDCode, IDCODE)
                .CheckList(p => p.ServicesNeeded, new List<ServiceNeeded> { SERVICENEEDED })
                .VerifyTheMappings();

            Assert.IsNotNull(message);
            Assert.IsInstanceOf<Guid>(message.Id);
            Assert.AreNotEqual(message.Id, Guid.Empty);

            Assert.AreEqual(INITIALS, message.Initials);
            Assert.AreEqual(GENDER, message.Gender);
            Assert.AreEqual(OUTPOSTID, message.OutpostId);
            Assert.AreEqual(IDCODE, message.IDCode);

            Assert.IsNotNull(message.ServicesNeeded);
            Assert.AreEqual(1, message.ServicesNeeded.Count);

            session.Delete(message);
            session.Flush();

        }
    }
}
