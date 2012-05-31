using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain;

namespace Web.Services
{
    public interface IManageReceivedSmsService
    {
        Guid AssignOutpostToRawSmsReceivedBySenderNumber(RawSmsReceived rawSmsReceived);
        RawSmsReceived ParseRawSmsReceivedFromDrugShop(RawSmsReceived rawSmsReceived);
        RawSmsReceived ParseRawSmsReceivedFromDispensary(RawSmsReceived rawSmsReceived);
        MessageFromDrugShop CreateMessageFromDrugShop(RawSmsReceived rawSmsReceived);
        MessageFromDispensary CreateMessageFromDispensary(RawSmsReceived rawSmsReceived);
    }
}