using NUnit.Framework;

namespace Tests.Unit.Controllers.Areas.AnalysisManagement
{
    public class ExcelHelperTests
    {
        private readonly ExcelHelper _excelHelper = new ExcelHelper();

        [Test]
        public void ParseExcel_Should_Parse_AValid_String()
        {
            //Arrange
            string content = "Excel\t Test\t\n"+
                             "This is\t another\t line";

            //Act
            var exc = _excelHelper.ParseExcel(content);

            //Assert
            Assert.AreEqual("Excel", exc[0][0]);
            Assert.AreEqual("Test", exc[0][1]);
            Assert.AreEqual("This is", exc[1][0]);
            Assert.AreEqual("line", exc[1][2]);
        }
    }
}