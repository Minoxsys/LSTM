using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Web.Models.Shared;
using Domain;
using Rhino.Mocks;
using Web.Areas.ServicesManagement.Models.Diagnosis;

namespace Tests.Unit.Controllers.Areas.ServicesManagement.DiagnosisControllerTests
{
    [TestFixture]
    public class CreateMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void Returns_JSON_With_ErrorMessage_When_Diagnosis_Code_IsNull()
        {
            //Arrange

            //Act
            var jsonResult = objectMother.controller.Create(new DiagnosisInputModel());

            //Assert
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
            Assert.That(response.Message, Is.EqualTo("The diagnosis has not been saved!"));
        }

        [Test]
        public void Returns_JSON_With_ErrorMessage_When_Diagnosis_Code_AND_ServiceNeeded_AlreadyExists()
        {
            //Arrange
            DiagnosisInputModel diagnosisInputModel = new DiagnosisInputModel()
            {
                ServiceNeeded = objectMother.diagnosis.ServiceNeeded,
                Code = objectMother.diagnosis.Code,
                Description = "new description"
            };
            objectMother.queryDiagnosis.Expect(call => call.Query()).Return(new Diagnosis[] { objectMother.diagnosis }.AsQueryable());

            //Act
            var jsonResult = objectMother.controller.Create(diagnosisInputModel);

            //Assert
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
        }

        [Test]
        public void Returns_JSON_With_SuccessMessage_When_Diagnosis_Has_Been_Saved()
        {
            //Arrange
            DiagnosisInputModel diagnosisInputModel = new DiagnosisInputModel()
            {
                ServiceNeeded = "Genital Warts test/Rx - STI",
                Code = "WART+; WART-",
                Description = "Bump genital"
            };
            objectMother.queryDiagnosis.Expect(call => call.Query()).Return(new Diagnosis[] { objectMother.diagnosis }.AsQueryable());
            objectMother.saveCommand.Expect(call => call.Execute(Arg<Diagnosis>.Matches(p => p.Code == diagnosisInputModel.Code)));

            //Act
            var jsonResult = objectMother.controller.Create(diagnosisInputModel);

            //Assert
            objectMother.queryDiagnosis.VerifyAllExpectations();
            objectMother.saveCommand.VerifyAllExpectations();

            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("Diagnosis WART+; WART- has been saved."));
        }
    }
}
