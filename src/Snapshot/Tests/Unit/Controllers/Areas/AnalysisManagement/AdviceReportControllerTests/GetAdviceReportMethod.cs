using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;
using Rhino.Mocks;
using NUnit.Framework;
using Web.Areas.AnalysisManagement.Models.AdviceReport;


namespace Tests.Unit.Controllers.Areas.AnalysisManagement.AdviceReportControllerTests
{
    [TestFixture]
    public class GetAdviceReportMethod
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
            AdviceIndexModel model = new AdviceIndexModel();
            objectMother.queryAdvice.Expect(call => call.Query()).Return(objectMother.adviceList.AsQueryable());
            objectMother.queryOutpost.Expect(call => call.Query()).Return(objectMother.outpostList.AsQueryable());
            objectMother.queryMessageFromDispensary.Expect(call => call.Query()).Return(objectMother.messageList.AsQueryable());

            //Act
            var result = objectMother.controller.GetAdviceReport(model);

            //Assert
            objectMother.queryAdvice.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDispensary.VerifyAllExpectations();

            Assert.IsInstanceOf<AdviceReportIndexOutputModel>(result.Data);
            var jsonData = result.Data as AdviceReportIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(12, jsonData.TotalItems);
        }

        [Test]
        public void Returns_JSONData_ForCountry()
        {
            //Arrange
            AdviceIndexModel model = new AdviceIndexModel
            {
                countryId = objectMother.countryId.ToString()
            };
            objectMother.queryAdvice.Expect(call => call.Query()).Return(objectMother.adviceList.AsQueryable());
            objectMother.queryOutpost.Expect(call => call.Query()).Return(objectMother.outpostList.AsQueryable());
            objectMother.queryMessageFromDispensary.Expect(call => call.Query()).Return(objectMother.messageList.AsQueryable());

            //Act
            var result = objectMother.controller.GetAdviceReport(model);

            //Assert
            objectMother.queryAdvice.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDispensary.VerifyAllExpectations();

            Assert.IsInstanceOf<AdviceReportIndexOutputModel>(result.Data);
            var jsonData = result.Data as AdviceReportIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(12, jsonData.TotalItems);
        }

        [Test]
        public void Returns_JSONData_ForCountry_Region()
        {
            //Arrange
            AdviceIndexModel model = new AdviceIndexModel
            {
                countryId = objectMother.countryId.ToString(),
                regionId = objectMother.regionId.ToString()
            };
            objectMother.queryAdvice.Expect(call => call.Query()).Return(objectMother.adviceList.AsQueryable());
            objectMother.queryOutpost.Expect(call => call.Query()).Return(objectMother.outpostList.AsQueryable());
            objectMother.queryMessageFromDispensary.Expect(call => call.Query()).Return(objectMother.messageList.AsQueryable());

            //Act
            var result = objectMother.controller.GetAdviceReport(model);

            //Assert
            objectMother.queryAdvice.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDispensary.VerifyAllExpectations();

            Assert.IsInstanceOf<AdviceReportIndexOutputModel>(result.Data);
            var jsonData = result.Data as AdviceReportIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(8, jsonData.TotalItems);
        }

        [Test]
        public void Returns_JSONData_ForCountry_Region_District()
        {
            //Arrange
            AdviceIndexModel model = new AdviceIndexModel
            {
                countryId = objectMother.countryId.ToString(),
                regionId = objectMother.regionId.ToString(),
                districtId = objectMother.districtId.ToString()
            };
            objectMother.queryAdvice.Expect(call => call.Query()).Return(objectMother.adviceList.AsQueryable());
            objectMother.queryOutpost.Expect(call => call.Query()).Return(objectMother.outpostList.AsQueryable());
            objectMother.queryMessageFromDispensary.Expect(call => call.Query()).Return(objectMother.messageList.AsQueryable());

            //Act
            var result = objectMother.controller.GetAdviceReport(model);

            //Assert
            objectMother.queryAdvice.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDispensary.VerifyAllExpectations();

            Assert.IsInstanceOf<AdviceReportIndexOutputModel>(result.Data);
            var jsonData = result.Data as AdviceReportIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(4, jsonData.TotalItems);
        }

        [Test]
        public void Returns_JSONData_FilteredBy_StartDate()
        {
            //Arrange
            AdviceIndexModel model = new AdviceIndexModel
            {
                startDate = DateTime.UtcNow.AddMonths(-3).ToString()
            };
            objectMother.queryAdvice.Expect(call => call.Query()).Return(objectMother.adviceList.AsQueryable());
            objectMother.queryOutpost.Expect(call => call.Query()).Return(objectMother.outpostList.AsQueryable());
            objectMother.queryMessageFromDispensary.Expect(call => call.Query()).Return(objectMother.messageList.AsQueryable());

            //Act
            var result = objectMother.controller.GetAdviceReport(model);

            //Assert
            objectMother.queryAdvice.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDispensary.VerifyAllExpectations();

            Assert.IsInstanceOf<AdviceReportIndexOutputModel>(result.Data);
            var jsonData = result.Data as AdviceReportIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(12, jsonData.TotalItems);
            //Assert.AreEqual(1, jsonData.Advice[0].Female);
            //Assert.AreEqual(0, jsonData.Advice[0].Male);
            //Assert.AreEqual(1, jsonData.Advice[0].NumberOfPatients);
        }

        [Test]
        public void Returns_JSONData_FilteredBy_EndDate()
        {
            //Arrange
            AdviceIndexModel model = new AdviceIndexModel
            {
                endDate = DateTime.UtcNow.AddMonths(-3).ToString()
            };
            objectMother.queryAdvice.Expect(call => call.Query()).Return(objectMother.adviceList.AsQueryable());
            objectMother.queryOutpost.Expect(call => call.Query()).Return(objectMother.outpostList.AsQueryable());
            objectMother.queryMessageFromDispensary.Expect(call => call.Query()).Return(objectMother.messageList.AsQueryable());

            //Act
            var result = objectMother.controller.GetAdviceReport(model);

            //Assert
            objectMother.queryAdvice.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDispensary.VerifyAllExpectations();

            Assert.IsInstanceOf<AdviceReportIndexOutputModel>(result.Data);
            var jsonData = result.Data as AdviceReportIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(12, jsonData.TotalItems);
            //Assert.AreEqual(1, jsonData.Advice[1].Female);
            //Assert.AreEqual(0, jsonData.Advice[1].Male);
            //Assert.AreEqual(1, jsonData.Advice[1].NumberOfPatients);
        }


    }
}
