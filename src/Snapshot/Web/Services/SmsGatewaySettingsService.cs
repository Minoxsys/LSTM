using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Bootstrap;

namespace Web.Services
{
    public class SmsGatewaySettingsService : ISmsGatewaySettingsService
    {
        public virtual string SmsGatewayUrl
        {
            get
            {
                return AppSettings.SmsGatewayUrl;
            }
        }

        public virtual string SmsGatewayUserName
        {
            get
            {
                return AppSettings.SmsGatewayUserName;
            }
        }

        public virtual string SmsGatewayPassword
        {
            get
            {
                return AppSettings.SmsGatewayPassword;
            }
        }

        public virtual string SmsGatewayShortcode
        {
            get
            {
                return AppSettings.SmsGatewayShortcode;
            }
        }

        public virtual string SmsGatewayKeyword
        {
            get
            {
                return AppSettings.SmsGatewayKeyword;
            }
        }
    }
}