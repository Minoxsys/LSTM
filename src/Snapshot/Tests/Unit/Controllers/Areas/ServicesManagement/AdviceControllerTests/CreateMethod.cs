using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Web.Areas.ServicesManagement.Models.Advice;
using Web.Models.Shared;
using Domain;

namespace Tests.Unit.Controllers.Areas.ServicesManagement.AdviceControllerTests
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
        public void Returns_JSON_With_ErrorMessage_When_Advice_Code_IsNull()
        {
            //Arrange

            //Act
            var jsonResult = objectMother.controller.Create(new AdviceModel());

            //Assert
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
            Assert.That(response.Message, Is.EqualTo("The advice has not been saved!"));
        }

        [Test]
        public void Returns_JSON_With_ErrorMessage_When_Advice_Code_AND_Keyword_AlreadyExists()
        {
            //Arrange
            AdviceModel adviceInputModel = new AdviceModel()
            {
                Keyword = objectMother.advice.Keyword,
                Code = objectMother.advice.Code,
                Description = "new description"
            };
            objectMother.queryAdvice.Expect(call => call.Query()).Return(new Advice[] { objectMother.advice }.AsQueryable());

            //Act
            var jsonResult = objectMother.controller.Create(adviceInputModel);

            //Assert
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
        }

        [Test]
        public void Returns_JSON_With_SuccessMessage_When_Advice_Has_Been_Saved()
        {
            //Arrange
            AdviceModel adviceInputModel = new AdviceModel()
            {
                Keyword = "To dispensary",
                Code = "A2",
                Description = "Patient referred to buy medicine from my collaborating drug shop"
            };
            objectMother.queryAdvice.Expect(call => call.Query()).Return(new Advice[] { objectMother.advice }.AsQueryable());
            objectMother.saveCommand.Expect(call => call.Execute(Arg<Advice>.Matches(p => p.Code == adviceInputModel.Code)));

            //Act
            var jsonResult = objectMother.controller.Create(adviceInputModel);

            //Assert
            objectMother.queryAdvice.VerifyAllExpectations();
            objectMother.saveCommand.VerifyAllExpectations();

            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("Advice "+adviceInputModel.Code+" has been saved."));
        }
    }
}
