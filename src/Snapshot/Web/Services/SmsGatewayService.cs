using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using Persistence.Queries.Outposts;
using Core.Persistence;
using System.Globalization;

namespace Web.Services
{
    public class SmsGatewayService : ISmsGatewayService
    {
        private IHttpService httpService;
        private ISmsGatewaySettingsService smsGatewaySettingsService;

        public SmsGatewayService(ISmsGatewaySettingsService smsGatewaySettingsService, IHttpService httpService)
        {
            this.httpService = httpService;
            this.smsGatewaySettingsService = smsGatewaySettingsService;
        }

        public string SendSmsRequest(SmsRequest smsRequest)
        {
            string postData = GetPostDataFromSettingsAndSmsRequest(smsRequest);
            string postResponse = httpService.Post(smsGatewaySettingsService.SmsGatewayUrl, postData);
            return postResponse;
        }

        private string GetPostDataFromSettingsAndSmsRequest(SmsRequest smsRequest)
        {
            return GetPostDataForSmsRequestFromSettings() + "&" + GetPostDataFromSmsRequest(smsRequest);
        }

        private string GetPostDataForSmsRequestFromSettings()
        {
            StringBuilder postDataBuilder = new StringBuilder();
            postDataBuilder.Append("uname=" + smsGatewaySettingsService.SmsGatewayUserName);
            postDataBuilder.Append("&pword=" + smsGatewaySettingsService.SmsGatewayPassword);
            postDataBuilder.Append("&from=" + smsGatewaySettingsService.SmsGatewayFrom);
            postDataBuilder.Append("&test=" + smsGatewaySettingsService.SmsGatewayTestMode);
            postDataBuilder.Append("&info=" + smsGatewaySettingsService.SmsGatewayDebugMode);

            return postDataBuilder.ToString();
        }

        private string GetPostDataFromSmsRequest(SmsRequest smsRequest)
        {
            StringBuilder postDataBuilder = new StringBuilder();
            postDataBuilder.Append("selectednums=" + smsRequest.Number);
            postDataBuilder.Append("&custom=" + smsRequest.Id);
            postDataBuilder.Append("&message=" + HttpUtility.HtmlEncode(smsRequest.Message));

            return postDataBuilder.ToString();
        }
    }
}