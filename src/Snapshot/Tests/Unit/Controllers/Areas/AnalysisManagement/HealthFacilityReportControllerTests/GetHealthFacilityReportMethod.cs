using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Web.Areas.AnalysisManagement.Models.HealthFacilityReport;
using Rhino.Mocks;
using Domain;

namespace Tests.Unit.Controllers.Areas.AnalysisManagement.HealthFacilityReportControllerTests
{
    [TestFixture]
    public class GetHealthFacilityReportMethod
    {
        public ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test]
        public void Returns_The_Data_Paginated()
        {
            //Arange
            var indexModel = new HealthFacilityIndexModel
            {
                dir = "ASC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "OutpostName"
            };
            var outpostList = objectMother.GetOutpostList(indexModel);
            objectMother.queryOutpost.Expect(call => call.Query()).Return(outpostList);

            //Act
            var jsonResult = objectMother.controller.GetHealthFacilityReport(indexModel);

            //Assert
            objectMother.queryOutpost.VerifyAllExpectations();

            Assert.IsInstanceOf<HealthFacilityIndexOutputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as HealthFacilityIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(outpostList.Count(), jsonData.TotalItems);
        }

        [Test]
        public void Returns_The_Data_Paginated_ForSelectedCountry()
        {
            //Arange
            var indexModel = new HealthFacilityIndexModel
            {
                dir = "ASC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "OutpostName",
                countryId = objectMother.countryId.ToString()

            };
            var outpostList = objectMother.GetOutpostList(indexModel);
            objectMother.queryOutpost.Expect(call => call.Query()).Return(outpostList);

            //Act
            var jsonResult = objectMother.controller.GetHealthFacilityReport(indexModel);

            //Assert
            objectMother.queryOutpost.VerifyAllExpectations();

            Assert.IsInstanceOf<HealthFacilityIndexOutputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as HealthFacilityIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(outpostList.Count()/5, jsonData.TotalItems);
        }

        [Test]
        public void Returns_The_Data_Paginated_ForSelectedCountry_andRegion()
        {
            //Arange
            var indexModel = new HealthFacilityIndexModel
            {
                dir = "ASC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "OutpostName",
                countryId = objectMother.countryId.ToString(),
                regionId = objectMother.regionId.ToString()

            };
            var outpostList = objectMother.GetOutpostList(indexModel);
            objectMother.queryOutpost.Expect(call => call.Query()).Return(outpostList);

            //Act
            var jsonResult = objectMother.controller.GetHealthFacilityReport(indexModel);

            //Assert
            objectMother.queryOutpost.VerifyAllExpectations();

            Assert.IsInstanceOf<HealthFacilityIndexOutputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as HealthFacilityIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(outpostList.Count() / 10, jsonData.TotalItems);
        }

        [Test]
        public void Returns_The_Data_Paginated_ForSelectedCountry_andRegion_andDistrict()
        {
            //Arange
            var indexModel = new HealthFacilityIndexModel
            {
                dir = "ASC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "OutpostName",
                countryId = objectMother.countryId.ToString(),
                regionId = objectMother.regionId.ToString(),
                districtId = objectMother.districtId.ToString()

            };
            var outpostList = objectMother.GetOutpostList(indexModel);
            objectMother.queryOutpost.Expect(call => call.Query()).Return(outpostList);

            //Act
            var jsonResult = objectMother.controller.GetHealthFacilityReport(indexModel);

            //Assert
            objectMother.queryOutpost.VerifyAllExpectations();

            Assert.IsInstanceOf<HealthFacilityIndexOutputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as HealthFacilityIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(outpostList.Count() / 20 + 1, jsonData.TotalItems);
        }

        [Test]
        public void Returns_The_Data_Paginated_ForSelectedOutpostType()
        {
            //Arange
            var indexModel = new HealthFacilityIndexModel
            {
                dir = "ASC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "OutpostName",
                outpostType = "0"

            };
            var outpostList = objectMother.GetOutpostList(indexModel);
            objectMother.queryOutpost.Expect(call => call.Query()).Return(outpostList);
            objectMother.queryMessageFromDrugShop.Expect(call => call.Query()).Return(new MessageFromDrugShop[] { } .AsQueryable() );

            //Act
            var jsonResult = objectMother.controller.GetHealthFacilityReport(indexModel);

            //Assert
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDrugShop.VerifyAllExpectations();

            Assert.IsInstanceOf<HealthFacilityIndexOutputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as HealthFacilityIndexOutputModel;
            Assert.IsNotNull(jsonData);

            Assert.AreEqual(outpostList.Count() / 3 + 1, jsonData.TotalItems);
        }

        [Test]
        public void Returns_The_Data_Paginated_ForSelectedOutpostType_ReturnsNumberOfPatients()
        {
            //Arange
            var indexModel = new HealthFacilityIndexModel
            {
                dir = "ASC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "OutpostName",
                outpostType = "0"

            };
            var outpostList = objectMother.GetOutpostList(indexModel);
            var messageList = objectMother.GetMessageFromDrugShopList(indexModel);
            objectMother.queryOutpost.Expect(call => call.Query()).Return(outpostList);
            objectMother.queryMessageFromDrugShop.Expect(call => call.Query()).Return(messageList);

            //Act
            var jsonResult = objectMother.controller.GetHealthFacilityReport(indexModel);

            //Assert
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDrugShop.VerifyAllExpectations();

            Assert.IsInstanceOf<HealthFacilityIndexOutputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as HealthFacilityIndexOutputModel;
            Assert.IsNotNull(jsonData);
            Assert.AreEqual(outpostList.Count() / 3 + 1, jsonData.TotalItems);

            Assert.AreEqual((messageList.Count() - messageList.Count()/ 3).ToString(), jsonData.Outposts[0].Female);
            Assert.AreEqual((messageList.Count() / 3).ToString(), jsonData.Outposts[0].Male);
            Assert.AreEqual(messageList.Count().ToString(), jsonData.Outposts[0].NumberOfPatients);
        }

        [Test]
        public void Returns_The_Data_Paginated_ForSelectedOutpostType_andStartDate()
        {
            //Arange
            var indexModel = new HealthFacilityIndexModel
            {
                dir = "ASC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "OutpostName",
                outpostType = "0",
                startDate = DateTime.UtcNow.AddMonths(-10).ToShortDateString()

            };
            var outpostList = objectMother.GetOutpostList(indexModel);
            var messageList = objectMother.GetMessageFromDrugShopList(indexModel);
            objectMother.queryOutpost.Expect(call => call.Query()).Return(outpostList);
            objectMother.queryMessageFromDrugShop.Expect(call => call.Query()).Return(messageList);

            //Act
            var jsonResult = objectMother.controller.GetHealthFacilityReport(indexModel);

            //Assert
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDrugShop.VerifyAllExpectations();

            Assert.IsInstanceOf<HealthFacilityIndexOutputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as HealthFacilityIndexOutputModel;
            Assert.IsNotNull(jsonData);
            Assert.AreEqual(outpostList.Count() / 3 + 1, jsonData.TotalItems);

            Assert.AreEqual("7", jsonData.Outposts[0].Female);
            Assert.AreEqual("3", jsonData.Outposts[0].Male);
            Assert.AreEqual((messageList.Count()/5).ToString(), jsonData.Outposts[0].NumberOfPatients);
        }

        [Test]
        public void Returns_The_Data_Paginated_ForSelectedOutpostType_andStartDate_andEndDate()
        {
            //Arange
            var indexModel = new HealthFacilityIndexModel
            {
                dir = "ASC",
                limit = 50,
                page = 1,
                start = 0,
                sort = "OutpostName",
                outpostType = "0",
                startDate = DateTime.UtcNow.AddMonths(-10).ToShortDateString(),
                endDate = DateTime.UtcNow.AddMonths(-8).ToShortDateString()

            };
            var outpostList = objectMother.GetOutpostList(indexModel);
            var messageList = objectMother.GetMessageFromDrugShopList(indexModel);
            objectMother.queryOutpost.Expect(call => call.Query()).Return(outpostList);
            objectMother.queryMessageFromDrugShop.Expect(call => call.Query()).Return(messageList);

            //Act
            var jsonResult = objectMother.controller.GetHealthFacilityReport(indexModel);

            //Assert
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDrugShop.VerifyAllExpectations();

            Assert.IsInstanceOf<HealthFacilityIndexOutputModel>(jsonResult.Data);
            var jsonData = jsonResult.Data as HealthFacilityIndexOutputModel;
            Assert.IsNotNull(jsonData);
            Assert.AreEqual(outpostList.Count() / 3 + 1, jsonData.TotalItems);

            Assert.AreEqual("1", jsonData.Outposts[0].Female);
            Assert.AreEqual("1", jsonData.Outposts[0].Male);
            Assert.AreEqual((messageList.Count() / 25).ToString(), jsonData.Outposts[0].NumberOfPatients);
        }
    }
}
