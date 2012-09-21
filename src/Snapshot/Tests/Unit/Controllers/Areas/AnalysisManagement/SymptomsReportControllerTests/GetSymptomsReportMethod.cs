using System;
using System.Linq;
using Rhino.Mocks;
using NUnit.Framework;
using Web.Areas.AnalysisManagement.Models.SymptomsReport;

namespace Tests.Unit.Controllers.Areas.AnalysisManagement.SymptomsReportControllerTests
{
    [TestFixture]
    public class GetSymptomsReportMethod
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
            SymptomsIndexModel model = new SymptomsIndexModel();
            objectMother.querySymptoms.Expect(call => call.Query()).Return(objectMother.symptomsList.AsQueryable());
            objectMother.queryOutpost.Expect(call => call.Query()).Return(objectMother.outpostList.AsQueryable());
            objectMother.queryMessageFromDrugShop.Expect(call => call.Query()).Return(objectMother.messageList.AsQueryable());

            //Act
            var result = objectMother.controller.GetSymptomsReport(model);

            //Assert
            objectMother.querySymptoms.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDrugShop.VerifyAllExpectations();

            Assert.IsInstanceOf<SymptomsReportIndexOutputModel>(result.Data);
            var jsonData = result.Data as SymptomsReportIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(8, jsonData.TotalItems);
        }

        [Test]
        public void Returns_JSONData_ForCountry()
        {
            //Arrange
            SymptomsIndexModel model = new SymptomsIndexModel()
            {
                countryId = objectMother.countryId.ToString()
            };
            objectMother.querySymptoms.Expect(call => call.Query()).Return(objectMother.symptomsList.AsQueryable());
            objectMother.queryOutpost.Expect(call => call.Query()).Return(objectMother.outpostList.AsQueryable());
            objectMother.queryMessageFromDrugShop.Expect(call => call.Query()).Return(objectMother.messageList.AsQueryable());

            //Act
            var result = objectMother.controller.GetSymptomsReport(model);

            //Assert
            objectMother.querySymptoms.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDrugShop.VerifyAllExpectations();

            Assert.IsInstanceOf<SymptomsReportIndexOutputModel>(result.Data);
            var jsonData = result.Data as SymptomsReportIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(8, jsonData.TotalItems);
        }

        [Test]
        public void Returns_JSONData_ForCountry_Region()
        {
            //Arrange
            SymptomsIndexModel model = new SymptomsIndexModel
            {
                countryId = objectMother.countryId.ToString(),
                regionId = objectMother.regionId.ToString()
            };
            objectMother.querySymptoms.Expect(call => call.Query()).Return(objectMother.symptomsList.AsQueryable());
            objectMother.queryOutpost.Expect(call => call.Query()).Return(objectMother.outpostList.AsQueryable());
            objectMother.queryMessageFromDrugShop.Expect(call => call.Query()).Return(objectMother.messageList.AsQueryable());

            //Act
            var result = objectMother.controller.GetSymptomsReport(model);

            //Assert
            objectMother.querySymptoms.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDrugShop.VerifyAllExpectations();

            Assert.IsInstanceOf<SymptomsReportIndexOutputModel>(result.Data);
            var jsonData = result.Data as SymptomsReportIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(8, jsonData.TotalItems);
        }

        [Test]
        public void Returns_JSONData_ForCountry_Region_District()
        {
            //Arrange
            SymptomsIndexModel model = new SymptomsIndexModel
            {
                countryId = objectMother.countryId.ToString(),
                regionId = objectMother.regionId.ToString(),
                districtId = objectMother.districtId.ToString()
            };
            objectMother.querySymptoms.Expect(call => call.Query()).Return(objectMother.symptomsList.AsQueryable());
            objectMother.queryOutpost.Expect(call => call.Query()).Return(objectMother.outpostList.AsQueryable());
            objectMother.queryMessageFromDrugShop.Expect(call => call.Query()).Return(objectMother.messageList.AsQueryable());

            //Act
            var result = objectMother.controller.GetSymptomsReport(model);

            //Assert
            objectMother.querySymptoms.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDrugShop.VerifyAllExpectations();

            Assert.IsInstanceOf<SymptomsReportIndexOutputModel>(result.Data);
            var jsonData = result.Data as SymptomsReportIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(4, jsonData.TotalItems);
        }

        [Test]
        public void Returns_JSONData_FilteredBy_StartDate()
        {
            //Arrange
            SymptomsIndexModel model = new SymptomsIndexModel
            {
                startDate = DateTime.UtcNow.AddMonths(-3).ToString()
            };
            objectMother.querySymptoms.Expect(call => call.Query()).Return(objectMother.symptomsList.AsQueryable());
            objectMother.queryOutpost.Expect(call => call.Query()).Return(objectMother.outpostList.AsQueryable());
            objectMother.queryMessageFromDrugShop.Expect(call => call.Query()).Return(objectMother.messageList.AsQueryable());

            //Act
            var result = objectMother.controller.GetSymptomsReport(model);

            //Assert
            objectMother.querySymptoms.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDrugShop.VerifyAllExpectations();

            Assert.IsInstanceOf<SymptomsReportIndexOutputModel>(result.Data);
            var jsonData = result.Data as SymptomsReportIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(8, jsonData.TotalItems);
        }

        [Test]
        public void Returns_JSONData_FilteredBy_EndDate()
        {
            //Arrange
            SymptomsIndexModel model = new SymptomsIndexModel
            {
                endDate = DateTime.UtcNow.AddMonths(-3).ToString()
            };
            objectMother.querySymptoms.Expect(call => call.Query()).Return(objectMother.symptomsList.AsQueryable());
            objectMother.queryOutpost.Expect(call => call.Query()).Return(objectMother.outpostList.AsQueryable());
            objectMother.queryMessageFromDrugShop.Expect(call => call.Query()).Return(objectMother.messageList.AsQueryable());

            //Act
            var result = objectMother.controller.GetSymptomsReport(model);

            //Assert
            objectMother.querySymptoms.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDrugShop.VerifyAllExpectations();

            Assert.IsInstanceOf<SymptomsReportIndexOutputModel>(result.Data);
            var jsonData = result.Data as SymptomsReportIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(8, jsonData.TotalItems);
        }


    }
}
