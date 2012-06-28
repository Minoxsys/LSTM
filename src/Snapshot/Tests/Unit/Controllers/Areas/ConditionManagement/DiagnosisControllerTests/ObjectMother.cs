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
using Web.Areas.ConditionManagement.Models.Diagnosis;

namespace Tests.Unit.Controllers.Areas.ConditionManagement.DiagnosisControllerTests
{
    public class ObjectMother
    {
        public DiagnosisController controller;

        public IQueryService<Diagnosis> queryDiagnosis;
        public IQueryService<User> queryUses;
        public IQueryService<Client> queryClient;

        public ISaveOrUpdateCommand<Diagnosis> saveCommand;
        public IDeleteCommand<Diagnosis> deleteCommand;

        public Guid diagnosisId;
        public Diagnosis diagnosis;
        public Guid clientId;
        public Client client;
        public Guid userId;
        public User user;

        private const string DIAGNOSIS_CODE = "CHL/GON+";
        private const string DIAGNOSIS_KEYWORD = "Chlamydia";
        private const string DIAGNOSIS_DESCRIPTION = "Discharge";
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
            queryDiagnosis = MockRepository.GenerateMock<IQueryService<Diagnosis>>();
            queryUses = MockRepository.GenerateMock<IQueryService<User>>();
            queryClient = MockRepository.GenerateMock<IQueryService<Client>>();
            saveCommand = MockRepository.GenerateMock<ISaveOrUpdateCommand<Diagnosis>>();
            deleteCommand = MockRepository.GenerateMock<IDeleteCommand<Diagnosis>>();
        }

        private void Setup_Controller()
        {
            controller = new DiagnosisController();

            FakeControllerContext.Builder.HttpContext.User = new FakePrincipal(new FakeIdentity(USER_NAME), new string[] { });
            FakeControllerContext.Initialize(controller);

            controller.QueryClients = queryClient;
            controller.QueryDiagnosis = queryDiagnosis;
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

            diagnosisId = Guid.NewGuid();
            diagnosis = MockRepository.GeneratePartialMock<Diagnosis>();
            diagnosis.Stub(c => c.Id).Return(diagnosisId);
            diagnosis.Code = DIAGNOSIS_CODE;
            diagnosis.Keyword = DIAGNOSIS_KEYWORD;
            diagnosis.Description = DIAGNOSIS_DESCRIPTION;
            diagnosis.Client = client;
        }

        public IQueryable<Diagnosis> PageOfDiagnosisData(DiagnosisIndexModel indexModel)
        {
            List<Diagnosis> diagnosisList = new List<Diagnosis>();

            for (int i = indexModel.start.Value; i < indexModel.limit.Value; i++)
            {
                diagnosisList.Add(new Diagnosis
                {
                    Code = "CHL/GON+" + i,
                    Description = "some Description",
                    Keyword = "Chlamydia" + i,
                    Client = client
                });
            }
            return diagnosisList.AsQueryable();
        }

    }
}
