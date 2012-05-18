using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Web.Models.Shared;
using Rhino.Mocks;
using Domain;

namespace Tests.Unit.Controllers.Areas.ServicesManagement.ServiceNeededControllerTests
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
        public void Returns_JSON_With_ErrorMessage_When_ServiceNeededId_IsNull()
        {
            //Arrange

            //Act
            var jsonResult = objectMother.controller.Delete(null);

            //Assert
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
            Assert.That(response.Message, Is.EqualTo("You must supply a serviceNeededId in order to remove the service needed."));
        }

        [Test]
        public void Returns_JSON_With_SuccessMessage_When_ServiceNeeded_HasBeenDeleted()
        {
            //Arrange
            objectMother.queryServiceNeeded.Expect(call => call.Load(objectMother.serviceNeededId)).Return(objectMother.serviceNeeded);
            objectMother.deleteCommand.Expect(call => call.Execute(Arg<ServiceNeeded>.Matches(p => p.Id == objectMother.serviceNeededId)));

            //Act
            var jsonResult = objectMother.controller.Delete(objectMother.serviceNeeded.Id);

            //Assert
            objectMother.queryServiceNeeded.VerifyAllExpectations();
            objectMother.deleteCommand.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);
            var response = jsonResult.Data as JsonActionResponse;
            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("Service needed with code "+objectMother.serviceNeeded.Code+" was removed."));
        }
    }
}
