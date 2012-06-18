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
        public void WhenWeWantToSendMessageToDispensary_ButDispensaryHasNoPhoneNumber_ItShouldReturnFalse()
        {
            //Arange
            objectMother.queryOutposts.Expect(call => call.GetWarehouse(objectMother.outpostId)).Return(null);

            //Act
            var result = objectMother.service.SendMessageToDispensary(objectMother.messageFromDrugShop, objectMother.rawSms);

            //Assert
            objectMother.queryOutposts.VerifyAllExpectations();
            Assert.IsInstanceOf<bool>(result);
            Assert.AreEqual(false, result);

        }

        [Test]
        public void WhenWeWantToSendMessageToDispensary_AndMessageIsSent_ItShouldReturnTrue()
        {
            //Arange
            objectMother.queryOutposts.Expect(call => call.GetWarehouse(objectMother.outpostId)).Return(objectMother.warehouse);
            objectMother.smsGatewayService.Expect(call => call.SendSmsRequest(Arg<string>.Is.Anything)).Return("");
            objectMother.queryContact.Expect(call => call.Query()).Return(new Contact[] { objectMother.contact }.AsQueryable());

            //Act
            var result = objectMother.service.SendMessageToDispensary(objectMother.messageFromDrugShop, objectMother.rawSms);

            //Assert
            objectMother.queryOutposts.VerifyAllExpectations();
            objectMother.smsGatewayService.VerifyAllExpectations();
            objectMother.queryContact.VerifyAllExpectations();

            Assert.IsInstanceOf<bool>(result);
            Assert.AreEqual(true, result);
        }
    }
}
