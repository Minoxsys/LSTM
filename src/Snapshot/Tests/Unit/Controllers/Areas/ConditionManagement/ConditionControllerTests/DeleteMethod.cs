using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Web.Models.Shared;
using Rhino.Mocks;
using Domain;

namespace Tests.Unit.Controllers.Areas.ConditionManagement.ConditionControllerTests
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
        public void Returns_JSON_With_ErrorMessage_When_ConditionId_IsNull()
        {
            //Arrange

            //Act
            var jsonResult = objectMother.controller.Delete(null);

            //Assert
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
            Assert.That(response.Message, Is.EqualTo("You must supply a symptomId in order to remove the condition."));
        }

        [Test]
        public void Returns_JSON_With_SuccessMessage_When_Condition_HasBeenDeleted()
        {
            //Arrange
            objectMother.queryCondition.Expect(call => call.Load(objectMother.conditionId)).Return(objectMother.condition);
            objectMother.deleteCommand.Expect(call => call.Execute(Arg<Condition>.Matches(p => p.Id == objectMother.conditionId)));

            //Act
            var jsonResult = objectMother.controller.Delete(objectMother.condition.Id);

            //Assert
            objectMother.queryCondition.VerifyAllExpectations();
            objectMother.deleteCommand.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);
            var response = jsonResult.Data as JsonActionResponse;
            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("Symptom with code " + objectMother.condition.Code + " was removed."));
        }
    }
}
