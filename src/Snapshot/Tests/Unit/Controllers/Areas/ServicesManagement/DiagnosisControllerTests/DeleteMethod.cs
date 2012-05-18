using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Web.Models.Shared;
using Rhino.Mocks;
using Domain;

namespace Tests.Unit.Controllers.Areas.ServicesManagement.DiagnosisControllerTests
{
    [TestFixture]
    public class DeleteMethod
    {
        private readonly ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void Returns_JSON_With_ErrorMessage_When_DiagnosisId_IsNull()
        {
            //Arrange

            //Act
            var jsonResult = objectMother.controller.Delete(null);

            //Assert
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
            Assert.That(response.Message, Is.EqualTo("You must supply a diagnosisId in order to remove the diagnosis."));
        }

        [Test]
        public void Returns_JSON_With_SuccessMessage_When_Diagnosis_HasBeenDeleted()
        {
            //Arrange
            objectMother.queryDiagnosis.Expect(call => call.Load(objectMother.diagnosisId)).Return(objectMother.diagnosis);
            objectMother.deleteCommand.Expect(call => call.Execute(Arg<Diagnosis>.Matches(p => p.Id == objectMother.diagnosisId)));

            //Act
            var jsonResult = objectMother.controller.Delete(objectMother.diagnosis.Id);

            //Assert
            objectMother.queryDiagnosis.VerifyAllExpectations();
            objectMother.deleteCommand.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);
            var response = jsonResult.Data as JsonActionResponse;
            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("Diagnosis with code CHL/GON+ was removed."));
        }
    }
}
