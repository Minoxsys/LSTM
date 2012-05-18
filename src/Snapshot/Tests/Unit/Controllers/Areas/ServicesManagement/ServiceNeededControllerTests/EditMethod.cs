using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Web.Areas.ServicesManagement.Models.ServiceNeeded;
using Web.Models.Shared;
using Domain;
using Rhino.Mocks;

namespace Tests.Unit.Controllers.Areas.ServicesManagement.ServiceNeededControllerTests
{
    [TestFixture]
    public class EditMethod
    {
        public ObjectMother objectMother = new ObjectMother();

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
            var jsonResult = objectMother.controller.Edit(new ServiceNeededModel());

            //Assert
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
            Assert.That(response.Message, Is.EqualTo("You must supply a serviceNeededId in order to edit the service needed."));
        }

        [Test]
        public void Returns_JSON_With_ErrorMessage_When_ThereIsAllreadyAServiceNeeded_WithTheSame_Code_and_Keyword()
        {
            //Arrange
            ServiceNeededModel serviceNeededModel = new ServiceNeededModel()
            {
                Id = Guid.NewGuid(),
                Code = objectMother.serviceNeeded.Code,
                Keyword = objectMother.serviceNeeded.Keyword,
                Description = objectMother.serviceNeeded.Description
            };
            objectMother.queryServiceNeeded.Expect(call => call.Query()).Return(new ServiceNeeded[] { objectMother.serviceNeeded }.AsQueryable());
            //Act
            var jsonResult = objectMother.controller.Edit(serviceNeededModel);

            //Assert
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
        }
        [Test]
        public void Returns_JSON_With_SuccessMessage_When_ServiceNeeded_HasBeenSaved()
        {
            //Arrange
            ServiceNeededModel serviceNeededModel = new ServiceNeededModel()
            {
                Id = objectMother.serviceNeeded.Id,
                Code = "new Code",
                Keyword = "new keyword",
                Description = "new description"
            };
            objectMother.queryServiceNeeded.Expect(call => call.Query()).Return(new ServiceNeeded[] { objectMother.serviceNeeded }.AsQueryable());
            objectMother.saveCommand.Expect(call => call.Execute(Arg<ServiceNeeded>.Matches(p => p.Code != objectMother.serviceNeeded.Code)));

            //Act
            var jsonResult = objectMother.controller.Edit(serviceNeededModel);

            //Assert
            objectMother.queryServiceNeeded.VerifyAllExpectations();
            objectMother.saveCommand.VerifyAllExpectations();

            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("Service needed " + serviceNeededModel .Code+ " has been saved."));
        }
    }
}
