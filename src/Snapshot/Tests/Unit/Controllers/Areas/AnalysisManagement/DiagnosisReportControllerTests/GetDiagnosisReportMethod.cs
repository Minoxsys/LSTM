using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;
using Rhino.Mocks;
using NUnit.Framework;
using Web.Areas.AnalysisManagement.Models.DiagnosisReport;


namespace Tests.Unit.Controllers.Areas.AnalysisManagement.DiagnosisReportControllerTests
{
    [TestFixture]
    public class GetDiagnosisReportMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void Returns_All_Data()
        {
            //Arrange
            DiagnosisIndexModel model = new DiagnosisIndexModel();
            objectMother.queryDiagnosis.Expect(call => call.Query()).Return(objectMother.diagnosisList.AsQueryable());
            objectMother.queryOutpost.Expect(call => call.Query()).Return(objectMother.outpostList.AsQueryable());
            objectMother.queryMessageFromDispensary.Expect(call => call.Query()).Return(objectMother.messageList.AsQueryable());

            //Act
            var result = objectMother.controller.GetDiagnosisReport(model);

            //Assert
            objectMother.queryDiagnosis.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDispensary.VerifyAllExpectations();

            Assert.IsInstanceOf<DiagnosisReportIndexOutputModel>(result.Data);
            var jsonData = result.Data as DiagnosisReportIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(12, jsonData.TotalItems);
        }

        [Test]
        public void Returns_JSONData_ForCountry()
        {
            //Arrange
            DiagnosisIndexModel model = new DiagnosisIndexModel
            {
                countryId = objectMother.countryId.ToString()
            };
            objectMother.queryDiagnosis.Expect(call => call.Query()).Return(objectMother.diagnosisList.AsQueryable());
            objectMother.queryOutpost.Expect(call => call.Query()).Return(objectMother.outpostList.AsQueryable());
            objectMother.queryMessageFromDispensary.Expect(call => call.Query()).Return(objectMother.messageList.AsQueryable());

            //Act
            var result = objectMother.controller.GetDiagnosisReport(model);

            //Assert
            objectMother.queryDiagnosis.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDispensary.VerifyAllExpectations();

            Assert.IsInstanceOf<DiagnosisReportIndexOutputModel>(result.Data);
            var jsonData = result.Data as DiagnosisReportIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(12, jsonData.TotalItems);
        }

        [Test]
        public void Returns_JSONData_ForCountry_Region()
        {
            //Arrange
            DiagnosisIndexModel model = new DiagnosisIndexModel
            {
                countryId = objectMother.countryId.ToString(),
                regionId = objectMother.regionId.ToString()
            };
            objectMother.queryDiagnosis.Expect(call => call.Query()).Return(objectMother.diagnosisList.AsQueryable());
            objectMother.queryOutpost.Expect(call => call.Query()).Return(objectMother.outpostList.AsQueryable());
            objectMother.queryMessageFromDispensary.Expect(call => call.Query()).Return(objectMother.messageList.AsQueryable());

            //Act
            var result = objectMother.controller.GetDiagnosisReport(model);

            //Assert
            objectMother.queryDiagnosis.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDispensary.VerifyAllExpectations();

            Assert.IsInstanceOf<DiagnosisReportIndexOutputModel>(result.Data);
            var jsonData = result.Data as DiagnosisReportIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(8, jsonData.TotalItems);
        }

        [Test]
        public void Returns_JSONData_ForCountry_Region_District()
        {
            //Arrange
            DiagnosisIndexModel model = new DiagnosisIndexModel
            {
                countryId = objectMother.countryId.ToString(),
                regionId = objectMother.regionId.ToString(),
                districtId = objectMother.districtId.ToString()
            };
            objectMother.queryDiagnosis.Expect(call => call.Query()).Return(objectMother.diagnosisList.AsQueryable());
            objectMother.queryOutpost.Expect(call => call.Query()).Return(objectMother.outpostList.AsQueryable());
            objectMother.queryMessageFromDispensary.Expect(call => call.Query()).Return(objectMother.messageList.AsQueryable());

            //Act
            var result = objectMother.controller.GetDiagnosisReport(model);

            //Assert
            objectMother.queryDiagnosis.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDispensary.VerifyAllExpectations();

            Assert.IsInstanceOf<DiagnosisReportIndexOutputModel>(result.Data);
            var jsonData = result.Data as DiagnosisReportIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(4, jsonData.TotalItems);
        }

        [Test]
        public void Returns_JSONData_FilteredBy_StartDate()
        {
            //Arrange
            DiagnosisIndexModel model = new DiagnosisIndexModel
            {
                startDate = DateTime.UtcNow.AddMonths(-3).ToString()
            };
            objectMother.queryDiagnosis.Expect(call => call.Query()).Return(objectMother.diagnosisList.AsQueryable());
            objectMother.queryOutpost.Expect(call => call.Query()).Return(objectMother.outpostList.AsQueryable());
            objectMother.queryMessageFromDispensary.Expect(call => call.Query()).Return(objectMother.messageList.AsQueryable());

            //Act
            var result = objectMother.controller.GetDiagnosisReport(model);

            //Assert
            objectMother.queryDiagnosis.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDispensary.VerifyAllExpectations();

            Assert.IsInstanceOf<DiagnosisReportIndexOutputModel>(result.Data);
            var jsonData = result.Data as DiagnosisReportIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(12, jsonData.TotalItems);
            Assert.AreEqual(1, jsonData.Diagnosis[0].Female);
            Assert.AreEqual(0, jsonData.Diagnosis[0].Male);
            Assert.AreEqual(1, jsonData.Diagnosis[0].NumberOfPatients);
        }

        [Test]
        public void Returns_JSONData_FilteredBy_EndDate()
        {
            //Arrange
            DiagnosisIndexModel model = new DiagnosisIndexModel
            {
                endDate = DateTime.UtcNow.AddMonths(-3).ToString()
            };
            objectMother.queryDiagnosis.Expect(call => call.Query()).Return(objectMother.diagnosisList.AsQueryable());
            objectMother.queryOutpost.Expect(call => call.Query()).Return(objectMother.outpostList.AsQueryable());
            objectMother.queryMessageFromDispensary.Expect(call => call.Query()).Return(objectMother.messageList.AsQueryable());

            //Act
            var result = objectMother.controller.GetDiagnosisReport(model);

            //Assert
            objectMother.queryDiagnosis.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDispensary.VerifyAllExpectations();

            Assert.IsInstanceOf<DiagnosisReportIndexOutputModel>(result.Data);
            var jsonData = result.Data as DiagnosisReportIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(12, jsonData.TotalItems);
            Assert.AreEqual(1, jsonData.Diagnosis[1].Female);
            Assert.AreEqual(0, jsonData.Diagnosis[1].Male);
            Assert.AreEqual(1, jsonData.Diagnosis[1].NumberOfPatients);
        }


    }
}
