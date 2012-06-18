using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Domain;

namespace Tests.Unit.Services.SmsRequestServiceTests
{
    [TestFixture]
    public class SendMessageMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void WhenWeWantToSendMessage_AndMessageIsSuccesfullySent_ItShouldReturn_True()
        {
            //Arrange
            objectMother.smsGatewayService.Expect(call => call.SendSmsRequest(Arg<string>.Is.Anything)).Return("");

            //Act
            var result = objectMother.service.SendMessage(ObjectMother.MESSAGE, objectMother.rawSms);

            //Assert
            objectMother.smsGatewayService.VerifyAllExpectations();
            Assert.IsInstanceOf<bool>(result);
            Assert.AreEqual(true, result);

        }
    }
}
