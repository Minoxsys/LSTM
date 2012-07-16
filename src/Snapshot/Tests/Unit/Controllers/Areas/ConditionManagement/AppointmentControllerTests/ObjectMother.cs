using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Domain;
using Web.Areas.ConditionManagement.Models.Appointment;
using Core.Domain;
using Rhino.Mocks;
using Web.Areas.ConditionManagement.Controllers;
using Core.Persistence;
using MvcContrib.TestHelper.Fakes;

namespace Tests.Unit.Controllers.Areas.ConditionManagement.AppointmentControllerTests
{
    public class ObjectMother
    {
        public AppointmentController controller;

        public IQueryService<Appointment> queryAppointment;
        public ISaveOrUpdateCommand<Appointment> saveCommand;
        public IDeleteCommand<Appointment> deleteCommand;

        public IQueryService<User> queryUses;
        public IQueryService<Client> queryClient;

        public Guid appointmentId;
        public Appointment appointment;
        public Guid clientId;
        public Client client;
        public Guid userId;
        public User user;

        private const string SERVICENEEDED_CODE = "H-1";
        private const string SERVICENEEDED_KEYWORD = "1hour";
        private const string SERVICENEEDED_DESCRIPTION = "Patient will come after 1 hour";
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
            queryAppointment = MockRepository.GenerateMock<IQueryService<Appointment>>();
            queryUses = MockRepository.GenerateMock<IQueryService<User>>();
            queryClient = MockRepository.GenerateMock<IQueryService<Client>>();
            saveCommand = MockRepository.GenerateMock<ISaveOrUpdateCommand<Appointment>>();
            deleteCommand = MockRepository.GenerateMock<IDeleteCommand<Appointment>>();
        }

        private void Setup_Controller()
        {
            controller = new AppointmentController();

            FakeControllerContext.Builder.HttpContext.User = new FakePrincipal(new FakeIdentity(USER_NAME), new string[] { });
            FakeControllerContext.Initialize(controller);

            controller.QueryClients = queryClient;
            controller.QueryAppointment = queryAppointment;
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

            appointmentId = Guid.NewGuid();
            appointment = MockRepository.GeneratePartialMock<Appointment>();
            appointment.Stub(c => c.Id).Return(appointmentId);
            appointment.Code = SERVICENEEDED_CODE;
            appointment.Keyword = SERVICENEEDED_KEYWORD;
            appointment.Description = SERVICENEEDED_DESCRIPTION;
            appointment.Client = client;
        }

        public IQueryable<Appointment> PageOfAppointmentData(AppointmentIndexModel indexModel)
        {
            List<Appointment> appointmentList = new List<Appointment>();

            for (int i = indexModel.start.Value; i < indexModel.limit.Value; i++)
            {
                appointmentList.Add(new Appointment
                {
                    Code = SERVICENEEDED_CODE + i,
                    Description = SERVICENEEDED_DESCRIPTION,
                    Keyword = SERVICENEEDED_KEYWORD + i,
                    Client = client
                });
            }
            return appointmentList.AsQueryable();
        }
    }
}
