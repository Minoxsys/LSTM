using System;
using System.Linq;
using Rhino.Mocks;
using NUnit.Framework;
using Web.Areas.AnalysisManagement.Models.SymptomsReport;

namespace Tests.Unit.Controllers.Areas.AnalysisManagement.SymptomsReportControllerTests
{
    [TestFixture]
    public class GetChartDataMethod
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
            var result = objectMother.controller.GetChartData(model);

            //Assert
            objectMother.querySymptoms.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDrugShop.VerifyAllExpectations();

            Assert.IsInstanceOf<SymptomsReportIndexOutputModel>(result.Data);
            var jsonData = result.Data as SymptomsReportIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(4, jsonData.TotalItems);
            Assert.AreEqual(0, jsonData.Symptoms[0].Female); Assert.AreEqual(2, jsonData.Symptoms[0].Male); Assert.AreEqual(2, jsonData.Symptoms[0].NumberOfPatients);
            Assert.AreEqual(2, jsonData.Symptoms[1].Female); Assert.AreEqual(0, jsonData.Symptoms[1].Male); Assert.AreEqual(2, jsonData.Symptoms[1].NumberOfPatients);
            Assert.AreEqual(2, jsonData.Symptoms[3].Female); Assert.AreEqual(0, jsonData.Symptoms[3].Male); Assert.AreEqual(2, jsonData.Symptoms[3].NumberOfPatients);
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
            var result = objectMother.controller.GetChartData(model);

            //Assert
            objectMother.querySymptoms.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDrugShop.VerifyAllExpectations();

            Assert.IsInstanceOf<SymptomsReportIndexOutputModel>(result.Data);
            var jsonData = result.Data as SymptomsReportIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(4, jsonData.TotalItems);

            Assert.AreEqual(0, jsonData.Symptoms[0].Female); Assert.AreEqual(1, jsonData.Symptoms[0].Male); Assert.AreEqual(1, jsonData.Symptoms[0].NumberOfPatients);
            Assert.AreEqual(1, jsonData.Symptoms[1].Female); Assert.AreEqual(0, jsonData.Symptoms[1].Male); Assert.AreEqual(1, jsonData.Symptoms[1].NumberOfPatients);
            Assert.AreEqual(1, jsonData.Symptoms[3].Female); Assert.AreEqual(0, jsonData.Symptoms[3].Male); Assert.AreEqual(1, jsonData.Symptoms[3].NumberOfPatients);
        }

        [Test]
        public void Returns_JSONData_FilteredBy_EndDate()
        {
            //Arrange
            SymptomsIndexModel model = new SymptomsIndexModel
            {
                endDate = DateTime.UtcNow.AddMonths(-5).ToString()
            };
            objectMother.querySymptoms.Expect(call => call.Query()).Return(objectMother.symptomsList.AsQueryable());
            objectMother.queryOutpost.Expect(call => call.Query()).Return(objectMother.outpostList.AsQueryable());
            objectMother.queryMessageFromDrugShop.Expect(call => call.Query()).Return(objectMother.messageList.AsQueryable());

            //Act
            var result = objectMother.controller.GetChartData(model);

            //Assert
            objectMother.querySymptoms.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDrugShop.VerifyAllExpectations();

            Assert.IsInstanceOf<SymptomsReportIndexOutputModel>(result.Data);
            var jsonData = result.Data as SymptomsReportIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(4, jsonData.TotalItems);
            Assert.AreEqual(0, jsonData.Symptoms[0].Female); Assert.AreEqual(0, jsonData.Symptoms[0].Male); Assert.AreEqual(0, jsonData.Symptoms[0].NumberOfPatients);
            Assert.AreEqual(1, jsonData.Symptoms[1].Female); Assert.AreEqual(0, jsonData.Symptoms[1].Male); Assert.AreEqual(1, jsonData.Symptoms[1].NumberOfPatients);
            Assert.AreEqual(1, jsonData.Symptoms[3].Female); Assert.AreEqual(0, jsonData.Symptoms[3].Male); Assert.AreEqual(1, jsonData.Symptoms[3].NumberOfPatients);
        }


    }
}
