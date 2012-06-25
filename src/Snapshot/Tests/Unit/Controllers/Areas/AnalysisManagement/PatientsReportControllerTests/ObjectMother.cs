using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Persistence;
using Domain;
using Core.Domain;
using Web.Areas.AnalysisManagement.Controllers;
using Rhino.Mocks;
using MvcContrib.TestHelper.Fakes;

namespace Tests.Unit.Controllers.Areas.AnalysisManagement.PatientsReportControllerTests
{
    public class ObjectMother
    {
        public PatientsReportController controller;

        public IQueryService<MessageFromDrugShop> queryMessageFromDrugShop;
        public IQueryService<MessageFromDispensary> queryMessageFromDispensary;
        public IQueryService<Outpost> queryOutpost;

        public IQueryService<ServiceNeeded> queryServiceNeeded;
        public IQueryService<Diagnosis> queryDiagnosis;
        public IQueryService<Treatment> queryTreatment;
        public IQueryService<Advice> queryAdvice;

        public IQueryService<Client> queryClient;
        public IQueryService<User> queryUsers;

        public Guid clientId;
        public Guid userId;
        public Client client;
        public User user;

        public Guid adviceId;
        public Advice advice;
        public Guid diagnosisId;
        public Diagnosis diagnosis;
        public Guid serviceNeededId;
        public ServiceNeeded serviceNeeded;
        public Guid treatmentId;
        public Treatment treatment;

        public Guid countryId;
        public Country country;
        public Guid regionId;
        public Region region;
        public Guid districtId;
        public District district;
        public Guid drugshopId;
        public Outpost drugshop;
        public Guid dispensaryId;
        public Outpost dispensary;
        public Guid drugshopId1;
        public Outpost drugshop1;
        public Guid drugshopId2;
        public Outpost drugshop2;

        public List<MessageFromDrugShop> listOfMessagesFromDrugShop;
        public List<MessageFromDispensary> listOfMessagesFromDispensary;


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

            queryServiceNeeded = MockRepository.GenerateMock<IQueryService<ServiceNeeded>>();
            queryDiagnosis = MockRepository.GenerateMock<IQueryService<Diagnosis>>();
            queryTreatment = MockRepository.GenerateMock<IQueryService<Treatment>>();
            queryAdvice = MockRepository.GenerateMock<IQueryService<Advice>>();
        }

        private void Setup_Controller()
        {
            controller = new PatientsReportController();

            FakeControllerContext.Builder.HttpContext.User = new FakePrincipal(new FakeIdentity(USER_NAME), new string[] { });
            FakeControllerContext.Initialize(controller);

            controller.QueryMessageFromDispensary = queryMessageFromDispensary;
            controller.QueryMessageFromDrugShop = queryMessageFromDrugShop;
            controller.QueryOutpost = queryOutpost;
            controller.QueryAdvice = queryAdvice;
            controller.QueryDiagnosis = queryDiagnosis;
            controller.QueryServiceNeeded = queryServiceNeeded;
            controller.QueryTreatment = queryTreatment;
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
            adviceId = Guid.NewGuid();
            advice = MockRepository.GeneratePartialMock<Advice>();
            advice.Stub(c => c.Id).Return(adviceId);
            advice.Keyword = "Advice1";
            advice.Client = client;
            advice.Code = "A1";

            diagnosisId = Guid.NewGuid();
            diagnosis = MockRepository.GeneratePartialMock<Diagnosis>();
            diagnosis.Stub(c => c.Id).Return(diagnosisId);
            diagnosis.Keyword = "Diagnosis1";
            diagnosis.Code = "D1";
            diagnosis.Client = client;

            serviceNeededId = Guid.NewGuid();
            serviceNeeded = MockRepository.GeneratePartialMock<ServiceNeeded>();
            serviceNeeded.Stub(c => c.Id).Return(serviceNeededId);
            serviceNeeded.Keyword = "ServiceNeeded1";
            serviceNeeded.Code = "S1";
            serviceNeeded.Client = client;

            treatmentId = Guid.NewGuid();
            treatment = MockRepository.GeneratePartialMock<Treatment>();
            treatment.Stub(c => c.Id).Return(treatmentId);
            treatment.Keyword = "Treatment1";
            treatment.Code = "T1";
            treatment.Client = client;

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

            dispensaryId = Guid.NewGuid();
            dispensary = MockRepository.GeneratePartialMock<Outpost>();
            dispensary.Stub(c => c.Id).Return(dispensaryId);
            dispensary.Name = "DispensaryCluj";
            dispensary.Country = country;
            dispensary.Region = region;
            dispensary.District = district;
            dispensary.OutpostType = new OutpostType { Name = "Dispensary", Type = 1 };
            dispensary.Client = client;

            drugshopId = Guid.NewGuid();
            drugshop = MockRepository.GeneratePartialMock<Outpost>();
            drugshop.Stub(c => c.Id).Return(drugshopId);
            drugshop.Name = "DrugShopCluj";
            drugshop.Country = country;
            drugshop.Region = region;
            drugshop.District = district;
            drugshop.OutpostType = new OutpostType { Name = "DrugShop", Type = 0 };
            drugshop.Warehouse = dispensary;
            drugshop.Client = client;

            listOfMessagesFromDrugShop = new List<MessageFromDrugShop>();
            listOfMessagesFromDispensary = new List<MessageFromDispensary>();

            for (int i = 0; i < 3; i++)
            {
                var messageDrugShopId = Guid.NewGuid();
                var messageDrugShop = MockRepository.GeneratePartialMock<MessageFromDrugShop>();
                messageDrugShop.Stub(c => c.Id).Return(messageDrugShopId);
                messageDrugShop.BirthDate = DateTime.UtcNow.AddYears(-27 + i);
                messageDrugShop.Gender = (i % 2 == 0) ? "F" : "M";
                messageDrugShop.IDCode = "1234567" + i;
                messageDrugShop.Initials = "XY";
                messageDrugShop.OutpostId = drugshopId;
                messageDrugShop.SentDate = DateTime.UtcNow.AddDays(-i);
                messageDrugShop.ServicesNeeded = new List<ServiceNeeded> { serviceNeeded, new ServiceNeeded { Keyword = "a", Code = "SS" + i, Client = client } };

                listOfMessagesFromDrugShop.Add(messageDrugShop);

                var messageDispensaryId = Guid.NewGuid();
                var messageDispensary = MockRepository.GeneratePartialMock<MessageFromDispensary>();
                messageDispensary.Stub(c => c.Id).Return(messageDispensaryId);
                messageDispensary.Advices = new List<Advice> { new Advice { Keyword = "Adv" + i, Code = "AA" + i } };
                messageDispensary.Diagnosises = new List<Diagnosis> { diagnosis, new Diagnosis { Keyword = "Diag" + i, Code = "DD" + i } };
                messageDispensary.MessageFromDrugShop = messageDrugShop;
                messageDispensary.OutpostId = dispensaryId;
                messageDispensary.OutpostType = 1;
                messageDispensary.SentDate = DateTime.UtcNow.AddDays(-i);
                messageDispensary.Treatments = new List<Treatment> { treatment, new Treatment { Keyword = "Treat" + i, Code = "TT" + i } };

                listOfMessagesFromDispensary.Add(messageDispensary);
            }

            for (int i = 0; i < 2; i++)
            {
                var messageId = Guid.NewGuid();
                var message = MockRepository.GeneratePartialMock<MessageFromDrugShop>();
                message.Stub(c => c.Id).Return(messageId);
                message.BirthDate = DateTime.UtcNow.AddYears(-55 + i);
                message.Gender = (i % 2 == 0) ? "F" : "M";
                message.IDCode = "2222222" + i;
                message.Initials = "ZZ";
                message.OutpostId = Guid.NewGuid();
                message.SentDate = DateTime.UtcNow.AddDays(-i);
                message.ServicesNeeded = new List<ServiceNeeded> { new ServiceNeeded { Keyword = "a", Code = "SS" + i, Client = client } };

                listOfMessagesFromDrugShop.Add(message);

                var messageDispensaryId = Guid.NewGuid();
                var messageDispensary = MockRepository.GeneratePartialMock<MessageFromDispensary>();
                messageDispensary.Stub(c => c.Id).Return(messageDispensaryId);
                messageDispensary.Advices = new List<Advice> { advice, new Advice { Keyword = "Adv" + i, Code = "AAA" + i } };
                messageDispensary.Diagnosises = new List<Diagnosis> { diagnosis, new Diagnosis { Keyword = "Diag" + i, Code = "DDD" + i } };
                messageDispensary.MessageFromDrugShop = message;
                messageDispensary.OutpostId = Guid.NewGuid();
                messageDispensary.OutpostType = 1;
                messageDispensary.SentDate = DateTime.UtcNow.AddDays(-i);
                messageDispensary.Treatments = new List<Treatment> { new Treatment { Keyword = "Treat" + i, Code = "TTT" + i } };

                listOfMessagesFromDispensary.Add(messageDispensary);
            }

            drugshopId1 = Guid.NewGuid();
            drugshop1 = MockRepository.GeneratePartialMock<Outpost>();
            drugshop1.Stub(c => c.Id).Return(drugshopId1);
            drugshop1.Name = "DrugShop12";
            drugshop1.Country = country;
            drugshop1.Region = region;
            drugshop1.District = new District();
            drugshop1.OutpostType = new OutpostType { Name = "DrugShop", Type = 0 };
            drugshop1.Warehouse = dispensary;
            drugshop1.Client = client;

            drugshopId2 = Guid.NewGuid();
            drugshop2 = MockRepository.GeneratePartialMock<Outpost>();
            drugshop2.Stub(c => c.Id).Return(drugshopId2);
            drugshop2.Name = "DrugShop13";
            drugshop2.Country = country;
            drugshop2.Region = new Region();
            drugshop2.District = new District();
            drugshop2.OutpostType = new OutpostType { Name = "DrugShop", Type = 0 };
            drugshop2.Warehouse = dispensary;
            drugshop2.Client = client;

            MessageFromDrugShop messageDrugShop1 = new MessageFromDrugShop();
            messageDrugShop1.BirthDate = DateTime.UtcNow.AddYears(-34);
            messageDrugShop1.Gender = "M";
            messageDrugShop1.IDCode = "1234567f";
            messageDrugShop1.Initials = "XY";
            messageDrugShop1.OutpostId = drugshopId1;
            messageDrugShop1.SentDate = DateTime.UtcNow.AddDays(-5);
            messageDrugShop1.ServicesNeeded = new List<ServiceNeeded> { new ServiceNeeded { Keyword = "a", Code = "SA", Client = client } };

            MessageFromDrugShop messageDrugShop2 = new MessageFromDrugShop();
            messageDrugShop2.BirthDate = DateTime.UtcNow.AddYears(-16);
            messageDrugShop2.Gender = "F";
            messageDrugShop2.IDCode = "1234567b";
            messageDrugShop2.Initials = "XY";
            messageDrugShop2.OutpostId = drugshopId2;
            messageDrugShop2.SentDate = DateTime.UtcNow.AddDays(-6);
            messageDrugShop2.ServicesNeeded = new List<ServiceNeeded> { serviceNeeded, new ServiceNeeded { Keyword = "a", Code = "WS", Client = client } };

            listOfMessagesFromDrugShop.Add(messageDrugShop1);
            listOfMessagesFromDrugShop.Add(messageDrugShop2);
        }
    }
}
