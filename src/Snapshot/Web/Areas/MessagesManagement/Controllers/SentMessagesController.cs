﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.Persistence;
using Domain;
using Web.Security;
using Web.Areas.MessagesManagement.Models.SentMessages;

namespace Web.Areas.MessagesManagement.Controllers
{
    public class SentMessagesController : Controller
    {
        public IQueryService<SentSms> QuerySms { get; set; }

        [HttpGet]
        [Requires(Permissions = "Messages.View")]
        public ActionResult Overview()
        {
            return View("Overview");
        }

        [HttpGet]
        public JsonResult GetSentMessages(SentMessagesIndexModel indexModel)
        {
            var pageSize = indexModel.limit.Value;
            var rawDataQuery = this.QuerySms.Query();

            var orderByColumnDirection = new Dictionary<string, Func<IQueryable<SentSms>>>()
            {
                { "PhoneNumber-ASC", () => rawDataQuery.OrderBy(c => c.PhoneNumber) },
                { "PhoneNumber-DESC", () => rawDataQuery.OrderByDescending(c => c.PhoneNumber) },
                { "Message-ASC", () => rawDataQuery.OrderBy(c => c.Message) },
                { "Message-DESC", () => rawDataQuery.OrderByDescending(c => c.Message) },
                { "SentDate-ASC", () => rawDataQuery.OrderBy(c => c.SentDate) },
                { "SentDate-DESC", () => rawDataQuery.OrderByDescending(c => c.SentDate) },
                { "Response-ASC", () => rawDataQuery.OrderBy(c => c.Response) },
                { "Response-DESC", () => rawDataQuery.OrderByDescending(c => c.Response) }
            };

            rawDataQuery = orderByColumnDirection[String.Format("{0}-{1}", indexModel.sort, indexModel.dir)].Invoke();

            if (!string.IsNullOrEmpty(indexModel.searchValue))
                rawDataQuery = rawDataQuery.Where(it => it.Message.Contains(indexModel.searchValue));

            var totalItems = rawDataQuery.Count();

            rawDataQuery = rawDataQuery
                .Take(pageSize)
                .Skip(indexModel.start.Value);

            var messagesModelListProjection = (from message in rawDataQuery.ToList()
                                               select new SentMessageModel
                                               {
                                                   PhoneNumber = message.PhoneNumber,
                                                   SentDate = message.SentDate.ToString("dd/MM/yyyy HH:mm"),
                                                   Message = message.Message,
                                                   Response = message.Response
                                               }).ToArray();


            return Json(new SentMessageIndexOuputModel
            {
                Messages = messagesModelListProjection,
                TotalItems = totalItems
            }, JsonRequestBehavior.AllowGet);
        }

    }
}
