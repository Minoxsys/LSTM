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
        public void WhenWeWantToAssociateAnOutpostBy_WrondPhoneNumber_ToRawsSmS_ItReturnsRawsmsWithEmptyOutpostId()
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
            Assert.IsInstanceOf<RawSmsReceived>(result);
            Assert.AreNotEqual(null, result);
            Assert.AreEqual(Guid.Empty, result.OutpostId);
            Assert.AreEqual(0, result.OutpostType);
        }

        [Test]
        public void WhenWeWantToAssociateAnOutpostIdBy_ExistingPhoneNumber_ItReturnsARawSmsWithValidOutpostId()
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
            Assert.IsInstanceOf<RawSmsReceived>(result);
            Assert.AreEqual(objectMother.outpostId, result.OutpostId);
            Assert.AreEqual(objectMother.outpostType.Type, result.OutpostType);
        }
    }
}
