using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Web.Areas.ConditionManagement.Models.Treatment;
using Web.Models.Shared;
using Domain;
using Rhino.Mocks;

namespace Tests.Unit.Controllers.Areas.ConditionManagement.TreatmentControllerTests
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
        public void Returns_JSON_With_ErrorMessage_When_TreatmentId_IsNull()
        {
            //Arrange

            //Act
            var jsonResult = objectMother.controller.Edit(new TreatmentInputModel());

            //Assert
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
            Assert.That(response.Message, Is.EqualTo("You must supply a treatmentId in order to edit the treatment."));
        }

        [Test]
        public void Returns_JSON_With_ErrorMessage_When_ThereIsAllreadyATreatment_WithTheSame_CodeandKeyword()
        {
            //Arrange
            TreatmentInputModel treatmentModel = new TreatmentInputModel()
            {
                Id = Guid.NewGuid(),
                Code = objectMother.treatment.Code,
                Keyword = objectMother.treatment.Keyword,
                Description = objectMother.treatment.Description
            };
            objectMother.queryTreatment.Expect(call => call.Query()).Return(new Treatment[] { objectMother.treatment }.AsQueryable());
            //Act
            var jsonResult = objectMother.controller.Edit(treatmentModel);

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
                Id = objectMother.treatment.Id,
                Code = "new Code",
                Keyword = "new keyword",
                Description = "new description"
            };
            objectMother.queryTreatment.Expect(call => call.Query()).Return(new Treatment[] { objectMother.treatment }.AsQueryable());
            objectMother.queryTreatment.Expect(call => call.Load(objectMother.treatment.Id)).Return(new Treatment {Messages= objectMother.treatment.Messages });
            objectMother.saveCommand.Expect(call => call.Execute(Arg<Treatment>.Matches(p => p.Code != objectMother.treatment.Code)));

            //Act
            var jsonResult = objectMother.controller.Edit(treatmentModel);

            //Assert
            objectMother.queryTreatment.VerifyAllExpectations();
            objectMother.saveCommand.VerifyAllExpectations();

            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("Treatment new Code has been saved."));
        }
    }
}
