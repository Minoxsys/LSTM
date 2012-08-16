using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Web.Controllers;
using Core.Services;
using Web.Security;
using Core.Persistence;
using Core.Domain;
using Domain;
using Rhino.Mocks;
using MvcContrib.TestHelper.Fakes;

namespace Tests.Unit.Controllers.AccountOptionsControllerTests
{
    public class ObjectMother
    {
        public AccountOptionsController controller;

        public IQueryService<User> queryUsers;
        public IQueryService<Client> queryClient;

        public ISaveOrUpdateCommand<User> saveCommand;
        public ISecurePassword securePassword;

        public User user;
        public Client client;
        public Guid userId;
        public Guid clientId;

        public void Init()
        {
            MockServices();
            Setup_Controller();
            SetUp_StubData();
        }

        private void MockServices()
        {
            queryUsers = MockRepository.GenerateMock<IQueryService<User>>();
            queryClient = MockRepository.GenerateMock<IQueryService<Client>>();
            saveCommand = MockRepository.GenerateMock<ISaveOrUpdateCommand<User>>();
            securePassword = MockRepository.GenerateMock<ISecurePassword>();
        }

        private void Setup_Controller()
        {
            controller = new AccountOptionsController();
            FakeControllerContext.Builder.HttpContext.User = new FakePrincipal(new FakeIdentity("admin"), new string[] { });
            FakeControllerContext.Initialize(controller);
            controller.QueryClients = queryClient;
            controller.QueryUsers = queryUsers;
            controller.SaveOrUpdateCommand = saveCommand;
            controller.SecurePassword = securePassword;
        }

        private void SetUp_StubData()
        {
            clientId = Guid.NewGuid();
            client = MockRepository.GeneratePartialMock<Client>();
            client.Stub(c => c.Id).Return(clientId);
            client.Name = "Edgard";

            userId = Guid.NewGuid();
            user = MockRepository.GeneratePartialMock<User>();
            user.Stub(c => c.Id).Return(userId);
            user.Email = "eu@yahoo.com";
            user.FirstName = "Ion";
            user.LastName = "Pop";
            user.Password = "123asd";
            user.UserName = "admin";
            user.ClientId = client.Id;
        }
    }
}
