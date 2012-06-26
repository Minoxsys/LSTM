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
    public class CreateMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void Returns_JSON_With_ErrorMessage_When_ServiceNeeded_Code_IsNull()
        {
            //Arrange

            //Act
            var jsonResult = objectMother.controller.Create(new ServiceNeededModel());

            //Assert
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
            Assert.That(response.Message, Is.EqualTo("The condition has not been saved!"));
        }

        [Test]
        public void Returns_JSON_With_ErrorMessage_When_ServiceNeeded_Code_AND_Keyword_AlreadyExists()
        {
            //Arrange
            ServiceNeededModel serviceNeededInputModel = new ServiceNeededModel()
            {
                Keyword = objectMother.serviceNeeded.Keyword,
                Code = objectMother.serviceNeeded.Code,
                Description = "new description"
            };
            objectMother.queryServiceNeeded.Expect(call => call.Query()).Return(new ServiceNeeded[] { objectMother.serviceNeeded }.AsQueryable());

            //Act
            var jsonResult = objectMother.controller.Create(serviceNeededInputModel);

            //Assert
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
        }

        [Test]
        public void Returns_JSON_With_SuccessMessage_When_ServiceNeeded_Has_Been_Saved()
        {
            //Arrange
            ServiceNeededModel serviceNeededInputModel = new ServiceNeededModel()
            {
                Keyword = "Family planning",
                Code = "FP-8",
                Description = "Tubal ligation"
            };
            objectMother.queryServiceNeeded.Expect(call => call.Query()).Return(new ServiceNeeded[] { objectMother.serviceNeeded }.AsQueryable());
            objectMother.saveCommand.Expect(call => call.Execute(Arg<ServiceNeeded>.Matches(p => p.Code == serviceNeededInputModel.Code)));

            //Act
            var jsonResult = objectMother.controller.Create(serviceNeededInputModel);

            //Assert
            objectMother.queryServiceNeeded.VerifyAllExpectations();
            objectMother.saveCommand.VerifyAllExpectations();

            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("Condition " + serviceNeededInputModel.Code + " has been saved."));
        }
    }
}
