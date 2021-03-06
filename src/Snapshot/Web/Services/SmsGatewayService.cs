﻿using System;
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

        public SmsGatewayService(IHttpService httpService)
        {
            this.httpService = httpService;
        }

        public string SendSmsRequest(string smsRequest)
        {
            string postResponse = httpService.Post(smsRequest);
            return postResponse;
        }
    }
}