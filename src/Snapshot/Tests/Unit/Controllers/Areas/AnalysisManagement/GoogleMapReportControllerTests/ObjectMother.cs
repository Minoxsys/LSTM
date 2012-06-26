using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Web.Areas.AnalysisManagement.Controllers;
using Core.Persistence;
using Domain;
using Core.Domain;
using Rhino.Mocks;
using MvcContrib.TestHelper.Fakes;

namespace Tests.Unit.Controllers.Areas.AnalysisManagement.GoogleMapReportControllerTests
{
    public class ObjectMother
    {
        public GoogleMapReportController controller;

        public IQueryService<Outpost> queryOutpost;
        public IQueryService<Region> queryRegion;
        public IQueryService<District> queryDistrict;
        public IQueryService<Client> queryClient;
        public IQueryService<User> queryUsers;
        public IQueryService<MessageFromDispensary> queryMessageFromDispensary;
        public IQueryService<MessageFromDrugShop> queryMessageFromDrugShop;

        public Client client;
        public Guid clientId;
        public User user;
        public Guid userId;
        public Outpost drugshop;
        public Guid drugshopId;
        public Outpost dispensary;
        public Guid dispensaryId;
        public District district1;
        public Guid district1Id;
        public District district2;
        public Guid district2Id;
        public Region region;
        public Guid regionId;
        public Guid countryId;

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
            queryRegion = MockRepository.GenerateMock<IQueryService<Region>>();
            queryDistrict = MockRepository.GenerateMock<IQueryService<District>>();
            queryOutpost = MockRepository.GenerateMock<IQueryService<Outpost>>();
            queryMessageFromDispensary = MockRepository.GenerateMock<IQueryService<MessageFromDispensary>>();
            queryMessageFromDrugShop = MockRepository.GenerateMock<IQueryService<MessageFromDrugShop>>();
        }
        private void Setup_Controller()
        {
            controller = new GoogleMapReportController();

            FakeControllerContext.Builder.HttpContext.User = new FakePrincipal(new FakeIdentity(USER_NAME), new string[] { });
            FakeControllerContext.Initialize(controller);

            controller.QueryRegions = queryRegion;
            controller.QueryDistricts = queryDistrict;
            controller.QueryMessageFromDispensarys = queryMessageFromDispensary;
            controller.QueryMessageFromDrugShops = queryMessageFromDrugShop;
            controller.QueryOutposts = queryOutpost;
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
            Country country = MockRepository.GeneratePartialMock<Country>();
            country.Stub(c => c.Id).Return(countryId);
            country.Name = "Country";
            country.Client = client;

            regionId = Guid.NewGuid();
            region = MockRepository.GeneratePartialMock<Region>();
            region.Stub(c => c.Id).Return(regionId);
            region.Name = "Region";
            region.Country = country;
            region.Client = client;

            district1Id = Guid.NewGuid();
            district1 = MockRepository.GeneratePartialMock<District>();
            district1.Stub(c => c.Id).Return(district1Id);
            district1.Name = "District";
            district1.Region = region;
            district1.Client = client;

            district2Id = Guid.NewGuid();
            district2 = MockRepository.GeneratePartialMock<District>();
            district2.Stub(c => c.Id).Return(district2Id);
            district2.Name = "District";
            district2.Region = region;
            district2.Client = client;

            drugshopId = Guid.NewGuid();
            drugshop = MockRepository.GeneratePartialMock<Outpost>();
            drugshop.Stub(c => c.Id).Return(drugshopId);
            drugshop.Name = "Drugshop";
            drugshop.Region = region;
            drugshop.Country = country;
            drugshop.District = district2;
            drugshop.Client = client;
            drugshop.OutpostType = new OutpostType { Name = "DrugShop", Type = 0 };
            drugshop.Latitude = "(34.234323454,27.876567546)";

            dispensaryId = Guid.NewGuid();
            dispensary = MockRepository.GeneratePartialMock<Outpost>();
            dispensary.Stub(c => c.Id).Return(dispensaryId);
            dispensary.Name = "Dispensary";
            dispensary.Region = region;
            dispensary.Country = country;
            dispensary.District = district1;
            dispensary.Client = client;
            dispensary.OutpostType = new OutpostType { Name = "Dispensary", Type = 1 };
            dispensary.Latitude = "(12.123212321,4.345434567)";

        }

        public IQueryable<MessageFromDispensary> ListOfMessageFromDispensary()
        {
            List<MessageFromDispensary> list = new List<MessageFromDispensary>();

            for (int i = 0; i < 10; i++)
            {
                list.Add(new MessageFromDispensary
                {
                    OutpostId = dispensaryId,
                    OutpostType = 1,
                    SentDate = DateTime.UtcNow,
                    MessageFromDrugShop = new MessageFromDrugShop { Gender = (i % 2 == 0) ? "F" : "M", BirthDate = DateTime.UtcNow.AddYears(-i * 5) }
                });
            }
            return list.AsQueryable();
        }

        public IQueryable<MessageFromDrugShop> ListOfMessageFromDrugShop()
        {
            List<MessageFromDrugShop> list = new List<MessageFromDrugShop>();

            for (int i = 0; i < 10; i++)
            {
                list.Add(new MessageFromDrugShop
                {
                    OutpostId = drugshopId,
                    SentDate = DateTime.UtcNow,
                    Gender = (i%2 == 0)? "F" : "M",
                    BirthDate = DateTime.UtcNow.AddYears(-i*5)
                });
            }
            return list.AsQueryable();
        }
    }
}
