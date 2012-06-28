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
using Web.Areas.ConditionManagement.Models.Advice;


namespace Tests.Unit.Controllers.Areas.ConditionManagement.AdviceControllerTests
{
    public class ObjectMother
    {
        public AdviceController controller;

        public IQueryService<Advice> queryAdvice;
        public IQueryService<User> queryUses;
        public IQueryService<Client> queryClient;

        public ISaveOrUpdateCommand<Advice> saveCommand;
        public IDeleteCommand<Advice> deleteCommand;

        public Guid adviceId;
        public Advice advice;
        public Guid clientId;
        public Client client;
        public Guid userId;
        public User user;

        private const string ADVICE_CODE = "A1";
        private const string ADVICE_KEYWORD = "Go home";
        private const string ADVICE_DESCRIPTION = "Patient treated and sent home";
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
            queryAdvice = MockRepository.GenerateMock<IQueryService<Advice>>();
            queryUses = MockRepository.GenerateMock<IQueryService<User>>();
            queryClient = MockRepository.GenerateMock<IQueryService<Client>>();
            saveCommand = MockRepository.GenerateMock<ISaveOrUpdateCommand<Advice>>();
            deleteCommand = MockRepository.GenerateMock<IDeleteCommand<Advice>>();
        }

        private void Setup_Controller()
        {
            controller = new AdviceController();

            FakeControllerContext.Builder.HttpContext.User = new FakePrincipal(new FakeIdentity(USER_NAME), new string[] { });
            FakeControllerContext.Initialize(controller);

            controller.QueryClients = queryClient;
            controller.QueryAdvice = queryAdvice;
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

            adviceId = Guid.NewGuid();
            advice = MockRepository.GeneratePartialMock<Advice>();
            advice.Stub(c => c.Id).Return(adviceId);
            advice.Code = ADVICE_CODE;
            advice.Keyword = ADVICE_KEYWORD;
            advice.Description = ADVICE_DESCRIPTION;
            advice.Client = client;
        }

        public IQueryable<Advice> PageOfAdviceData(AdviceIndexModel indexModel)
        {
            List<Advice> adviceList = new List<Advice>();

            for (int i = indexModel.start.Value; i < indexModel.limit.Value; i++)
            {
                adviceList.Add(new Advice
                {
                    Code = advice.Code + i,
                    Description = advice.Description,
                    Keyword = advice.Keyword + i,
                    Client = client
                });
            }
            return adviceList.AsQueryable();
        }
    }
}
