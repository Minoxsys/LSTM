using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.Persistence;
using Domain;
using Web.Areas.MessagesManagement.Models.Messages;
using Web.Security;

namespace Web.Areas.MessagesManagement.Controllers
{
    public class HealthCenterController : Controller
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
        public JsonResult GetMessagesFromHealthCenter(MessagesIndexModel indexModel)
        {
            var pageSize = indexModel.limit.Value;
            var rawDataQuery = this.QueryRawSms.Query().Where(it => it.OutpostType == 2);

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

            var messagesModelListProjection = (from message in rawDataQuery.ToList()
                                               select new MessageModel
                                               {
                                                   Sender = message.Sender,
                                                   Date = message.ReceivedDate.ToString("dd/MM/yyyy"),
                                                   Content = message.Content,
                                                   ParseSucceeded = message.ParseSucceeded,
                                                   ParseErrorMessage = message.ParseErrorMessage,
                                                   OutpostName = QueryOutpost.Load(message.OutpostId) != null ? QueryOutpost.Load(message.OutpostId).Name : null
                                               }).ToArray();


            return Json(new MessageIndexOuputModel
            {
                Messages = messagesModelListProjection,
                TotalItems = totalItems
            }, JsonRequestBehavior.AllowGet);
        }

    }
}
