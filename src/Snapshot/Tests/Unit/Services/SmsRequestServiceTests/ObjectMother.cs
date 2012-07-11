using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Web.Services;
using Persistence.Queries.Outposts;
using Core.Persistence;
using Domain;
using Rhino.Mocks;

namespace Tests.Unit.Services.SmsRequestServiceTests
{
    public class ObjectMother
    {
        public ISmsRequestService service;

        public IHttpService httpService;
        public IQueryOutposts queryOutposts;
        public IQueryService<Contact> queryContact;

        public const string CORRECTNUMBER = "0747858959";
        public const string WRONGNUMBER = "0000000000";
        public const string MESSAGE = "Some message here";
        public string CORRECTMESSAGE = "10000008 " + DateTime.UtcNow.ToString("ddMMyy") + "DrugShop1 XY" + DateTime.UtcNow.AddYears(-20).ToString("ddMMyy") + "F S0 S1 S2";

        public Guid outpostId;
        public Outpost outpost;
        public Guid warehouseId;
        public Outpost warehouse;
        public Guid contactId;
        public Contact contact;
        public Guid messageFromDrugShopId;
        public MessageFromDrugShop messageFromDrugShop;
        public Guid rawSmsId;
        public RawSmsReceived rawSms;

        public void Init()
        {
            MockServices();
            Setup_Service();
            SetUp_StubData();
        }

        private void MockServices()
        {
            httpService = MockRepository.GenerateMock<IHttpService>();
            queryOutposts = MockRepository.GenerateMock<IQueryOutposts>();
            queryContact = MockRepository.GenerateMock<IQueryService<Contact>>();
        }

        private void Setup_Service()
        {
            service = new SmsRequestService(httpService, queryOutposts, queryContact);
        }

        private void SetUp_StubData()
        {
            contactId = Guid.NewGuid();
            contact = MockRepository.GeneratePartialMock<Contact>();
            contact.Stub(c => c.Id).Return(contactId);
            contact.IsMainContact = true;
            contact.ContactDetail = CORRECTNUMBER;
            contact.ContactType = Contact.MOBILE_NUMBER_CONTACT_TYPE;

            warehouseId = Guid.NewGuid();
            warehouse = MockRepository.GeneratePartialMock<Outpost>();
            warehouse.Stub(c => c.Id).Return(warehouseId);
            warehouse.Contacts.Add(contact);
            warehouse.Name = "Dispensary";

            contact.Outpost = warehouse;

            outpostId = Guid.NewGuid();
            outpost = MockRepository.GeneratePartialMock<Outpost>();
            outpost.Stub(c => c.Id).Return(outpostId);
            outpost.Warehouse = warehouse;
            outpost.Name = "DrugShop1";

            rawSmsId = Guid.NewGuid();
            rawSms = MockRepository.GeneratePartialMock<RawSmsReceived>();
            rawSms.Stub(c => c.Id).Return(rawSmsId);
            rawSms.Sender = CORRECTNUMBER;

            messageFromDrugShopId = Guid.NewGuid();
            messageFromDrugShop = MockRepository.GeneratePartialMock<MessageFromDrugShop>();
            messageFromDrugShop.Stub(c => c.Id).Return(messageFromDrugShopId);
            messageFromDrugShop.BirthDate = DateTime.UtcNow.AddYears(-20);
            messageFromDrugShop.Gender = "F";
            messageFromDrugShop.IDCode = "10000008";
            messageFromDrugShop.Initials = "XY";
            messageFromDrugShop.OutpostId = Guid.NewGuid();
            messageFromDrugShop.SentDate = DateTime.UtcNow;
            messageFromDrugShop.OutpostId = outpostId;

            for (int i = 0; i < 3; i++)
            {
                messageFromDrugShop.ServicesNeeded.Add(new Condition
                {
                    Code = "S" + i,
                    Description = "some Description",
                    Keyword = "K" + i
                });
            }
        }
    }
}
