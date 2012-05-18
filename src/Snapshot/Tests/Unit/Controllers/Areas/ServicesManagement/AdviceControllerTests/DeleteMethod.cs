using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Web.Models.Shared;
using Domain;

namespace Tests.Unit.Controllers.Areas.ServicesManagement.AdviceControllerTests
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
        public void Returns_JSON_With_ErrorMessage_When_AdviceId_IsNull()
        {
            //Arrange

            //Act
            var jsonResult = objectMother.controller.Delete(null);

            //Assert
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
            Assert.That(response.Message, Is.EqualTo("You must supply a adviceId in order to remove the advice."));
        }

        [Test]
        public void Returns_JSON_With_SuccessMessage_When_Advice_HasBeenDeleted()
        {
            //Arrange
            objectMother.queryAdvice.Expect(call => call.Load(objectMother.adviceId)).Return(objectMother.advice);
            objectMother.deleteCommand.Expect(call => call.Execute(Arg<Advice>.Matches(p => p.Id == objectMother.adviceId)));

            //Act
            var jsonResult = objectMother.controller.Delete(objectMother.advice.Id);

            //Assert
            objectMother.queryAdvice.VerifyAllExpectations();
            objectMother.deleteCommand.VerifyAllExpectations();

            Assert.IsNotNull(jsonResult);
            var response = jsonResult.Data as JsonActionResponse;
            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("Advice with code "+objectMother.advice.Code+" was removed."));
        }
    }
}
