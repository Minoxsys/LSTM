﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Mocks;
using Domain;
using Core.Persistence;
using Core.Domain;
using Web.Areas.AnalysisManagement.Controllers;
using MvcContrib.TestHelper.Fakes;

namespace Tests.Unit.Controllers.Areas.AnalysisManagement.AdviceReportControllerTests
{
    public class ObjectMother
    {
        public AdviceReportController controller;

        public IQueryService<MessageFromDispensary> queryMessageFromDispensary;
        public IQueryService<Outpost> queryOutpost;
        public IQueryService<Advice> queryAdvice;
        public IQueryService<Client> queryClient;
        public IQueryService<User> queryUsers;

        public Client client;
        public User user;
        public Guid clientId;
        public Guid userId;

        public Country country;
        public Guid countryId;
        public Region region;
        public Guid regionId;
        public District district;
        public Guid districtId;
        public List<Outpost> outpostList;
        public List<Advice> adviceList;
        public List<MessageFromDispensary> messageList;

        private const string CLIENT_NAME = "Ion";
        private const string USER_NAME = "IonPopescu";

        public void Init()
        {
            MockServices();
            Setup_Controller();
            StubUserAndItsClient();
            SetUp_StubData();
        }

        private void MockServices()
        {
            queryMessageFromDispensary = MockRepository.GenerateMock<IQueryService<MessageFromDispensary>>();
            queryAdvice = MockRepository.GenerateMock<IQueryService<Advice>>();
            queryOutpost = MockRepository.GenerateMock<IQueryService<Outpost>>();
        }

        private void Setup_Controller()
        {
            controller = new AdviceReportController();

            FakeControllerContext.Builder.HttpContext.User = new FakePrincipal(new FakeIdentity(USER_NAME), new string[] { });
            FakeControllerContext.Initialize(controller);

            controller.QueryMessageFromDispensary = queryMessageFromDispensary;
            controller.QueryAdvice = queryAdvice;
            controller.QueryOutpost = queryOutpost;
        }

        public void StubUserAndItsClient()
        {
            queryClient = MockRepository.GenerateStub<IQueryService<Client>>();
            queryUsers = MockRepository.GenerateStub<IQueryService<User>>();

            clientId = Guid.NewGuid();
            client = MockRepository.GeneratePartialMock<Client>();
            client.Stub(c => c.Id).Return(clientId);
            client.Name = CLIENT_NAME;

            userId = Guid.NewGuid();
            user = MockRepository.GeneratePartialMock<User>();
            user.Stub(c => c.Id).Return(Guid.NewGuid());
            user.Stub(c => c.ClientId).Return(client.Id);
            user.UserName = USER_NAME;
            user.Password = "4321";

            queryClient.Stub(c => c.Load(clientId)).Return(client);
            queryUsers.Stub(c => c.Query()).Return(new[] { user }.AsQueryable());

            controller.QueryClients = queryClient;
            controller.QueryUsers = queryUsers;
        }

        private void SetUp_StubData()
        {
            countryId = Guid.NewGuid();
            country = MockRepository.GeneratePartialMock<Country>();
            country.Stub(c => c.Id).Return(countryId);
            country.Name = "Romania";
            country.Client = client;

            regionId = Guid.NewGuid();
            region = MockRepository.GeneratePartialMock<Region>();
            region.Stub(c => c.Id).Return(regionId);
            region.Name = "Transilvania";
            region.Country = country;
            region.Client = client;

            districtId = Guid.NewGuid();
            district = MockRepository.GeneratePartialMock<District>();
            district.Stub(c => c.Id).Return(districtId);
            district.Name = "Cluj";
            district.Region = region;
            district.Client = client;

            Guid outpostId1 = Guid.NewGuid();
            Outpost outpost1 = MockRepository.GeneratePartialMock<Outpost>();
            outpost1.Stub(c => c.Id).Return(outpostId1);
            outpost1.Name = "Dispensary1";
            outpost1.Region = region;
            outpost1.Country = country;
            outpost1.District = district;
            outpost1.Client = client;
            outpost1.OutpostType = new OutpostType { Type = 1, Name = "dispensary" };

            Guid outpostId2 = Guid.NewGuid();
            Outpost outpost2 = MockRepository.GeneratePartialMock<Outpost>();
            outpost2.Stub(c => c.Id).Return(outpostId2);
            outpost2.Name = "Dispensary2";
            outpost2.Region = region;
            outpost2.Country = country;
            outpost2.District = new District();
            outpost2.Client = client;
            outpost2.OutpostType = new OutpostType { Type = 1, Name = "dispensary" };

            Guid outpostId3 = Guid.NewGuid();
            Outpost outpost3 = MockRepository.GeneratePartialMock<Outpost>();
            outpost3.Stub(c => c.Id).Return(outpostId3);
            outpost3.Name = "Dispensary3";
            outpost3.Region = new Region();
            outpost3.Country = country;
            outpost3.District = new District();
            outpost3.Client = client;
            outpost3.OutpostType = new OutpostType { Type = 1, Name = "dispensary" };

            outpostList = new List<Outpost>();
            outpostList.Add(outpost1);
            outpostList.Add(outpost2);
            outpostList.Add(outpost3);

            adviceList = new List<Advice>();
            for (int i = 0; i < 4; i++)
            {
                Guid adviceId = Guid.NewGuid();
                Advice advice = MockRepository.GeneratePartialMock<Advice>();
                advice.Stub(c => c.Id).Return(adviceId);
                advice.Code = "A" + i;
                advice.Keyword = "A" + i;
                advice.Client = client;

                adviceList.Add(advice);
            }

            messageList = new List<MessageFromDispensary>();

            for (int i = 0; i < 10; i++)
            {
                Guid messageId = Guid.NewGuid();
                MessageFromDispensary message = MockRepository.GeneratePartialMock<MessageFromDispensary>();
                message.Stub(c => c.Id).Return(messageId);
                message.OutpostId = outpostList[i % 3].Id;
                message.OutpostType = 1;
                message.SentDate = DateTime.UtcNow.AddMonths(-i);
                message.Advices = new List<Advice> { adviceList[i % 4] };
                message.MessageFromDrugShop = (i % 2 == 0) ? new MessageFromDrugShop { Gender = "F" } : new MessageFromDrugShop { Gender = "M" };

                messageList.Add(message);
            }

        }
    }
}
