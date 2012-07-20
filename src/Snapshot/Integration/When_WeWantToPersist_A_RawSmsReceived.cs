using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;
using NUnit.Framework;
using FluentNHibernate.Testing;

namespace IntegrationTests
{
    public class When_WeWantToPersist_A_RawSmsReceived : GivenAPersistenceSpecification<RawSmsReceived>
    {
        const string SENDER = "0747888888";
        const string CONTENT = "XU&76687676ghfhfhgRTDFC";
        const string CREDITS = "10";
        DateTime DATE = DateTime.UtcNow;
        Guid OUTPOSTOID = Guid.NewGuid();
        const bool PARSESUCCEEDED = false;
        const string PARSEERRORMESSAGE = "Some error message";

        [Test]
        public void It_ShouldSuccessfullyPersist_A_RawSmsReceived()
        {

            var rawsms = Specs.CheckProperty(e => e.Sender, SENDER)
                .CheckProperty(c => c.Content, CONTENT)
                .CheckProperty(c => c.OutpostId, OUTPOSTOID)
                .CheckProperty(c => c.ParseSucceeded, PARSESUCCEEDED)
                .CheckProperty(c => c.ParseErrorMessage, PARSEERRORMESSAGE)
                .VerifyTheMappings();

            Assert.IsNotNull(rawsms);
            Assert.IsInstanceOf<Guid>(rawsms.Id);
            Assert.AreEqual(rawsms.Sender, SENDER);
            Assert.AreEqual(rawsms.Content, CONTENT);
            Assert.AreEqual(rawsms.OutpostId, OUTPOSTOID);
            Assert.AreEqual(rawsms.ParseSucceeded, PARSESUCCEEDED);
            Assert.AreEqual(rawsms.ParseErrorMessage, PARSEERRORMESSAGE);

            session.Delete(rawsms);
            session.Flush();
        }
    }
}
