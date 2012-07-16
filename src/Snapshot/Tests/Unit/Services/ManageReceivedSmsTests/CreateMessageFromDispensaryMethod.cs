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
    public class CreateMessageFromDispensaryMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void WhenWeWantToCreateMessageFormDispensary_ItShouldParseTheRawSmSAndReturnTheCorrectMessageFromDispensary()
        {
            //Arange
            var diagnosis = objectMother.ListOfDiagnosis();
            var treatments = objectMother.ListOfTreatments();
            var advices = objectMother.ListOfAdvices();
            var messages = objectMother.ListOfMessagesFromDrugSho();
            objectMother.queryAdvice.Expect(call => call.Query()).Return(advices);
            objectMother.queryDiagnosis.Expect(call => call.Query()).Return(diagnosis);
            objectMother.queryTreatment.Expect(call => call.Query()).Return(treatments);
            objectMother.queryMessageFromDrugShop.Expect(call => call.Query()).Return(messages);

            //Act
            var result = objectMother.service.CreateMessageFromDispensary(objectMother.rawSmsReceivedFromDispensary);

            //Assert
            objectMother.queryAdvice.VerifyAllExpectations();
            objectMother.queryDiagnosis.VerifyAllExpectations();
            objectMother.queryTreatment.VerifyAllExpectations();

            Assert.IsInstanceOf<MessageFromDispensary>(result);
            Assert.AreEqual(2, result.Diagnosises.Count);
            Assert.AreEqual(2, result.Treatments.Count);
            Assert.AreEqual(1, result.Advices.Count);
        }
    }
}
