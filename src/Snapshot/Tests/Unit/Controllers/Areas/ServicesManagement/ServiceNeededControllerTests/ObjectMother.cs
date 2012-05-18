using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Web.Areas.ServicesManagement.Controllers;
using Core.Persistence;
using Domain;
using Core.Domain;
using Rhino.Mocks;
using MvcContrib.TestHelper.Fakes;
using Web.Areas.ServicesManagement.Models.ServiceNeeded;

namespace Tests.Unit.Controllers.Areas.ServicesManagement.ServiceNeededControllerTests
{
    public class ObjectMother
    {
        public ServiceNeededController controller;

        public IQueryService<ServiceNeeded> queryServiceNeeded;
        public ISaveOrUpdateCommand<ServiceNeeded> saveCommand;
        public IDeleteCommand<ServiceNeeded> deleteCommand;

        public IQueryService<User> queryUses;
        public IQueryService<Client> queryClient;

        public Guid serviceNeededId;
        public ServiceNeeded serviceNeeded;
        public Guid clientId;
        public Client client;
        public Guid userId;
        public User user;

        private const string SERVICENEEDED_CODE = "T-1";
        private const string SERVICENEEDED_KEYWORD = "HIV";
        private const string SERVICENEEDED_DESCRIPTION = "HIV testing";
        private const string CLIENT_NAME = "Ion";
        private const string USER_NAME = "IonPopescu";

        public void Init()
        {
            MockServices();
            Setup_Controller();
            SetUp_StubData();
        }

        private void MockServices()
        {
            queryServiceNeeded = MockRepository.GenerateMock<IQueryService<ServiceNeeded>>();
            queryUses = MockRepository.GenerateMock<IQueryService<User>>();
            queryClient = MockRepository.GenerateMock<IQueryService<Client>>();
            saveCommand = MockRepository.GenerateMock<ISaveOrUpdateCommand<ServiceNeeded>>();
            deleteCommand = MockRepository.GenerateMock<IDeleteCommand<ServiceNeeded>>();
        }

        private void Setup_Controller()
        {
            controller = new ServiceNeededController();

            FakeControllerContext.Builder.HttpContext.User = new FakePrincipal(new FakeIdentity(USER_NAME), new string[] { });
            FakeControllerContext.Initialize(controller);

            controller.QueryClients = queryClient;
            controller.QueryServiceNeeded = queryServiceNeeded;
            controller.QueryUsers = queryUses;

            controller.SaveOrUpdateCommand = saveCommand;
            controller.DeleteCommand = deleteCommand;
        }

        private void SetUp_StubData()
        {
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
            queryUses.Stub(c => c.Query()).Return(new[] { user }.AsQueryable());

            serviceNeededId = Guid.NewGuid();
            serviceNeeded = MockRepository.GeneratePartialMock<ServiceNeeded>();
            serviceNeeded.Stub(c => c.Id).Return(serviceNeededId);
            serviceNeeded.Code = SERVICENEEDED_CODE;
            serviceNeeded.Keyword = SERVICENEEDED_KEYWORD;
            serviceNeeded.Description = SERVICENEEDED_DESCRIPTION;
            serviceNeeded.Client = client;
        }

        public IQueryable<ServiceNeeded> PageOfServiceNeededData(ServiceNeededIndexModel indexModel)
        {
            List<ServiceNeeded> serviceNeededList = new List<ServiceNeeded>();

            for (int i = indexModel.start.Value; i < indexModel.limit.Value; i++)
            {
                serviceNeededList.Add(new ServiceNeeded
                {
                    Code = SERVICENEEDED_CODE + i,
                    Description = SERVICENEEDED_DESCRIPTION,
                    Keyword = SERVICENEEDED_KEYWORD + i,
                    Client = client
                });
            }
            return serviceNeededList.AsQueryable();
        }
    }
}
