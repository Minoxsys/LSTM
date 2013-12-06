using System;
using System.Drawing;
using System.Linq;
using System.Web.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Web.Services;

namespace Web.Areas.MessagesManagement.Controllers
{
    public class ExcelExportController : Controller
    {
        public IMessagesService MessagesService { get; set; }

        [HttpPost]
        public void ExportToExcel(int outpostType)
        {
            string fileName, sheetName, outpostColumnName;
            switch (outpostType)
            {
                case 0:
                    fileName = "MessagesReceivedFromDrugShop";
                    sheetName = "Messages from Drug Shop";
                    outpostColumnName = "Shop/ADDO";
                    break;
                case 1:
                    fileName = "MessagesReceivedFromDispensary";
                    sheetName = "Messages from Dispensary";
                    outpostColumnName = "Dispensary";
                    break;
                case 2:
                    fileName = "MessagesReceivedFromHealthCenter";
                    sheetName = "Messages from Health Center";
                    outpostColumnName = "Health Center";
                    break;
                default:
                    fileName = "Messages";
                    sheetName = "Unknown";
                    outpostColumnName = "Outpost";
                    break;
            }

            Response.Clear();
            Response.ContentType = "application/vnd.xls";
            Response.AddHeader("Content-disposition",
                "attachment; filename=" + fileName + DateTime.UtcNow.ToShortDateString() + ".xls");

            var reportData = MessagesService.GetMessagesFromOutpost(null, outpostType).Messages;

            using (ExcelPackage pck = new ExcelPackage())
            {
                //Create the worksheet
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add(sheetName);

                //add header
                ws.Cells[1, 1].Value = "Phone Number";
                ws.Cells[1, 2].Value = outpostColumnName;
                ws.Cells[1, 3].Value = "Date";
                ws.Cells[1, 4].Value = "Content";
                ws.Cells[1, 5].Value = "Is Message Correct";
                ws.Cells[1, 6].Value = "Error Messages";

                //load data
                for (int i = 0; i < reportData.Count(); i++)
                {
                    ws.Cells[i + 2, 1].Value = reportData[i].Sender;

                    ws.Cells[i + 2, 2].Value = reportData[i].OutpostName;
                    ws.Cells[i + 2, 3].Value = reportData[i].Date;
                    ws.Cells[i + 2, 4].Value = reportData[i].Content;
                    ws.Cells[i + 2, 5].Value = reportData[i].ParseSucceeded ? "Yes" : "No";
                    ws.Cells[i + 2, 6].Value = reportData[i].ParseErrorMessage;

                }
                ws.Column(1).Style.Numberformat.Format = "0";

                ws.Cells.AutoFitColumns();

                //Format the header
                using (ExcelRange rng = ws.Cells["A1:F1"])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(76, 126, 44));
                    rng.Style.Font.Color.SetColor(Color.White);
                }

                Response.BinaryWrite(pck.GetAsByteArray());
            }

            Response.End();
        }
    }
}
