using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Web.Areas.ConditionManagement.Models.Treatment;
using Web.Models.Shared;
using Rhino.Mocks;
using Domain;

namespace Tests.Unit.Controllers.Areas.ConditionManagement.TreatmentControllerTests
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
        public void Returns_JSON_With_ErrorMessage_When_Treatment_Code_IsNull()
        {
            //Arrange

            //Act
            var jsonResult = objectMother.controller.Create(new TreatmentInputModel());

            //Assert
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
            Assert.That(response.Message, Is.EqualTo("The treatment has not been saved!"));
        }

        [Test]
        public void Returns_JSON_With_ErrorMessage_When_Treatment_CodeandKeyword_AlreadyExists()
        {
            //Arrange
            TreatmentInputModel treatmentModel = new TreatmentInputModel()
            {
                Keyword = objectMother.treatment.Keyword,
                Code = objectMother.treatment.Code,
                Description = "new description"
            };
            objectMother.queryTreatment.Expect(call => call.Query()).Return(new Treatment[] { objectMother.treatment }.AsQueryable());

            //Act
            var jsonResult = objectMother.controller.Create(treatmentModel);

            //Assert
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
        }

        [Test]
        public void Returns_JSON_With_SuccessMessage_When_Treatment_HasBeenSaved()
        {
            //Arrange
            TreatmentInputModel treatmentModel = new TreatmentInputModel()
            {
                Keyword = "Keyword",
                Code = "Code",
                Description = "Description"
            };
            objectMother.queryTreatment.Expect(call => call.Query()).Return(new Treatment[] { objectMother.treatment }.AsQueryable());
            objectMother.saveCommand.Expect(call => call.Execute(Arg<Treatment>.Matches(p => p.Code == treatmentModel.Code)));

            //Act
            var jsonResult = objectMother.controller.Create(treatmentModel);

            //Assert
            objectMother.queryTreatment.VerifyAllExpectations();
            objectMother.saveCommand.VerifyAllExpectations();

            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("Treatment Code has been saved."));
        }
    }
}
