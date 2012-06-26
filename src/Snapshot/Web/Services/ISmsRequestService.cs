using System;
using System.Linq;
using Domain;

namespace Web.Services
{
    public interface ISmsRequestService
    {
        bool SendMessage(string message, RawSmsReceived rawSms);
        bool SendMessageToDispensary(MessageFromDrugShop message, RawSmsReceived rawSms);
        string CreateMessageToBeSentToDispensary(MessageFromDrugShop messageFromDrugShop);
        bool SendResponseMessage();
    }
}