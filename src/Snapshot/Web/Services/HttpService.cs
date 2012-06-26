using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Text;

namespace Web.Services
{
    public class HttpService : IHttpService
    {
        public string Post(string url, string data)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(data);

            WebRequest request = WebRequest.Create(url);
            request.Method = "POST";
            request.ContentLength = byteArray.Length;
            request.ContentType = "text/xml; charset=UTF-8";

            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            WebResponse response = request.GetResponse();
            string value = ((HttpWebResponse)response).StatusDescription;
            dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();

            reader.Close();
            dataStream.Close();
            response.Close();

            return value; //ok - for sent messages
        }
    }
}