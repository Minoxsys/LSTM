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
    public class ActivateThePhoneNumberMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void WhenWeWantTo_ActivateAPhoneNumber_ItShouldSaveTheActiveContact_AndDezactivateAllOtherPhoneNumbers()
        {
            //Arrange
            objectMother.queryServiceContact.Expect(call => call.Query()).Return(new Contact[] { objectMother.contact }.AsQueryable<Contact>());
            objectMother.queryOutposts.Expect(call => call.GetAllContacts()).Return(new Outpost[] { objectMother.outpost }.AsQueryable<Outpost>());

            objectMother.saveContact.Expect(call => call.Execute(Arg<Contact>.Matches(
                p => p.IsActive == true &&
                     p.ContactDetail == objectMother.contact.ContactDetail &&
                     p.ContactType == objectMother.contact.ContactType 
                     )));

            //Act
            objectMother.service.ActivateThePhoneNumber(objectMother.rawSmsReceivedForActivation);

            //Assert
            objectMother.queryServiceContact.VerifyAllExpectations();
            objectMother.queryOutposts.VerifyAllExpectations();
            objectMother.saveContact.VerifyAllExpectations();
        }
    }
}
