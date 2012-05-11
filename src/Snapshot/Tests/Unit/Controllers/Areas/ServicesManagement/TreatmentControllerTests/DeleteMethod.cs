using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Web.Models.Shared;
using Domain;
using Rhino.Mocks;

namespace Tests.Unit.Controllers.Areas.ServicesManagement.TreatmentControllerTests
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
        public void Returns_JSON_With_ErrorMessage_When_TreatmentId_IsNull()
        {
            //Arrange

            //Act
            var jsonResult = objectMother.controller.Delete(null);

            //Assert
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
            Assert.That(response.Message, Is.EqualTo("You must supply a treatmentId in order to remove the treatment."));
        }

        [Test]
        public void Returns_JSON_With_SuccessMessage_When_Treatment_HasBeenDeleted()
        {
            //Arrange
            objectMother.queryTreatment.Expect(call => call.Load(objectMother.treatmentId)).Return(objectMother.treatment);
            objectMother.deleteCommand.Expect(call => call.Execute(Arg<Treatment>.Matches(p => p.Id == objectMother.treatmentId)));

            //Act
            var jsonResult = objectMother.controller.Delete(objectMother.treatmentId);

            //Assert
            objectMother.queryTreatment.VerifyAllExpectations();
            objectMother.deleteCommand.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);
            var response = jsonResult.Data as JsonActionResponse;
            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("Treatment with code RX1; RX2; NOT/AV. was removed."));
        }
    }
}
