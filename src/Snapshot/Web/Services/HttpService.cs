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
            WebRequest request = WebRequest.Create(URL);
            request.Method = "POST";
            request.ContentLength = Encoding.UTF8.GetByteCount(data);
            request.ContentType = "application/x-www-form-urlencoded";

            StreamWriter myWriter = null;
            try
            {
                myWriter = new StreamWriter(request.GetRequestStream());
                myWriter.Write(data);
                myWriter.Flush();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                myWriter.Close();
            }

            String result = "";
            HttpWebResponse objResponse = (HttpWebResponse)request.GetResponse();
            using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
            {
                result = sr.ReadToEnd();
                sr.Close();
            }

            return result;
        }
    }
}