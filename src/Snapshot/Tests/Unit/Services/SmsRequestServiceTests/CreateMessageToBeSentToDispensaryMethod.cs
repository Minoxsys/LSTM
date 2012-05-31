using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests.Unit.Services.SmsRequestServiceTests
{
    [TestFixture]
    public class CreateMessageToBeSentToDispensaryMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void WhenWeWantToCreateMessageForDispensary_ItShouldReturnTheCorrectMessage()
        {
            //Arrange
            objectMother.queryOutposts.Expect(call => call.GetOutpostName(objectMother.outpostId)).Return(objectMother.outpost.Name);

            //Act
            var result = objectMother.service.CreateMessageToBeSentToDispensary(objectMother.messageFromDrugShop);

            //Assert
            objectMother.queryOutposts.VerifyAllExpectations();
            Assert.IsInstanceOf<string>(result);
            Assert.AreEqual(ObjectMother.CORRECTMESSAGE, result);
        }
    }
}
