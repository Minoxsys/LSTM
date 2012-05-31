using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Domain;
using Rhino.Mocks;

namespace Tests.Unit.Services.ManageReceivedSmsTests
{
    [TestFixture]
    public class AssignOutpostToRawSmsReceivedBySenderNumberMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void WhenWeWantToGetAnOutpostIdBy_WrondPhoneNumber_ItReturnsAnEmptyGuid()
        {
            //Arrange
            objectMother.queryServiceContact.Expect(call => call.Query()).Return(new Contact[] { objectMother.contact }.AsQueryable<Contact>());
            objectMother.queryOutposts.Expect(call => call.GetAllContacts()).Return(new Outpost[] { objectMother.outpost }.AsQueryable<Outpost>());
            RawSmsReceived smsReceived = new RawSmsReceived { Sender = ObjectMother.WRONG_MOBILE_NUMBER };

            //Act
            var result = objectMother.service.AssignOutpostToRawSmsReceivedBySenderNumber(smsReceived);

            //Assert
            objectMother.queryServiceContact.VerifyAllExpectations();
            objectMother.queryOutposts.VerifyAllExpectations();
            Assert.IsInstanceOf<Guid>(result);
            Assert.AreEqual(Guid.Empty, result);
        }

        [Test]
        public void WhenWeWantToGetAnOutpostIdBy_ExistingPhoneNumber_ItReturnsAValidOutpostId()
        {
            //Arrange
            objectMother.queryServiceContact.Expect(call => call.Query()).Return(new Contact[] { objectMother.contact }.AsQueryable<Contact>());
            objectMother.queryOutposts.Expect(call => call.GetAllContacts()).Return(new Outpost[] { objectMother.outpost }.AsQueryable<Outpost>());
            RawSmsReceived smsReceived = new RawSmsReceived { Sender = ObjectMother.MOBILE_NUMBER };

            //Act
            var result = objectMother.service.AssignOutpostToRawSmsReceivedBySenderNumber(smsReceived);

            //Assert
            objectMother.queryServiceContact.VerifyAllExpectations();
            objectMother.queryOutposts.VerifyAllExpectations();
            Assert.IsInstanceOf<Guid>(result);
            Assert.AreEqual(objectMother.outpostId, result);
        }
    }
}
