using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Web.Areas.MessagesManagement.Controllers;
using Core.Persistence;
using Domain;
using Rhino.Mocks;
using Web.Areas.MessagesManagement.Models.SentMessages;

namespace Tests.Unit.Controllers.Areas.MessagesManagement.SentMessagesControllerTests
{
    public class ObjectMother
    {
        public SentMessagesController controller;
        public IQueryService<SentSms> querySms;

        public const string PHONENUMBER = "+123456789";
        public const string MESSAGE = "Some message";
        public const string RESPONSE = "OK";

        public void Init()
        {
            MockServices();
            Setup_Controller();
        }

        private void Setup_Controller()
        {
            controller = new SentMessagesController();
            controller.QuerySms = querySms;
        }

        private void MockServices()
        {
            querySms = MockRepository.GenerateMock<IQueryService<SentSms>>();
        }

        public IQueryable<SentSms> PageOfData(SentMessagesIndexModel indexModel)
        {
            List<SentSms> smsList = new List<SentSms>();

            for (int i = indexModel.start.Value; i < indexModel.limit.Value; i++)
            {
                smsList.Add(new SentSms
                {
                    PhoneNumber = PHONENUMBER,
                    Message = MESSAGE+i,
                    SentDate = DateTime.UtcNow,
                    Response = RESPONSE

                });
            }
            return smsList.AsQueryable();
        }
    }
}
