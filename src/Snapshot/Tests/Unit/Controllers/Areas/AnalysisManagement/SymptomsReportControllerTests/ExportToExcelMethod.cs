using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using Rhino.Mocks;
using NUnit.Framework;
using Web.Areas.AnalysisManagement.Models.SymptomsReport;

namespace Tests.Unit.Controllers.Areas.AnalysisManagement.SymptomsReportControllerTests
{
    [TestFixture]
    public class ExportToExcelMethod
    {
        public ObjectMother objectMother = new ObjectMother();
        private ExcelHelper _excelHelper= new ExcelHelper();

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

            var response = new Mock<HttpResponseBase>();
            var context = new Mock<HttpContextBase>();
            var stream = new MemoryStream();


            context.Expect(ctx => ctx.Response).Returns(response.Object);
            context.Expect(ctx => ctx.Response.OutputStream).Returns(stream);
            context.Expect(ctx => ctx.User).Returns(objectMother.User);
            
            objectMother.controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), objectMother.controller);

            //Act
            objectMother.controller.ExportToExcel(model);

            //Assert
            objectMother.querySymptoms.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDrugShop.VerifyAllExpectations();

            var res = objectMother.controller.Response;
            string str=Encoding.UTF8.GetString(stream.GetBuffer());
            var exc = _excelHelper.ParseExcel(str);

            Assert.AreEqual(_excelHelper.ExcelContentType, res.ContentType);
            
            Assert.AreEqual("Country:", exc[0][0]); Assert.AreEqual("", exc[0][1]);
            Assert.AreEqual("Region:", exc[1][0]); Assert.AreEqual("", exc[1][1]);
            Assert.AreEqual("Start date:", exc[3][0]); Assert.AreEqual("", exc[3][1]);

            Assert.AreEqual(objectMother.symptomsList[0].Code, exc[8][0]); 
                Assert.AreEqual("0", exc[8][2]); Assert.AreEqual("2", exc[8][3]); Assert.AreEqual("2", exc[8][4]);
            Assert.AreEqual(objectMother.symptomsList[2].Code, exc[14][0]); 
                Assert.AreEqual("1", exc[14][2]); Assert.AreEqual("0", exc[14][3]); Assert.AreEqual("1", exc[14][4]);
            Assert.AreEqual(objectMother.symptomsList[3].Code, exc[17][0]); 
                Assert.AreEqual("2", exc[17][2]); Assert.AreEqual("0", exc[17][3]); Assert.AreEqual("2", exc[17][4]);

            Assert.AreEqual(objectMother.outpostList[0].Name+" (" + objectMother.outpostList[0].District.Name+ ")", exc[9][1]); 
                Assert.AreEqual("0", exc[9][2]); Assert.AreEqual("1", exc[9][3]); Assert.AreEqual("1", exc[9][4]);
        }

        [Test]
        public void Returns_Data_FilteredBy_Region()
        {
            //Arrange
            SymptomsIndexModel model = new SymptomsIndexModel
                                           {
                                               regionId = objectMother.regionId.ToString()
                                           };
            objectMother.querySymptoms.Expect(call => call.Query()).Return(objectMother.symptomsList.AsQueryable());
            objectMother.queryOutpost.Expect(call => call.Query()).Return(objectMother.outpostList.AsQueryable());
            objectMother.queryMessageFromDrugShop.Expect(call => call.Query()).Return(objectMother.messageList.AsQueryable());
            objectMother.queryRegion.Expect(call => call.Load(objectMother.regionId)).Return(objectMother.region);

            var response = new Mock<HttpResponseBase>();
            var context = new Mock<HttpContextBase>();
            var stream = new MemoryStream();


            context.Expect(ctx => ctx.Response).Returns(response.Object);
            context.Expect(ctx => ctx.Response.OutputStream).Returns(stream);
            context.Expect(ctx => ctx.User).Returns(objectMother.User);
            
            objectMother.controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), objectMother.controller);

            //Act
            objectMother.controller.ExportToExcel(model);

            //Assert
            objectMother.querySymptoms.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDrugShop.VerifyAllExpectations();
            objectMother.queryRegion.VerifyAllExpectations();

            var res = objectMother.controller.Response;
            string str=Encoding.UTF8.GetString(stream.GetBuffer());
            var exc = _excelHelper.ParseExcel(str);

            Assert.AreEqual(_excelHelper.ExcelContentType, res.ContentType);
            
            Assert.AreEqual("Country:", exc[0][0]); Assert.AreEqual("", exc[0][1]);
            Assert.AreEqual("Region:", exc[1][0]); Assert.AreEqual(objectMother.region.Name, exc[1][1]);
            Assert.AreEqual("Start date:", exc[3][0]); Assert.AreEqual("", exc[3][1]);

            Assert.AreEqual(objectMother.symptomsList[0].Code, exc[8][0]); 
                Assert.AreEqual("0", exc[8][2]); Assert.AreEqual("2", exc[8][3]); Assert.AreEqual("2", exc[8][4]);
            Assert.AreEqual(objectMother.symptomsList[2].Code, exc[14][0]); 
                Assert.AreEqual("1", exc[14][2]); Assert.AreEqual("0", exc[14][3]); Assert.AreEqual("1", exc[14][4]);
            Assert.AreEqual(objectMother.symptomsList[3].Code, exc[17][0]); 
                Assert.AreEqual("2", exc[17][2]); Assert.AreEqual("0", exc[17][3]); Assert.AreEqual("2", exc[17][4]);

            Assert.AreEqual(objectMother.outpostList[0].Name+" (" + objectMother.outpostList[0].District.Name+ ")", exc[9][1]); 
                Assert.AreEqual("0", exc[9][2]); Assert.AreEqual("1", exc[9][3]); Assert.AreEqual("1", exc[9][4]);
        }
    }
}
