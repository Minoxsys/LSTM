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
            objectMother.saveOrUpdateCommand.Expect(call => call.Execute(Arg<SentSms>.Matches(p => p.Message == ObjectMother.MESSAGE && p.PhoneNumber == "+" + objectMother.rawSms.Sender.Trim('+'))));

            //Act
            var result = objectMother.service.SendMessage(ObjectMother.MESSAGE, objectMother.rawSms);

            //Assert
            objectMother.httpService.VerifyAllExpectations();
            objectMother.saveOrUpdateCommand.VerifyAllExpectations();
            Assert.IsInstanceOf<bool>(result);
            Assert.AreEqual(true, result);

        }
    }
}
