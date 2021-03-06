﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using Rhino.Mocks;
using NUnit.Framework;
using Web.Areas.AnalysisManagement.Models.DiagnosisReport;

namespace Tests.Unit.Controllers.Areas.AnalysisManagement.DiagnosisReportControllerTests
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
            DiagnosisIndexModel model = new DiagnosisIndexModel();
            objectMother.queryDiagnosis.Expect(call => call.Query()).Return(objectMother.diagnosisList.AsQueryable());
            objectMother.queryOutpost.Expect(call => call.Query()).Return(objectMother.outpostList.AsQueryable());
            objectMother.queryMessageFromDispensary.Expect(call => call.Query()).Return(objectMother.messageList.AsQueryable());

            var response = new Mock<HttpResponseBase>();
            var context = new Mock<HttpContextBase>();
            var stream = new MemoryStream();


            context.Setup(ctx => ctx.Response).Returns(response.Object);
            context.Setup(ctx => ctx.Response.OutputStream).Returns(stream);
            context.Setup(ctx => ctx.User).Returns(objectMother.User);
            
            objectMother.controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), objectMother.controller);

            //Act
            objectMother.controller.ExportToExcel(model);

            //Assert
            objectMother.queryDiagnosis.VerifyAllExpectations();
            objectMother.queryOutpost.VerifyAllExpectations();
            objectMother.queryMessageFromDispensary.VerifyAllExpectations();

            var res = objectMother.controller.Response;
            string str=Encoding.UTF8.GetString(stream.GetBuffer());
            var exc = _excelHelper.ParseExcel(str);

            Assert.AreEqual(_excelHelper.ExcelContentType, res.ContentType);
            
            Assert.AreEqual("Country:", exc[0][0]); Assert.AreEqual("", exc[0][1]);
            Assert.AreEqual("Region:", exc[1][0]); Assert.AreEqual("", exc[1][1]);
            Assert.AreEqual("Start date:", exc[3][0]); Assert.AreEqual("", exc[3][1]);

            Assert.AreEqual(objectMother.diagnosisList[0].Code, exc[8][0]); 
                Assert.AreEqual("0", exc[8][2]); Assert.AreEqual("3", exc[8][3]); Assert.AreEqual("3", exc[8][4]);
            Assert.AreEqual(objectMother.diagnosisList[1].Code, exc[12][0]); 
                Assert.AreEqual("3", exc[12][2]); Assert.AreEqual("0", exc[12][3]); Assert.AreEqual("3", exc[12][4]);
            Assert.AreEqual(objectMother.diagnosisList[3].Code, exc[20][0]); 
                Assert.AreEqual("2", exc[20][2]); Assert.AreEqual("0", exc[20][3]); Assert.AreEqual("2", exc[20][4]);

            Assert.AreEqual(objectMother.outpostList[0].Name+" (" + objectMother.outpostList[0].District.Name+ ")", exc[9][1]); 
                Assert.AreEqual("0", exc[9][2]); Assert.AreEqual("1", exc[9][3]); Assert.AreEqual("1", exc[9][4]);
        }
    }
}
