using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Services
{
    public interface ISmsGatewaySettingsService
    {
        string SmsGatewayUrl { get; }
        string SmsGatewayUserName { get; }
        string SmsGatewayPassword { get; }
        string SmsGatewayShortcode { get; }
        string SmsGatewayKeyword { get; }

    }
}