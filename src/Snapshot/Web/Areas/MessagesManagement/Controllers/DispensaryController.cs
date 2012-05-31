using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Areas.MessagesManagement.Models.Messages;
using Domain;
using Core.Persistence;

namespace Web.Areas.MessagesManagement.Controllers
{
    public class DispensaryController : Controller
    {
        public IQueryService<RawSmsReceived> QueryRawSms { get; set; }

        [HttpGet]
        //[Requires(Permissions = "Diagnosis.View")]
        public ActionResult Overview()
        {
            //ViewBag.HasNoRightsToAdd = (PermissionService.HasPermissionAssigned(REGION_ADD_PERMISSION, User.Identity.Name) == true) ? false.ToString().ToLowerInvariant() : true.ToString().ToLowerInvariant();
            //ViewBag.HasNoRightsToDelete = (PermissionService.HasPermissionAssigned(REGION_DELETE_PERMISSION, User.Identity.Name) == true) ? false.ToString().ToLowerInvariant() : true.ToString().ToLowerInvariant();

            return View("Overview");
        }

        [HttpGet]
        public JsonResult GetMessagesFromDispensary(MessagesIndexModel indexModel)
        {
            var pageSize = indexModel.limit.Value;
            var rawDataQuery = this.QueryRawSms.Query().Where(it => it.IsDispensary == true);

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
                                                   Date = message.ReceivedDate.ToShortDateString(),
                                                   Content = message.Content,
                                                   ParseSucceeded = message.ParseSucceeded,
                                                   ParseErrorMessage = message.ParseErrorMessage
                                               }).ToArray();


            return Json(new MessageIndexOuputModel
            {
                Messages = messagesModelListProjection,
                TotalItems = totalItems
            }, JsonRequestBehavior.AllowGet);
        }

    }
}
