using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Web.Areas.AnalysisManagement.Models.SummaryReport;
using Rhino.Mocks;

namespace Tests.Unit.Controllers.Areas.AnalysisManagement.SummaryReportControllerTests
{
    [TestFixture]
    public class GetSummaryReportMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void Returns_JSON_With_SummaryReportData()
        {
            //Arrange
            SummaryIndexModel model = new SummaryIndexModel();
            objectMother.queryOutpost.Expect(call => call.Query()).Return(objectMother.outpostList.AsQueryable());
            objectMother.queryMessageFromDispensary.Expect(call => call.Query()).Return(objectMother.messageFromDispensaryList.AsQueryable());
            objectMother.queryMessageFromDrugShop.Expect(call => call.Query()).Return(objectMother.messageFromDrugShopList.AsQueryable());

            //Act
            var result = objectMother.controller.GetSummaryReport(model);

            //Assert
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDispensary.VerifyAllExpectations();
            objectMother.queryMessageFromDrugShop.VerifyAllExpectations();

            Assert.IsInstanceOf<SummaryReportIndexOutputModel>(result.Data);
            var jsonData = result.Data as SummaryReportIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(3, jsonData.TotalItems);
            Assert.AreEqual(4, jsonData.Patients[0].DistinctPatients);
            Assert.AreEqual(1, jsonData.Patients[0].FailedToReport);
            Assert.AreEqual(1, jsonData.Patients[0].NotTreated);
            Assert.AreEqual(2, jsonData.Patients[0].Treated);
        }
    }
}
