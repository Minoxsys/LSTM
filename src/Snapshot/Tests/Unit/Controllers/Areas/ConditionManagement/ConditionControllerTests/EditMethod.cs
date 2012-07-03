using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Web.Areas.ConditionManagement.Models.Condition;
using Web.Models.Shared;
using Domain;
using Rhino.Mocks;

namespace Tests.Unit.Controllers.Areas.ConditionManagement.ConditionControllerTests
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
        public void Returns_JSON_With_ErrorMessage_When_ConditionId_IsNull()
        {
            //Arrange

            //Act
            var jsonResult = objectMother.controller.Edit(new ConditionModel());

            //Assert
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
            Assert.That(response.Message, Is.EqualTo("You must supply a symptomId in order to edit the symptom."));
        }

        [Test]
        public void Returns_JSON_With_ErrorMessage_When_ThereIsAllreadyACondition_WithTheSame_Code_and_Keyword()
        {
            //Arrange
            ConditionModel conditionModel = new ConditionModel()
            {
                Id = Guid.NewGuid(),
                Code = objectMother.condition.Code,
                Keyword = objectMother.condition.Keyword,
                Description = objectMother.condition.Description
            };
            objectMother.queryCondition.Expect(call => call.Query()).Return(new Condition[] { objectMother.condition }.AsQueryable());
            //Act
            var jsonResult = objectMother.controller.Edit(conditionModel);

            //Assert
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
        }
        [Test]
        public void Returns_JSON_With_SuccessMessage_When_Condition_HasBeenSaved()
        {
            //Arrange
            ConditionModel conditionModel = new ConditionModel()
            {
                Id = objectMother.condition.Id,
                Code = "new Code",
                Keyword = "new keyword",
                Description = "new description"
            };
            objectMother.queryCondition.Expect(call => call.Query()).Return(new Condition[] { objectMother.condition }.AsQueryable());
            objectMother.saveCommand.Expect(call => call.Execute(Arg<Condition>.Matches(p => p.Code != objectMother.condition.Code)));

            //Act
            var jsonResult = objectMother.controller.Edit(conditionModel);

            //Assert
            objectMother.queryCondition.VerifyAllExpectations();
            objectMother.saveCommand.VerifyAllExpectations();

            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("Symptom " + conditionModel.Code + " has been saved."));
        }
    }
}
