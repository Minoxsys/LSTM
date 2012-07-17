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

namespace Tests.Unit.Controllers.Areas.AnalysisManagement.SummaryReportControllerTests
{
    public class ObjectMother
    {
        public SummaryReportController controller;

        public IQueryService<MessageFromDispensary> queryMessageFromDispensary;
        public IQueryService<MessageFromDrugShop> queryMessageFromDrugShop;
        public IQueryService<Outpost> queryOutpost;
        public IQueryService<Client> queryClient;
        public IQueryService<User> queryUsers;
        public IQueryService<Country> queryCountry;
        public IQueryService<Region> queryRegion;
        public IQueryService<District> queryDistrict;

        public Guid clientId;
        public Guid userId;
        public Client client;
        public User user;
        private const string CLIENT_NAME = "Ion";
        private const string USER_NAME = "IonPopescu";

        public Country country;
        public Guid countryId;
        public Region region;
        public Guid regionId;
        public District district;
        public Guid districtId;
        public List<Outpost> outpostList;
        public List<MessageFromDispensary> messageFromDispensaryList;
        public List<Diagnosis> diagnosisList;
        public List<Treatment> treatmentList;
        public List<Advice> adviceList;
        public List<MessageFromDrugShop> messageFromDrugShopList;
        

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

            queryCountry = MockRepository.GenerateMock<IQueryService<Country>>();
            queryRegion = MockRepository.GenerateMock<IQueryService<Region>>();
            queryDistrict = MockRepository.GenerateMock<IQueryService<District>>();
        }

        private void Setup_Controller()
        {
            controller = new SummaryReportController();

            FakeControllerContext.Builder.HttpContext.User = new FakePrincipal(new FakeIdentity(USER_NAME), new string[] { });
            FakeControllerContext.Initialize(controller);

            controller.QueryMessageFromDispensary = queryMessageFromDispensary;
            controller.QueryMessageFromDrugShop = queryMessageFromDrugShop;
            controller.QueryOutpost = queryOutpost;
            controller.QueryCountry = queryCountry;
            controller.QueryRegion = queryRegion;
            controller.QueryDistrict = queryDistrict;
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

            Guid drugShopId1 = Guid.NewGuid();
            Outpost drugShop1 = MockRepository.GeneratePartialMock<Outpost>();
            drugShop1.Stub(c => c.Id).Return(drugShopId1);
            drugShop1.Name = "DrugShop1";
            drugShop1.Region = region;
            drugShop1.Country = country;
            drugShop1.District = district;
            drugShop1.Client = client;
            drugShop1.OutpostType = new OutpostType { Type = 0, Name = "drugshop" };
            drugShop1.Warehouse = outpost1;
            drugShop1.Stub(c => c.Warehouse.Id).Return(outpost1.Id);

            Guid drugShopId2 = Guid.NewGuid();
            Outpost drugShop2 = MockRepository.GeneratePartialMock<Outpost>();
            drugShop2.Stub(c => c.Id).Return(drugShopId2);
            drugShop2.Name = "DrugShop2";
            drugShop2.Region = region;
            drugShop2.Country = country;
            drugShop2.District = district;
            drugShop2.Client = client;
            drugShop2.OutpostType = new OutpostType { Type = 0, Name = "drugshop" };
            drugShop2.Warehouse = outpost1;
            drugShop2.Stub(c => c.Warehouse.Id).Return(outpost1.Id);

            Guid drugShopId3 = Guid.NewGuid();
            Outpost drugShop3 = MockRepository.GeneratePartialMock<Outpost>();
            drugShop3.Stub(c => c.Id).Return(drugShopId3);
            drugShop3.Name = "DrugShop3";
            drugShop3.Region = region;
            drugShop3.Country = country;
            drugShop3.District = district;
            drugShop3.Client = client;
            drugShop3.OutpostType = new OutpostType { Type = 0, Name = "drugshop" };
            drugShop3.Warehouse = outpost1;
            drugShop3.Stub(c => c.Warehouse.Id).Return(outpost1.Id);

            outpostList = new List<Outpost>();
            outpostList.Add(outpost1);
            outpostList.Add(outpost2);
            outpostList.Add(outpost3);
            outpostList.Add(drugShop1);
            outpostList.Add(drugShop2);
            outpostList.Add(drugShop3);

            diagnosisList = new List<Diagnosis>();
            for (int i = 0; i < 4; i++)
            {
                Guid diagnosisId = Guid.NewGuid();
                Diagnosis diagnosis = MockRepository.GeneratePartialMock<Diagnosis>();
                diagnosis.Stub(c => c.Id).Return(diagnosisId);
                diagnosis.Code = "D" + i;
                diagnosis.Keyword = "d" + i;
                diagnosis.Client = client;

                diagnosisList.Add(diagnosis);
            }

            treatmentList = new List<Treatment>();
            for (int i = 0; i < 6; i++)
            {
                Guid treatmentId = Guid.NewGuid();
                Treatment treatment = MockRepository.GeneratePartialMock<Treatment>();
                treatment.Stub(c => c.Id).Return(treatmentId);
                treatment.Code = "T" + i;
                treatment.Keyword = "t" + i;
                treatment.Client = client;

                treatmentList.Add(treatment);
            }

            Guid treatmentId1 = Guid.NewGuid();
            Treatment treatment1 = MockRepository.GeneratePartialMock<Treatment>();
            treatment1.Stub(c => c.Id).Return(treatmentId1);
            treatment1.Code = "N";
            treatment1.Keyword = "N";
            treatment1.Client = client;

            treatmentList.Add(treatment1);

            Guid treatmentId2 = Guid.NewGuid();
            Treatment treatment2 = MockRepository.GeneratePartialMock<Treatment>();
            treatment2.Stub(c => c.Id).Return(treatmentId2);
            treatment2.Code = "NP";
            treatment2.Keyword = "NP";
            treatment2.Client = client;

            treatmentList.Add(treatment2);

            Guid treatmentId3 = Guid.NewGuid();
            Treatment treatment3 = MockRepository.GeneratePartialMock<Treatment>();
            treatment3.Stub(c => c.Id).Return(treatmentId3);
            treatment3.Code = "NS4";
            treatment3.Keyword = "NS4";
            treatment3.Client = client;

            treatmentList.Add(treatment3);

            Guid treatmentId4 = Guid.NewGuid();
            Treatment treatment4 = MockRepository.GeneratePartialMock<Treatment>();
            treatment4.Stub(c => c.Id).Return(treatmentId4);
            treatment4.Code = "NSA";
            treatment4.Keyword = "NSA";
            treatment4.Client = client;

            treatmentList.Add(treatment4);

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

            DateTime birthdate = DateTime.UtcNow.AddYears(-25);

            messageFromDrugShopList = new List<MessageFromDrugShop>();
            messageFromDrugShopList.Add(new MessageFromDrugShop { BirthDate = birthdate, Gender = "F", Initials = "XY", OutpostId = drugShop1.Id });
            messageFromDrugShopList.Add(new MessageFromDrugShop { BirthDate = DateTime.UtcNow.AddYears(-27), Gender = "M", Initials = "AA", OutpostId = drugShop2.Id });
            messageFromDrugShopList.Add(new MessageFromDrugShop { BirthDate = DateTime.UtcNow.AddYears(-22), Gender = "F", Initials = "SS", OutpostId = drugShop3.Id });
            messageFromDrugShopList.Add(new MessageFromDrugShop { BirthDate = DateTime.UtcNow.AddYears(-23), Gender = "M", Initials = "SA", OutpostId = drugShop1.Id });
            messageFromDrugShopList.Add(new MessageFromDrugShop { BirthDate = birthdate, Gender = "F", Initials = "XY", OutpostId = drugShop1.Id });

            messageFromDispensaryList = new List<MessageFromDispensary>();

            for (int i = 0; i < 10; i++)
            {
                Guid messageId = Guid.NewGuid();
                MessageFromDispensary message = MockRepository.GeneratePartialMock<MessageFromDispensary>();
                message.Stub(c => c.Id).Return(messageId);
                message.OutpostId = outpostList[i % 3].Id;
                message.OutpostType = 1;
                message.SentDate = DateTime.UtcNow.AddMonths(-i);
                message.Diagnosises = new List<Diagnosis> { diagnosisList[i % 4] };
                message.Treatments = new List<Treatment> { treatmentList[i] };
                message.Advices = new List<Advice> { adviceList[i % 4] };
                message.MessageFromDrugShop = messageFromDrugShopList[i%5];

                messageFromDispensaryList.Add(message);
            }
        }
    }
}
