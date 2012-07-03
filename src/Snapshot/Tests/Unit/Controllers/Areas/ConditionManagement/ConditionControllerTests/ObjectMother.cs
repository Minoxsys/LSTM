using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Web.Areas.ConditionManagement.Controllers;
using Core.Persistence;
using Domain;
using Core.Domain;
using Rhino.Mocks;
using MvcContrib.TestHelper.Fakes;
using Web.Areas.ConditionManagement.Models.Condition;

namespace Tests.Unit.Controllers.Areas.ConditionManagement.ConditionControllerTests
{
    public class ObjectMother
    {
        public ConditionController controller;

        public IQueryService<Condition> queryCondition;
        public ISaveOrUpdateCommand<Condition> saveCommand;
        public IDeleteCommand<Condition> deleteCommand;

        public IQueryService<User> queryUses;
        public IQueryService<Client> queryClient;

        public Guid conditionId;
        public Condition condition;
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
            queryCondition = MockRepository.GenerateMock<IQueryService<Condition>>();
            queryUses = MockRepository.GenerateMock<IQueryService<User>>();
            queryClient = MockRepository.GenerateMock<IQueryService<Client>>();
            saveCommand = MockRepository.GenerateMock<ISaveOrUpdateCommand<Condition>>();
            deleteCommand = MockRepository.GenerateMock<IDeleteCommand<Condition>>();
        }

        private void Setup_Controller()
        {
            controller = new ConditionController();

            FakeControllerContext.Builder.HttpContext.User = new FakePrincipal(new FakeIdentity(USER_NAME), new string[] { });
            FakeControllerContext.Initialize(controller);

            controller.QueryClients = queryClient;
            controller.QueryCondition = queryCondition;
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

            conditionId = Guid.NewGuid();
            condition = MockRepository.GeneratePartialMock<Condition>();
            condition.Stub(c => c.Id).Return(conditionId);
            condition.Code = SERVICENEEDED_CODE;
            condition.Keyword = SERVICENEEDED_KEYWORD;
            condition.Description = SERVICENEEDED_DESCRIPTION;
            condition.Client = client;
        }

        public IQueryable<Condition> PageOfConditionData(ConditionIndexModel indexModel)
        {
            List<Condition> conditionList = new List<Condition>();

            for (int i = indexModel.start.Value; i < indexModel.limit.Value; i++)
            {
                conditionList.Add(new Condition
                {
                    Code = SERVICENEEDED_CODE + i,
                    Description = SERVICENEEDED_DESCRIPTION,
                    Keyword = SERVICENEEDED_KEYWORD + i,
                    Client = client
                });
            }
            return conditionList.AsQueryable();
        }
    }
}
