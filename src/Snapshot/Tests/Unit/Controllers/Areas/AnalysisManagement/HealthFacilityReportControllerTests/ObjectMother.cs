using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Web.Areas.AnalysisManagement.Controllers;
using Core.Persistence;
using Domain;
using Rhino.Mocks;
using Web.Areas.AnalysisManagement.Models.HealthFacilityReport;
using Core.Domain;
using MvcContrib.TestHelper.Fakes;

namespace Tests.Unit.Controllers.Areas.AnalysisManagement.HealthFacilityReportControllerTests
{
    public class ObjectMother
    {
        public HealthFacilityReportController controller;

        public IQueryService<MessageFromDrugShop> queryMessageFromDrugShop;
        public IQueryService<MessageFromDispensary> queryMessageFromDispensary;
        public IQueryService<Outpost> queryOutpost;
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
        public Outpost outpost;
        public Guid outpostId;

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
            queryMessageFromDrugShop = MockRepository.GenerateMock<IQueryService<MessageFromDrugShop>>();
            queryMessageFromDispensary = MockRepository.GenerateMock<IQueryService<MessageFromDispensary>>();
            queryOutpost = MockRepository.GenerateMock<IQueryService<Outpost>>();
        }

        private void Setup_Controller()
        {
            controller = new HealthFacilityReportController();

            FakeControllerContext.Builder.HttpContext.User = new FakePrincipal(new FakeIdentity(USER_NAME), new string[] { });
            FakeControllerContext.Initialize(controller);

            controller.QueryMessageFromDispensary = queryMessageFromDispensary;
            controller.QueryMessageFromDrugShop = queryMessageFromDrugShop;
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

            outpostId = Guid.NewGuid();
            outpost = MockRepository.GeneratePartialMock<Outpost>();
            outpost.Stub(c => c.Id).Return(outpostId);
            outpost.Name = "_Spital";
            outpost.Region = region;
            outpost.Country = country;
            outpost.District = district;
            outpost.Client = client;
        }

        public IQueryable<Outpost> GetOutpostList(HealthFacilityIndexModel indexModel)
        {
            List<Outpost> outpostList = new List<Outpost>();

            outpost.OutpostType = new OutpostType() { Name = "Shop", Type = 0 };
            outpostList.Add(outpost);

            for (int i = indexModel.start.Value+1; i < indexModel.limit.Value; i++)
            {
                outpostList.Add(new Outpost
                {
                    Name = "Outpost" + i,
                    Client = client,
                    OutpostType = new OutpostType() { Name = "Shop" + i, Type = i % 3 },
                    Country = i % 5 == 0 ? country : new Country { Name = "Country" + i },
                    Region = i % 10 == 0 ? region : new Region { Name = "Region" + i },
                    District = i % 20 == 0 ? district : new District { Name = "District" + i }
                });
            }
            return outpostList.AsQueryable();
        }

        public IQueryable<MessageFromDrugShop> GetMessageFromDrugShopList(HealthFacilityIndexModel indexModel)
        {
            List<MessageFromDrugShop> messageList = new List<MessageFromDrugShop>();

            for (int i = indexModel.start.Value + 1; i < indexModel.limit.Value + 1; i++)
            {
                messageList.Add(new MessageFromDrugShop
                {
                   BirthDate = DateTime.UtcNow.AddYears(-i),
                   Gender = i % 3 == 0 ? "M" : "F",
                   IDCode = (10000000 + i).ToString(),
                   Initials = "AB",
                   SentDate = DateTime.UtcNow.AddMonths(-i),
                   OutpostId = outpostId
                });
            }
            return messageList.AsQueryable();
        }


    }
}
