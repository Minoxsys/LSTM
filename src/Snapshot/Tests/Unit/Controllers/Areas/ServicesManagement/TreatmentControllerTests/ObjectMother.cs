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
using Web.Areas.ServicesManagement.Models.Treatment;


namespace Tests.Unit.Controllers.Areas.ServicesManagement.TreatmentControllerTests
{
    public class ObjectMother
    {
        public TreatmentController controller;

        public IQueryService<Treatment> queryTreatment;
        public IQueryService<User> queryUses;
        public IQueryService<Client> queryClient;

        public ISaveOrUpdateCommand<Treatment> saveCommand;
        public IDeleteCommand<Treatment> deleteCommand;

        public Guid treatmentId;
        public Treatment treatment;
        public Guid clientId;
        public Client client;
        public Guid userId;
        public User user;

        private const string TREATMENT_CODE = "RX1; RX2; NOT/AV.";
        private const string TREATMENT_KEYWORD = "A1; A2; A3; A4";
        private const string TREATMENT_DESCRIPTION = "Discharge";
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
            queryTreatment = MockRepository.GenerateMock<IQueryService<Treatment>>();
            queryUses = MockRepository.GenerateMock<IQueryService<User>>();
            queryClient = MockRepository.GenerateMock<IQueryService<Client>>();
            saveCommand = MockRepository.GenerateMock<ISaveOrUpdateCommand<Treatment>>();
            deleteCommand = MockRepository.GenerateMock<IDeleteCommand<Treatment>>();
        }

        private void Setup_Controller()
        {
            controller = new TreatmentController();

            FakeControllerContext.Builder.HttpContext.User = new FakePrincipal(new FakeIdentity(USER_NAME), new string[] { });
            FakeControllerContext.Initialize(controller);

            controller.QueryClients = queryClient;
            controller.QueryTreatments = queryTreatment;
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

            treatmentId = Guid.NewGuid();
            treatment = MockRepository.GeneratePartialMock<Treatment>();
            treatment.Stub(c => c.Id).Return(treatmentId);
            treatment.Code = TREATMENT_CODE;
            treatment.Keyword = TREATMENT_KEYWORD;
            treatment.Description = TREATMENT_DESCRIPTION;
            treatment.Client = client;
        }

        public IQueryable<Treatment> PageOfTreatmentData(TreatmentIndexModel indexModel)
        {
            List<Treatment> treatmentList = new List<Treatment>();

            for (int i = indexModel.start.Value; i < indexModel.limit.Value; i++)
            {
                treatmentList.Add(new Treatment
                {
                    Code = "Code" + i,
                    Description = "some Description",
                    Keyword = "Keyword" + i,
                    Client = client
                });
            }
            return treatmentList.AsQueryable();
        }
    }
}
