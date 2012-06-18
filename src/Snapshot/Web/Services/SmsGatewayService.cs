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

        public string SendSmsRequest(string smsRequest)
        {
            string postData = GetPostDataFromSettingsAndSmsRequest(smsRequest);
            string postResponse = httpService.Post(smsGatewaySettingsService.SmsGatewayUrl, postData);
            return postResponse;
        }

        private string GetPostDataFromSettingsAndSmsRequest(string smsRequest)
        {
            StringBuilder postDataBuilder = new StringBuilder();
            postDataBuilder.Append("login=" + smsGatewaySettingsService.SmsGatewayUserName);
            postDataBuilder.Append("&password=" + smsGatewaySettingsService.SmsGatewayPassword);
            postDataBuilder.Append("&shortcode=" + smsGatewaySettingsService.SmsGatewayShortcode);
            postDataBuilder.Append("&keyword=" + smsGatewaySettingsService.SmsGatewayKeyword);
            postDataBuilder.Append("&message=" + HttpUtility.HtmlEncode(smsRequest));
            return postDataBuilder.ToString();
        }
    }
}