using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Web.Areas.AnalysisManagement.Models.PatientsReport;

namespace Tests.Unit.Controllers.Areas.AnalysisManagement.PatientsReportControllerTests
{
    [TestFixture]
    public class GetPatientsReportMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void Returns_JSON_With_AllMessages_FromDrugShop()
        {
            //Arrange
            objectMother.queryMessageFromDrugShop.Expect(call => call.Query()).Return(objectMother.listOfMessagesFromDrugShop.AsQueryable());
            objectMother.queryOutpost.Expect(call => call.Load(objectMother.drugshopId)).Return(objectMother.drugshop);
            objectMother.queryMessageFromDispensary.Expect(call => call.Query()).Return(objectMother.listOfMessagesFromDispensary.AsQueryable());
            objectMother.queryOutpost.Expect(call => call.Load(objectMother.dispensaryId)).Return(objectMother.dispensary);

            //Act
            var result = objectMother.controller.GetPatientsReport(new PatientReportIndexModel());

            //Assert
            objectMother.queryMessageFromDrugShop.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDispensary.VerifyAllExpectations();

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<PatientReportIndexOutputModel>(result.Data);
            var jsonData = result.Data as PatientReportIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(7, jsonData.TotalItems);
        }

        [Test]
        public void Returns_JSON_With_Messages_Filtered_By_Country()
        {
            //Arrange
            objectMother.queryMessageFromDrugShop.Expect(call => call.Query()).Return(objectMother.listOfMessagesFromDrugShop.AsQueryable());
            objectMother.queryMessageFromDispensary.Expect(call => call.Query()).Return(objectMother.listOfMessagesFromDispensary.AsQueryable());

            objectMother.queryOutpost.Expect(call => call.Load(objectMother.drugshopId)).Return(objectMother.drugshop);
            objectMother.queryOutpost.Expect(call => call.Load(objectMother.drugshopId1)).Return(objectMother.drugshop1);
            objectMother.queryOutpost.Expect(call => call.Load(objectMother.drugshopId2)).Return(objectMother.drugshop2);
            objectMother.queryOutpost.Expect(call => call.Load(objectMother.dispensaryId)).Return(objectMother.dispensary);

            PatientReportIndexModel model = new PatientReportIndexModel { countryId = objectMother.countryId.ToString() };

            //Act
            var result = objectMother.controller.GetPatientsReport(model);

            //Assert
            objectMother.queryMessageFromDrugShop.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDispensary.VerifyAllExpectations();

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<PatientReportIndexOutputModel>(result.Data);
            var jsonData = result.Data as PatientReportIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(5, jsonData.TotalItems);
        }

        [Test]
        public void Returns_JSON_With_Messages_Filtered_By_Region()
        {
            //Arrange
            objectMother.queryMessageFromDrugShop.Expect(call => call.Query()).Return(objectMother.listOfMessagesFromDrugShop.AsQueryable());
            objectMother.queryMessageFromDispensary.Expect(call => call.Query()).Return(objectMother.listOfMessagesFromDispensary.AsQueryable());

            objectMother.queryOutpost.Expect(call => call.Load(objectMother.drugshopId)).Return(objectMother.drugshop);
            objectMother.queryOutpost.Expect(call => call.Load(objectMother.drugshopId1)).Return(objectMother.drugshop1);
            objectMother.queryOutpost.Expect(call => call.Load(objectMother.drugshopId2)).Return(objectMother.drugshop2);
            objectMother.queryOutpost.Expect(call => call.Load(objectMother.dispensaryId)).Return(objectMother.dispensary);

            PatientReportIndexModel model = new PatientReportIndexModel
            {
                countryId = objectMother.countryId.ToString(),
                regionId = objectMother.regionId.ToString()
            };

            //Act
            var result = objectMother.controller.GetPatientsReport(model);

            //Assert
            objectMother.queryMessageFromDrugShop.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDispensary.VerifyAllExpectations();

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<PatientReportIndexOutputModel>(result.Data);
            var jsonData = result.Data as PatientReportIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(4, jsonData.TotalItems);
        }

        [Test]
        public void Returns_JSON_With_Messages_Filtered_By_District()
        {
            //Arrange
            objectMother.queryMessageFromDrugShop.Expect(call => call.Query()).Return(objectMother.listOfMessagesFromDrugShop.AsQueryable());
            objectMother.queryMessageFromDispensary.Expect(call => call.Query()).Return(objectMother.listOfMessagesFromDispensary.AsQueryable());

            objectMother.queryOutpost.Expect(call => call.Load(objectMother.drugshopId)).Return(objectMother.drugshop);
            objectMother.queryOutpost.Expect(call => call.Load(objectMother.drugshopId1)).Return(objectMother.drugshop1);
            objectMother.queryOutpost.Expect(call => call.Load(objectMother.drugshopId2)).Return(objectMother.drugshop2);
            objectMother.queryOutpost.Expect(call => call.Load(objectMother.dispensaryId)).Return(objectMother.dispensary);

            PatientReportIndexModel model = new PatientReportIndexModel
            {
                countryId = objectMother.countryId.ToString(),
                regionId = objectMother.regionId.ToString(),
                districtId = objectMother.districtId.ToString()
            };

            //Act
            var result = objectMother.controller.GetPatientsReport(model);

            //Assert
            objectMother.queryMessageFromDrugShop.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDispensary.VerifyAllExpectations();

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<PatientReportIndexOutputModel>(result.Data);
            var jsonData = result.Data as PatientReportIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(3, jsonData.TotalItems);
        }

        [Test]
        public void Returns_JSON_With_Messages_Filtered_By_Condition()
        {
            //Arrange
            objectMother.queryMessageFromDrugShop.Expect(call => call.Query()).Return(objectMother.listOfMessagesFromDrugShop.AsQueryable());
            objectMother.queryMessageFromDispensary.Expect(call => call.Query()).Return(objectMother.listOfMessagesFromDispensary.AsQueryable());

            objectMother.queryOutpost.Expect(call => call.Load(objectMother.drugshopId)).Return(objectMother.drugshop);
            objectMother.queryOutpost.Expect(call => call.Load(objectMother.drugshopId2)).Return(objectMother.drugshop2);
            objectMother.queryOutpost.Expect(call => call.Load(objectMother.dispensaryId)).Return(objectMother.dispensary);

            PatientReportIndexModel model = new PatientReportIndexModel
            {
                conditionId = objectMother.conditionId.ToString()
            };

            //Act
            var result = objectMother.controller.GetPatientsReport(model);

            //Assert
            objectMother.queryMessageFromDrugShop.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDispensary.VerifyAllExpectations();

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<PatientReportIndexOutputModel>(result.Data);
            var jsonData = result.Data as PatientReportIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(4, jsonData.TotalItems);
        }

        [Test]
        public void Returns_JSON_With_Messages_Filtered_By_Diagnosis()
        {
            //Arrange
            objectMother.queryMessageFromDrugShop.Expect(call => call.Query()).Return(objectMother.listOfMessagesFromDrugShop.AsQueryable());
            objectMother.queryMessageFromDispensary.Expect(call => call.Query()).Return(objectMother.listOfMessagesFromDispensary.AsQueryable());

            objectMother.queryOutpost.Expect(call => call.Load(objectMother.drugshopId)).Return(objectMother.drugshop);
            objectMother.queryOutpost.Expect(call => call.Load(objectMother.dispensaryId)).Return(objectMother.dispensary);

            PatientReportIndexModel model = new PatientReportIndexModel
            {
                diagnosisId = objectMother.diagnosisId.ToString()
            };

            //Act
            var result = objectMother.controller.GetPatientsReport(model);

            //Assert
            objectMother.queryMessageFromDrugShop.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDispensary.VerifyAllExpectations();

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<PatientReportIndexOutputModel>(result.Data);
            var jsonData = result.Data as PatientReportIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(5, jsonData.TotalItems);
        }

        [Test]
        public void Returns_JSON_With_Messages_Filtered_By_Treatment()
        {
            //Arrange
            objectMother.queryMessageFromDrugShop.Expect(call => call.Query()).Return(objectMother.listOfMessagesFromDrugShop.AsQueryable());
            objectMother.queryMessageFromDispensary.Expect(call => call.Query()).Return(objectMother.listOfMessagesFromDispensary.AsQueryable());

            objectMother.queryOutpost.Expect(call => call.Load(objectMother.drugshopId)).Return(objectMother.drugshop);
            objectMother.queryOutpost.Expect(call => call.Load(objectMother.dispensaryId)).Return(objectMother.dispensary);

            PatientReportIndexModel model = new PatientReportIndexModel
            {
                treatmentId = objectMother.treatmentId.ToString()
            };

            //Act
            var result = objectMother.controller.GetPatientsReport(model);

            //Assert
            objectMother.queryMessageFromDrugShop.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDispensary.VerifyAllExpectations();

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<PatientReportIndexOutputModel>(result.Data);
            var jsonData = result.Data as PatientReportIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(3, jsonData.TotalItems);
        }

        [Test]
        public void Returns_JSON_With_Messages_Filtered_By_Advice()
        {
            //Arrange
            objectMother.queryMessageFromDrugShop.Expect(call => call.Query()).Return(objectMother.listOfMessagesFromDrugShop.AsQueryable());
            objectMother.queryMessageFromDispensary.Expect(call => call.Query()).Return(objectMother.listOfMessagesFromDispensary.AsQueryable());

            objectMother.queryOutpost.Expect(call => call.Load(objectMother.drugshopId)).Return(objectMother.drugshop);
            objectMother.queryOutpost.Expect(call => call.Load(objectMother.dispensaryId)).Return(objectMother.dispensary);

            PatientReportIndexModel model = new PatientReportIndexModel
            {
                adviceId = objectMother.adviceId.ToString()
            };

            //Act
            var result = objectMother.controller.GetPatientsReport(model);

            //Assert
            objectMother.queryMessageFromDrugShop.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDispensary.VerifyAllExpectations();

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<PatientReportIndexOutputModel>(result.Data);
            var jsonData = result.Data as PatientReportIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(2, jsonData.TotalItems);
        }

        [Test]
        public void Returns_JSON_With_Messages_Filtered_By_Gender()
        {
            //Arrange
            objectMother.queryMessageFromDrugShop.Expect(call => call.Query()).Return(objectMother.listOfMessagesFromDrugShop.AsQueryable());
            objectMother.queryMessageFromDispensary.Expect(call => call.Query()).Return(objectMother.listOfMessagesFromDispensary.AsQueryable());

            objectMother.queryOutpost.Expect(call => call.Load(objectMother.drugshopId)).Return(objectMother.drugshop);
            objectMother.queryOutpost.Expect(call => call.Load(objectMother.dispensaryId)).Return(objectMother.dispensary);

            PatientReportIndexModel model = new PatientReportIndexModel
            {
                gender = "M"
            };

            //Act
            var result = objectMother.controller.GetPatientsReport(model);

            //Assert
            objectMother.queryMessageFromDrugShop.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDispensary.VerifyAllExpectations();

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<PatientReportIndexOutputModel>(result.Data);
            var jsonData = result.Data as PatientReportIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(3, jsonData.TotalItems);
        }

        [Test]
        public void Returns_JSON_With_Messages_Filtered_By_StartDate()
        {
            //Arrange
            objectMother.queryMessageFromDrugShop.Expect(call => call.Query()).Return(objectMother.listOfMessagesFromDrugShop.AsQueryable());
            objectMother.queryMessageFromDispensary.Expect(call => call.Query()).Return(objectMother.listOfMessagesFromDispensary.AsQueryable());

            objectMother.queryOutpost.Expect(call => call.Load(objectMother.drugshopId)).Return(objectMother.drugshop);
            objectMother.queryOutpost.Expect(call => call.Load(objectMother.dispensaryId)).Return(objectMother.dispensary);

            PatientReportIndexModel model = new PatientReportIndexModel
            {
                startDate = DateTime.UtcNow.AddDays(-1).ToString()
            };

            //Act
            var result = objectMother.controller.GetPatientsReport(model);

            //Assert
            objectMother.queryMessageFromDrugShop.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDispensary.VerifyAllExpectations();

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<PatientReportIndexOutputModel>(result.Data);
            var jsonData = result.Data as PatientReportIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(4, jsonData.TotalItems);
        }

        [Test]
        public void Returns_JSON_With_Messages_Filtered_By_EndDate()
        {
            //Arrange
            objectMother.queryMessageFromDrugShop.Expect(call => call.Query()).Return(objectMother.listOfMessagesFromDrugShop.AsQueryable());
            objectMother.queryMessageFromDispensary.Expect(call => call.Query()).Return(objectMother.listOfMessagesFromDispensary.AsQueryable());

            objectMother.queryOutpost.Expect(call => call.Load(objectMother.drugshopId)).Return(objectMother.drugshop);
            objectMother.queryOutpost.Expect(call => call.Load(objectMother.dispensaryId)).Return(objectMother.dispensary);

            PatientReportIndexModel model = new PatientReportIndexModel
            {
                startDate = DateTime.UtcNow.ToString()
            };

            //Act
            var result = objectMother.controller.GetPatientsReport(model);

            //Assert
            objectMother.queryMessageFromDrugShop.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDispensary.VerifyAllExpectations();

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<PatientReportIndexOutputModel>(result.Data);
            var jsonData = result.Data as PatientReportIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(2, jsonData.TotalItems);
        }
    }
}
