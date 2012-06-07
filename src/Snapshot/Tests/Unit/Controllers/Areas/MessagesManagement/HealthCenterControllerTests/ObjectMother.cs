using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Web.Areas.MessagesManagement.Controllers;
using Core.Persistence;
using Domain;
using Rhino.Mocks;
using Web.Areas.MessagesManagement.Models;
using Web.Areas.MessagesManagement.Models.Messages;

namespace Tests.Unit.Controllers.Areas.MessagesManagement.HealthCenterControllerTests
{
    public class ObjectMother
    {
        public HealthCenterController controller;
        public IQueryService<RawSmsReceived> queryRawSms;
        public IQueryService<Outpost> queryOutposts;

        public Guid rawSmsId;
        public RawSmsReceived rawSms;

        private const string SENDER = "0747548965";
        private const string CONTENT = "FTT452518G RS!GDD11";
        private DateTime DATE = DateTime.UtcNow;
        private const string CREDITS = "10";
        private Guid OUTPOSTID = Guid.NewGuid();
        private const string OUTPOSTNAME = "Spitalul Judetean";
        private const string ERRORMESSAGE = "Not parse correct";

        public void Init()
        {
            MockServices();
            Setup_Controller();
            SetUp_StubData();
        }

        private void SetUp_StubData()
        {
            rawSmsId = Guid.NewGuid();
            rawSms = MockRepository.GeneratePartialMock<RawSmsReceived>();
            rawSms.Stub(c => c.Id).Return(rawSmsId);
            rawSms.Content = CONTENT;
            rawSms.Credits = CREDITS;
            rawSms.OutpostType = 0;
            rawSms.OutpostId = OUTPOSTID;
            rawSms.ParseErrorMessage = ERRORMESSAGE;
            rawSms.ParseSucceeded = false;
            rawSms.ReceivedDate = DATE;
            rawSms.Sender = SENDER;
        }

        private void Setup_Controller()
        {
            controller = new HealthCenterController();
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
                    Credits = CREDITS,
                    OutpostType = 2,
                    OutpostId = Guid.NewGuid(),
                    ParseErrorMessage = "Parse error no." + i,
                    ParseSucceeded = false,
                    ReceivedDate = DateTime.UtcNow.AddDays(-i),
                    Sender = SENDER,


                });
            }
            return rawSMSList.AsQueryable();
        }

        public IQueryable<RawSmsReceived> PageOfDrugstoreData(MessagesIndexModel indexModel)
        {
            List<RawSmsReceived> rawSMSList = new List<RawSmsReceived>();

            for (int i = indexModel.start.Value; i < indexModel.limit.Value; i++)
            {
                rawSMSList.Add(new RawSmsReceived
                {
                    Content = CONTENT + "-" + i,
                    Credits = CREDITS,
                    OutpostType = i % 3,
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
