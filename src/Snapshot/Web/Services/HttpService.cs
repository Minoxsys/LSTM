using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Text;
using Web.Bootstrap;

namespace Web.Services
{
    public class HttpService : IHttpService
    {
        private string URL = AppSettings.SmsGatewayUrl;

        public string Post(string data)
        {
            string url = URL + data;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            var result = "";
            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            {
                result = sr.ReadToEnd();
                sr.Close();
            }

            return result;
        }
    }
}