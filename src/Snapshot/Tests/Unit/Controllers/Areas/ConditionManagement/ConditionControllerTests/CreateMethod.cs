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
    public class CreateMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void Returns_JSON_With_ErrorMessage_When_Condition_Code_IsNull()
        {
            //Arrange

            //Act
            var jsonResult = objectMother.controller.Create(new ConditionModel());

            //Assert
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
            Assert.That(response.Message, Is.EqualTo("The symptom has not been saved!"));
        }

        [Test]
        public void Returns_JSON_With_ErrorMessage_When_Condition_Code_AND_Keyword_AlreadyExists()
        {
            //Arrange
            ConditionModel conditionInputModel = new ConditionModel()
            {
                Keyword = objectMother.condition.Keyword,
                Code = objectMother.condition.Code,
                Description = "new description"
            };
            objectMother.queryCondition.Expect(call => call.Query()).Return(new Condition[] { objectMother.condition }.AsQueryable());

            //Act
            var jsonResult = objectMother.controller.Create(conditionInputModel);

            //Assert
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
        }

        [Test]
        public void Returns_JSON_With_SuccessMessage_When_Condition_Has_Been_Saved()
        {
            //Arrange
            ConditionModel conditionInputModel = new ConditionModel()
            {
                Keyword = "Family planning",
                Code = "FP-8",
                Description = "Tubal ligation"
            };
            objectMother.queryCondition.Expect(call => call.Query()).Return(new Condition[] { objectMother.condition }.AsQueryable());
            objectMother.saveCommand.Expect(call => call.Execute(Arg<Condition>.Matches(p => p.Code == conditionInputModel.Code)));

            //Act
            var jsonResult = objectMother.controller.Create(conditionInputModel);

            //Assert
            objectMother.queryCondition.VerifyAllExpectations();
            objectMother.saveCommand.VerifyAllExpectations();

            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("Symptom " + conditionInputModel.Code + " has been saved."));
        }
    }
}
