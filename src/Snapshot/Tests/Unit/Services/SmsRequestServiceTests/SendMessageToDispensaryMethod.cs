using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests.Unit.Services.SmsRequestServiceTests
{
    [TestFixture]
    public class SendMessageToDispensaryMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void WhenWeWantToSendMessageToDispensary_AndMessageIsSuccesfullySent_ItShouldReturn_True()
        {
            //Arrange
            objectMother.httpService.Expect(call => call.Post(Arg<string>.Is.Anything)).Return("");

            //Act
            var result = objectMother.service.SendMessage(ObjectMother.MESSAGE, objectMother.rawSms);

            //Assert
            objectMother.httpService.VerifyAllExpectations();
            Assert.IsInstanceOf<bool>(result);
            Assert.AreEqual(true, result);

        }
    }
}
