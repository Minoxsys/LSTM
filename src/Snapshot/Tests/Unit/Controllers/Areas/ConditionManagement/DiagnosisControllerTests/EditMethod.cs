﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Web.Models.Shared;
using Domain;
using Rhino.Mocks;
using Web.Areas.ConditionManagement.Models.Diagnosis;

namespace Tests.Unit.Controllers.Areas.ConditionManagement.DiagnosisControllerTests
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
        public void Returns_JSON_With_ErrorMessage_When_DiagnosisId_IsNull()
        {
            //Arrange

            //Act
            var jsonResult = objectMother.controller.Edit(new DiagnosisInputModel());

            //Assert
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
            Assert.That(response.Message, Is.EqualTo("You must supply a diagnosisId in order to edit the diagnosis."));
        }

        [Test]
        public void Returns_JSON_With_ErrorMessage_When_ThereIsAllreadyADiagnosis_WithTheSame_Code_and_Keyword()
        {
            //Arrange
            DiagnosisInputModel diagnosisModel = new DiagnosisInputModel()
            {
                Id = Guid.NewGuid(),
                Code = objectMother.diagnosis.Code,
                Keyword = objectMother.diagnosis.Keyword,
                Description = objectMother.diagnosis.Description
            };
            objectMother.queryDiagnosis.Expect(call => call.Query()).Return(new Diagnosis[] { objectMother.diagnosis }.AsQueryable());
            //Act
            var jsonResult = objectMother.controller.Edit(diagnosisModel);

            //Assert
            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
        }
        [Test]
        public void Returns_JSON_With_SuccessMessage_When_Diagnosis_HasBeenSaved()
        {
            //Arrange
            DiagnosisInputModel diagnosisModel = new DiagnosisInputModel()
            {
                Id = objectMother.diagnosis.Id,
                Code = "new Code",
                Keyword = "new keyword",
                Description = "new description"
            };
            objectMother.queryDiagnosis.Expect(call => call.Query()).Return(new Diagnosis[] { objectMother.diagnosis }.AsQueryable());
            objectMother.queryDiagnosis.Expect(call => call.Load(objectMother.diagnosis.Id)).Return(new Diagnosis{Messages = objectMother.diagnosis.Messages});
            objectMother.saveCommand.Expect(call => call.Execute(Arg<Diagnosis>.Matches(p => p.Code != objectMother.diagnosis.Code)));
            
            //Act
            var jsonResult = objectMother.controller.Edit(diagnosisModel);

            //Assert
            objectMother.queryDiagnosis.VerifyAllExpectations();
            objectMother.saveCommand.VerifyAllExpectations();
            

            var response = jsonResult.Data as JsonActionResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("Diagnosis new Code has been saved."));
        }

    }
}
