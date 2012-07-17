using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;
using Rhino.Mocks;
using NUnit.Framework;
using Web.Areas.AnalysisManagement.Models.TreatmentReport;


namespace Tests.Unit.Controllers.Areas.AnalysisManagement.TreatmentReportControllerTests
{
    [TestFixture]
    public class GetTreatmentReportMethod
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
            TreatmentIndexModel model = new TreatmentIndexModel();
            objectMother.queryTreatment.Expect(call => call.Query()).Return(objectMother.treatmentList.AsQueryable());
            objectMother.queryOutpost.Expect(call => call.Query()).Return(objectMother.outpostList.AsQueryable());
            objectMother.queryMessageFromDispensary.Expect(call => call.Query()).Return(objectMother.messageList.AsQueryable());

            //Act
            var result = objectMother.controller.GetTreatmentReport(model);

            //Assert
            objectMother.queryTreatment.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDispensary.VerifyAllExpectations();

            Assert.IsInstanceOf<TreatmentReportIndexOutputModel>(result.Data);
            var jsonData = result.Data as TreatmentReportIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(12, jsonData.TotalItems);
        }

        [Test]
        public void Returns_JSONData_ForCountry()
        {
            //Arrange
            TreatmentIndexModel model = new TreatmentIndexModel
            {
                countryId = objectMother.countryId.ToString()
            };
            objectMother.queryTreatment.Expect(call => call.Query()).Return(objectMother.treatmentList.AsQueryable());
            objectMother.queryOutpost.Expect(call => call.Query()).Return(objectMother.outpostList.AsQueryable());
            objectMother.queryMessageFromDispensary.Expect(call => call.Query()).Return(objectMother.messageList.AsQueryable());

            //Act
            var result = objectMother.controller.GetTreatmentReport(model);

            //Assert
            objectMother.queryTreatment.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDispensary.VerifyAllExpectations();

            Assert.IsInstanceOf<TreatmentReportIndexOutputModel>(result.Data);
            var jsonData = result.Data as TreatmentReportIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(12, jsonData.TotalItems);
        }

        [Test]
        public void Returns_JSONData_ForCountry_Region()
        {
            //Arrange
            TreatmentIndexModel model = new TreatmentIndexModel
            {
                countryId = objectMother.countryId.ToString(),
                regionId = objectMother.regionId.ToString()
            };
            objectMother.queryTreatment.Expect(call => call.Query()).Return(objectMother.treatmentList.AsQueryable());
            objectMother.queryOutpost.Expect(call => call.Query()).Return(objectMother.outpostList.AsQueryable());
            objectMother.queryMessageFromDispensary.Expect(call => call.Query()).Return(objectMother.messageList.AsQueryable());

            //Act
            var result = objectMother.controller.GetTreatmentReport(model);

            //Assert
            objectMother.queryTreatment.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDispensary.VerifyAllExpectations();

            Assert.IsInstanceOf<TreatmentReportIndexOutputModel>(result.Data);
            var jsonData = result.Data as TreatmentReportIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(8, jsonData.TotalItems);
        }

        [Test]
        public void Returns_JSONData_ForCountry_Region_District()
        {
            //Arrange
            TreatmentIndexModel model = new TreatmentIndexModel
            {
                countryId = objectMother.countryId.ToString(),
                regionId = objectMother.regionId.ToString(),
                districtId = objectMother.districtId.ToString()
            };
            objectMother.queryTreatment.Expect(call => call.Query()).Return(objectMother.treatmentList.AsQueryable());
            objectMother.queryOutpost.Expect(call => call.Query()).Return(objectMother.outpostList.AsQueryable());
            objectMother.queryMessageFromDispensary.Expect(call => call.Query()).Return(objectMother.messageList.AsQueryable());

            //Act
            var result = objectMother.controller.GetTreatmentReport(model);

            //Assert
            objectMother.queryTreatment.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDispensary.VerifyAllExpectations();

            Assert.IsInstanceOf<TreatmentReportIndexOutputModel>(result.Data);
            var jsonData = result.Data as TreatmentReportIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(4, jsonData.TotalItems);
        }

        [Test]
        public void Returns_JSONData_FilteredBy_StartDate()
        {
            //Arrange
            TreatmentIndexModel model = new TreatmentIndexModel
            {
                startDate = DateTime.UtcNow.AddMonths(-3).ToString()
            };
            objectMother.queryTreatment.Expect(call => call.Query()).Return(objectMother.treatmentList.AsQueryable());
            objectMother.queryOutpost.Expect(call => call.Query()).Return(objectMother.outpostList.AsQueryable());
            objectMother.queryMessageFromDispensary.Expect(call => call.Query()).Return(objectMother.messageList.AsQueryable());

            //Act
            var result = objectMother.controller.GetTreatmentReport(model);

            //Assert
            objectMother.queryTreatment.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDispensary.VerifyAllExpectations();

            Assert.IsInstanceOf<TreatmentReportIndexOutputModel>(result.Data);
            var jsonData = result.Data as TreatmentReportIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(12, jsonData.TotalItems);
        }

        [Test]
        public void Returns_JSONData_FilteredBy_EndDate()
        {
            //Arrange
            TreatmentIndexModel model = new TreatmentIndexModel
            {
                endDate = DateTime.UtcNow.AddMonths(-3).ToString()
            };
            objectMother.queryTreatment.Expect(call => call.Query()).Return(objectMother.treatmentList.AsQueryable());
            objectMother.queryOutpost.Expect(call => call.Query()).Return(objectMother.outpostList.AsQueryable());
            objectMother.queryMessageFromDispensary.Expect(call => call.Query()).Return(objectMother.messageList.AsQueryable());

            //Act
            var result = objectMother.controller.GetTreatmentReport(model);

            //Assert
            objectMother.queryTreatment.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDispensary.VerifyAllExpectations();

            Assert.IsInstanceOf<TreatmentReportIndexOutputModel>(result.Data);
            var jsonData = result.Data as TreatmentReportIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(12, jsonData.TotalItems);
        }


    }
}
