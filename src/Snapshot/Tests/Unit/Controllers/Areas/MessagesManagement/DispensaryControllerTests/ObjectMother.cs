﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Web.Areas.MessagesManagement.Controllers;
using Domain;
using Core.Persistence;
using Rhino.Mocks;
using Web.Areas.MessagesManagement.Models.Messages;

namespace Tests.Unit.Controllers.Areas.MessagesManagement.DispensaryControllerTests
{
    public class ObjectMother
    {
        public DispensaryController controller;
        public IQueryService<RawSmsReceived> queryRawSms;
        public IQueryService<Outpost> queryOutposts;
        public IQueryService<MessageFromDrugShop> queryMessageFromDrugShop;

        private const string SENDER = "0748523666";
        private const string CONTENT = "123456 Tr1 Tr2";
        private DateTime DATE = DateTime.UtcNow;
        private const string CREDITS = "10";
        private Guid OUTPOSTID = Guid.NewGuid();
        private const string OUTPOSTNAME = "Dispensary";
        private const string ERRORMESSAGE = "Not parse correct";

        public void Init()
        {
            MockServices();
            Setup_Controller();
        }

        private void Setup_Controller()
        {
            controller = new DispensaryController();
            controller.QueryRawSms = queryRawSms;
            controller.QueryOutpost = queryOutposts;
        }

        private void MockServices()
        {
            queryRawSms = MockRepository.GenerateMock<IQueryService<RawSmsReceived>>();
            queryOutposts = MockRepository.GenerateMock<IQueryService<Outpost>>();
        }

        public IQueryable<RawSmsReceived> PageOfData(MessagesIndexModel indexModel)
        {
            List<RawSmsReceived> rawSMSList = new List<RawSmsReceived>();

            for (int i = indexModel.start.Value; i < indexModel.limit.Value; i++)
            {
                rawSMSList.Add(new RawSmsReceived
                {
                    Content = CONTENT + "-" + i,
                    OutpostType = 1,
                    OutpostId = Guid.NewGuid(),
                    ParseErrorMessage = "Parse error no." + i,
                    ParseSucceeded = false,
                    ReceivedDate = DateTime.UtcNow.AddDays(-i),
                    Sender = SENDER,


                });
            }
            return rawSMSList.AsQueryable();
        }

        public IQueryable<RawSmsReceived> PageOfDispensaryData(MessagesIndexModel indexModel)
        {
            List<RawSmsReceived> rawSMSList = new List<RawSmsReceived>();

            for (int i = indexModel.start.Value; i < indexModel.limit.Value; i++)
            {
                rawSMSList.Add(new RawSmsReceived
                {
                    Content = CONTENT + "-" + i,
                    OutpostType = i % 2,
                    OutpostId = Guid.NewGuid(),
                    ParseErrorMessage = "Parse error no." + i,
                    ParseSucceeded = false,
                    ReceivedDate = DateTime.UtcNow.AddDays(-i),
                    Sender = SENDER,


                });
            }
            return rawSMSList.AsQueryable();
        }
    }
}
