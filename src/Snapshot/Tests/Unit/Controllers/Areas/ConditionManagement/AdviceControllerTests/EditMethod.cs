using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Web.Areas.ConditionManagement.Models.Advice;
using Web.Models.Shared;
using Domain;

namespace Tests.Unit.Controllers.Areas.ConditionManagement.AdviceControllerTests
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
        public void Returns_JSON_With_ErrorMessage_When_AdviceId_IsNull()
        {
            //Arrange

            //Act
            var jsonResult = objectMother.controller.Edit(new AdviceModel());

            //Assert
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
            Assert.That(response.Message, Is.EqualTo("You must supply a adviceId in order to edit the advice."));
        }

        [Test]
        public void Returns_JSON_With_ErrorMessage_When_ThereIsAllreadyAAdvice_WithTheSame_Code_and_Keyword()
        {
            //Arrange
            AdviceModel adviceModel = new AdviceModel()
            {
                Id = Guid.NewGuid(),
                Code = objectMother.advice.Code,
                Keyword = objectMother.advice.Keyword,
                Description = objectMother.advice.Description
            };
            objectMother.queryAdvice.Expect(call => call.Query()).Return(new Advice[] { objectMother.advice }.AsQueryable());
            //Act
            var jsonResult = objectMother.controller.Edit(adviceModel);

            //Assert
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
        }
        [Test]
        public void Returns_JSON_With_SuccessMessage_When_Advice_HasBeenSaved()
        {
            //Arrange
            AdviceModel adviceModel = new AdviceModel()
            {
                Id = objectMother.advice.Id,
                Code = "new Code",
                Keyword = "new keyword",
                Description = "new description"
            };
            objectMother.queryAdvice.Expect(call => call.Query()).Return(new Advice[] { objectMother.advice }.AsQueryable());
            objectMother.saveCommand.Expect(call => call.Execute(Arg<Advice>.Matches(p => p.Code != objectMother.advice.Code)));

            //Act
            var jsonResult = objectMother.controller.Edit(adviceModel);

            //Assert
            objectMother.queryAdvice.VerifyAllExpectations();
            objectMother.saveCommand.VerifyAllExpectations();

            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("Advice "+adviceModel.Code+" has been saved."));
        }
    }
}
