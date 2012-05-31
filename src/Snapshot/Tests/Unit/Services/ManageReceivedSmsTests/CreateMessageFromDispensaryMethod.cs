using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Domain;

namespace Tests.Unit.Services.ManageReceivedSmsTests
{
    [TestFixture]
    public class CreateMessageFromDispensaryMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void ItShouldCreateAMessageFromDispensary_FromAGivenRawSmsReceived()
        {
            //Arrange
            var rawSms = objectMother.rawSmsReceived;
            rawSms.Content = ObjectMother.CORRECTMESSAGEFROMDISPENSARY;
            var diagnosis = objectMother.ListOfDiagnosis();
            var treatments = objectMother.ListOfTreatments();
            var advices = objectMother.ListOfAdvices();

            objectMother.queryMessageFromDrugShop.Expect(call => call.Query()).Return(new MessageFromDrugShop[] {objectMother.messageFromDrugShop}.AsQueryable());
            objectMother.queryDiagnosis.Expect(call => call.Query()).Return(diagnosis);
            objectMother.queryTreatment.Expect(call => call.Query()).Return(treatments);
            objectMother.queryAdvice.Expect(call => call.Query()).Return(advices);

            //Act
            var result = objectMother.service.CreateMessageFromDispensary(rawSms);

            //Assert
            objectMother.messageFromDrugShop.VerifyAllExpectations();
            objectMother.queryDiagnosis.VerifyAllExpectations();
            objectMother.queryTreatment.VerifyAllExpectations();
            objectMother.queryAdvice.VerifyAllExpectations();

            Assert.IsInstanceOf<MessageFromDispensary>(result);
            Assert.AreEqual(1, result.Advices.Count());
            Assert.AreEqual(2, result.Diagnosises.Count());
            Assert.AreEqual(2, result.Treatments.Count());
            Assert.IsInstanceOf<MessageFromDrugShop>(result.MessageFromDrugShop);
            Assert.AreNotEqual(null, result.MessageFromDrugShop); 
            Assert.AreEqual(objectMother.outpostId, result.OutpostId);
            Assert.AreEqual(objectMother.rawSmsReceived.ReceivedDate, result.SentDate);

        }
    }
}
