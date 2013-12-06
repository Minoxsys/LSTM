using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Web.Areas.MessagesManagement.Models.Messages;
using Domain;
using Core.Persistence;
using Web.Security;

namespace Web.Areas.MessagesManagement.Controllers
{
    public class DispensaryController : Controller
    {
        public IQueryService<RawSmsReceived> QueryRawSms { get; set; }
        public IQueryService<Outpost> QueryOutpost { get; set; }

        [HttpGet]
        [Requires(Permissions = "Messages.View")]
        public ActionResult Overview()
        {
            return View("Overview");
        }

        [HttpGet]
        public JsonResult GetMessagesFromDispensary(MessagesIndexModel indexModel)
        {
            var pageSize = indexModel.limit.Value;
            var rawDataQuery = this.QueryRawSms.Query().Where(it => it.OutpostType == 1);

            var orderByColumnDirection = new Dictionary<string, Func<IQueryable<RawSmsReceived>>>()
            {
                { "Sender-ASC", () => rawDataQuery.OrderBy(c => c.Sender) },
                { "Sender-DESC", () => rawDataQuery.OrderByDescending(c => c.Sender) },
                { "Date-ASC", () => rawDataQuery.OrderBy(c => c.ReceivedDate) },
                { "Date-DESC", () => rawDataQuery.OrderByDescending(c => c.ReceivedDate) },
                { "Content-ASC", () => rawDataQuery.OrderBy(c => c.Content) },
                { "Content-DESC", () => rawDataQuery.OrderByDescending(c => c.Content) },
                { "ParseSucceeded-ASC", () => rawDataQuery.OrderBy(c => c.ParseSucceeded) },
                { "ParseSucceeded-DESC", () => rawDataQuery.OrderByDescending(c => c.ParseSucceeded) },
                { "ParseErrorMessage-ASC", () => rawDataQuery.OrderBy(c => c.ParseErrorMessage) },
                { "ParseErrorMessage-DESC", () => rawDataQuery.OrderByDescending(c => c.ParseErrorMessage) }
            };

            rawDataQuery = orderByColumnDirection[String.Format("{0}-{1}", indexModel.sort, indexModel.dir)].Invoke();

            if (!string.IsNullOrEmpty(indexModel.searchValue))
                rawDataQuery = rawDataQuery.Where(it => it.Content.Contains(indexModel.searchValue));

            var totalItems = rawDataQuery.Count();

            rawDataQuery = rawDataQuery
                .Take(pageSize)
                .Skip(indexModel.start.Value);

            var messagesModelListProjection = GetAllMessages();

            return Json(new MessageIndexOuputModel
            {
                Messages = messagesModelListProjection,
                TotalItems = totalItems
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public void ExportToExcel()
        {
            Response.Clear();
            Response.ContentType = "application/vnd.xls";
            Response.AddHeader("Content-disposition",
                "attachment; filename=" + "MessagesReceivedFromShopADDO" + DateTime.UtcNow.ToShortDateString() + ".xls");

            var reportData = GetAllMessages();

            using (ExcelPackage pck = new ExcelPackage())
            {
                //Create the worksheet
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Messages from Drugshop");

                //add header
                ws.Cells[1, 1].Value = "Phone Number";
                ws.Cells[1, 2].Value = "Dispensary";
                ws.Cells[1, 3].Value = "Date";
                ws.Cells[1, 4].Value = "Content";
                ws.Cells[1, 5].Value = "Is Message Correct";
                ws.Cells[1, 6].Value = "Error Messages";

                //load data
                for (int i = 0; i < reportData.Length; i++)
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

        private MessageModel[] GetAllMessages()
        {
            return (from message in QueryRawSms.Query().ToList()
                    select new MessageModel
                    {
                        Sender = message.Sender,
                        Date = message.ReceivedDate.ToString("dd/MM/yyyy HH:mm"),
                        Content = message.Content,
                        ParseSucceeded = message.ParseSucceeded,
                        ParseErrorMessage = message.ParseErrorMessage,
                        OutpostName = QueryOutpost.Load(message.OutpostId) != null ? QueryOutpost.Load(message.OutpostId).Name : null
                    }).ToArray();
        }

    }
}
