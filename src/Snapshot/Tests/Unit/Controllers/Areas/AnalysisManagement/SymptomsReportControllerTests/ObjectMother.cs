using System;
using System.Collections.Generic;
using System.Linq;
using Rhino.Mocks;
using Domain;
using Core.Persistence;
using Core.Domain;
using Web.Areas.AnalysisManagement.Controllers;
using MvcContrib.TestHelper.Fakes;

namespace Tests.Unit.Controllers.Areas.AnalysisManagement.SymptomsReportControllerTests
{
    public class ObjectMother
    {
        public SymptomsReportController controller;

        public IQueryService<MessageFromDrugShop> queryMessageFromDrugShop;
        public IQueryService<Outpost> queryOutpost;
        public IQueryService<Condition> querySymptoms;
        public IQueryService<Client> queryClient;
        public IQueryService<User> queryUsers;
        public IQueryService<Region> queryRegion;

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
        public List<Condition> symptomsList;
        public List<MessageFromDrugShop> messageList;

        public FakePrincipal User;

        private const string CLIENT_NAME = "Ion";
        public const string USER_NAME = "IonPopescu";

        public void Init()
        {
            MockServices();
            Setup_Controller();
            StubUserAndItsClient();
            SetUp_StubData();
        }

        private void MockServices()
        {
            queryMessageFromDrugShop = MockRepository.GenerateMock<IQueryService<MessageFromDrugShop>>();
            querySymptoms = MockRepository.GenerateMock<IQueryService<Condition>>();
            queryOutpost = MockRepository.GenerateMock<IQueryService<Outpost>>();
            queryRegion = MockRepository.GenerateMock<IQueryService<Region>>();
        }

        private void Setup_Controller()
        {
            controller = new SymptomsReportController();

            this.User = new FakePrincipal(new FakeIdentity(USER_NAME), new string[] { });
            FakeControllerContext.Builder.HttpContext.User = this.User;
            FakeControllerContext.Initialize(controller);

            controller.QueryMessageFromDrugShop = queryMessageFromDrugShop;
            controller.QuerySymptoms = querySymptoms;
            controller.QueryOutpost = queryOutpost;
            controller.QueryRegion = queryRegion;
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


            controller._user = user;
            controller._client = client;
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
            outpost1.OutpostType = new OutpostType { Type = 0, Name = "dispensary" };

            Guid outpostId2 = Guid.NewGuid();
            Outpost outpost2 = MockRepository.GeneratePartialMock<Outpost>();
            outpost2.Stub(c => c.Id).Return(outpostId2);
            outpost2.Name = "Dispensary2";
            outpost2.Region = region;
            outpost2.Country = country;
            outpost2.District = new District();
            outpost2.Client = client;
            outpost2.OutpostType = new OutpostType { Type = 0, Name = "dispensary" };

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

            symptomsList = new List<Condition>();
            for (int i = 0; i < 4; i++)
            {
                Guid diagnosisId = Guid.NewGuid();
                Condition diagnosis = MockRepository.GeneratePartialMock<Condition>();
                diagnosis.Stub(c => c.Id).Return(diagnosisId);
                diagnosis.Code = "S" + i;
                diagnosis.Keyword = "s" + i;
                diagnosis.Client = client;

                symptomsList.Add(diagnosis);
            }

            messageList = new List<MessageFromDrugShop>();

            for (int i = 0; i < 10; i++)
            {
                Guid messageId = Guid.NewGuid();
                MessageFromDrugShop message = MockRepository.GeneratePartialMock<MessageFromDrugShop>();
                message.Stub(c => c.Id).Return(messageId);
                message.OutpostId = outpostList[i % 3].Id;
                message.SentDate = DateTime.UtcNow.AddMonths(-i);
                message.ServicesNeeded = new List<Condition>{ symptomsList[i % 4] };
                message.Gender = (i % 4 == 0) ? "M":"F";

                messageList.Add(message);
            }

        }
    }
}
