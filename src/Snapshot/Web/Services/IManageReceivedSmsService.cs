﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain;

namespace Web.Services
{
    public interface IManageReceivedSmsService
    {
        RawSmsReceived AssignOutpostToRawSmsReceivedBySenderNumber(RawSmsReceived rawSmsReceived);
        RawSmsReceived ParseRawSmsReceivedFromDrugShop(RawSmsReceived rawSmsReceived);
        RawSmsReceived ParseRawSmsReceivedFromDispensary(RawSmsReceived rawSmsReceived);
        MessageFromDrugShop CreateMessageFromDrugShop(RawSmsReceived rawSmsReceived);
        MessageFromDispensary CreateMessageFromDispensary(RawSmsReceived rawSmsReceived);
        string CreateMessageToBeSentToDispensary(MessageFromDrugShop messageFromDrugShop);
        bool DoesMessageStartWithKeyword(string message);
        bool DoesMessageContainRRCode(MessageFromDrugShop drugshopMessage);
        bool IsMessageForActivation(RawSmsReceived rawSmsReceived);
        void ActivateThePhoneNumber(RawSmsReceived rawSmsReceived);
        bool IsAttendingReminderAnswer(string message, string sender, out int answer);
    }
}