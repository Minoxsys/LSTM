using System;
using System.Linq;
using Domain;

namespace Web.Services
{
    public interface ISmsRequestService
    {
        bool SendMessage(string message, string number);
        bool SendMessageToDispensary(MessageFromDrugShop message);
        string CreateMessageToBeSentToDispensary(MessageFromDrugShop messageFromDrugShop);
    }
}