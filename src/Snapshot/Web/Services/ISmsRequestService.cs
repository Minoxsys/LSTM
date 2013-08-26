using System;
using System.Linq;
using Domain;

namespace Web.Services
{
    public interface ISmsRequestService
    {
        bool SendMessage(string message, RawSmsReceived rawSms);
        bool SendMessageToDispensary(string message, RawSmsReceived rawSms);
        bool SendMessage(string message, string patientPhoneNumber);
    }
}