﻿using System;
using System.Collections.Generic;
using System.Linq;
using Core.Persistence;
using Domain;
using Web.Areas.MessagesManagement.Models.Messages;

namespace Web.Services
{
    public interface IMessagesService
    {
        MessageIndexOuputModel GetMessagesFromOutpost(MessagesIndexModel indexModel, int outpostType);
    }

    public class MessagesService : IMessagesService
    {
        private readonly IQueryService<RawSmsReceived> _queryRawSmsReceived;
        private readonly IQueryService<Outpost> _queryOutposts;

        public MessagesService(IQueryService<RawSmsReceived> queryRawSmsReceived, IQueryService<Outpost> queryOutposts)
        {
            _queryOutposts = queryOutposts;
            _queryRawSmsReceived = queryRawSmsReceived;
        }

        public MessageIndexOuputModel GetMessagesFromOutpost(MessagesIndexModel indexModel, int outpostType)
        {
            var rawDataQuery = _queryRawSmsReceived.Query().Where(it => it.OutpostType == outpostType);

            int totalItems;
            if (indexModel != null)
            {
                rawDataQuery = SortMessages(indexModel, rawDataQuery);
                rawDataQuery = ApplySearchFilter(indexModel, rawDataQuery);
                totalItems = rawDataQuery.Count();
                rawDataQuery = ApplyPaging(indexModel, rawDataQuery);
            }
            else
            {
                totalItems = rawDataQuery.Count();
            }

            var messagesModelListProjection = (from message in rawDataQuery.ToList()
                select new MessageModel
                {
                    Sender = message.Sender,
                    Date = message.ReceivedDate.ToString("dd/MM/yyyy HH:mm"),
                    Content = message.Content,
                    ParseSucceeded = message.ParseSucceeded,
                    ParseErrorMessage = message.ParseErrorMessage,
                    OutpostName = _queryOutposts.Load(message.OutpostId) != null ? _queryOutposts.Load(message.OutpostId).Name : null
                }).ToArray();

            return new MessageIndexOuputModel
            {
                Messages = messagesModelListProjection,
                TotalItems = totalItems
            };
        }

        private IQueryable<RawSmsReceived> ApplyPaging(MessagesIndexModel indexModel, IQueryable<RawSmsReceived> rawDataQuery)
        {
            if (indexModel.limit != null && indexModel.start != null)
            {
                var pageSize = indexModel.limit.Value;

                rawDataQuery = rawDataQuery
                    .Take(pageSize)
                    .Skip(indexModel.start.Value);
            }
            return rawDataQuery;
        }

        private IQueryable<RawSmsReceived> ApplySearchFilter(MessagesIndexModel indexModel, IQueryable<RawSmsReceived> rawDataQuery)
        {
            if (!string.IsNullOrEmpty(indexModel.searchValue))
                rawDataQuery = rawDataQuery.Where(it => it.Content.Contains(indexModel.searchValue));
            return rawDataQuery;
        }

        private IQueryable<RawSmsReceived> SortMessages(MessagesIndexModel indexModel, IQueryable<RawSmsReceived> rawDataQuery)
        {
            var orderByColumnDirection = new Dictionary<string, Func<IQueryable<RawSmsReceived>>>
            {
                {"Sender-ASC", () => rawDataQuery.OrderBy(c => c.Sender)},
                {"Sender-DESC", () => rawDataQuery.OrderByDescending(c => c.Sender)},
                {"Date-ASC", () => rawDataQuery.OrderBy(c => c.ReceivedDate)},
                {"Date-DESC", () => rawDataQuery.OrderByDescending(c => c.ReceivedDate)},
                {"Content-ASC", () => rawDataQuery.OrderBy(c => c.Content)},
                {"Content-DESC", () => rawDataQuery.OrderByDescending(c => c.Content)},
                {"ParseSucceeded-ASC", () => rawDataQuery.OrderBy(c => c.ParseSucceeded)},
                {"ParseSucceeded-DESC", () => rawDataQuery.OrderByDescending(c => c.ParseSucceeded)},
                {"ParseErrorMessage-ASC", () => rawDataQuery.OrderBy(c => c.ParseErrorMessage)},
                {"ParseErrorMessage-DESC", () => rawDataQuery.OrderByDescending(c => c.ParseErrorMessage)}
            };
            rawDataQuery = orderByColumnDirection[String.Format("{0}-{1}", indexModel.sort, indexModel.dir)].Invoke();
            return rawDataQuery;
        }
    }
}