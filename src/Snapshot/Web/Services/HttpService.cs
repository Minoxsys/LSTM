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
        WebRequest webRequest;

        public void SetWebRequest(WebRequest webRequest)
        {
            this.webRequest = webRequest;
        }

        private void PrepareWebRequestForPost(string url, string data)
        {
            webRequest = (HttpWebRequest)WebRequest.Create(url);

            byte[] postDataBytes = Encoding.UTF8.GetBytes(data);

            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ContentLength = postDataBytes.Length;


            //webRequest.ContentType = "application/x-www-form-urlencoded";
            //webRequest.ContentType = "text/xml";
            //webRequest.ContentLength = Encoding.UTF8.GetByteCount(data);
            //webRequest.ContentLength = data.Length;
            
        }

        private void SendRequest(string data)
        {
            StreamWriter myWriter = null;
            try
            {
                myWriter = new StreamWriter(webRequest.GetRequestStream());
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
        }

        private string GetResponse()
        {
            String result = "";

            HttpWebResponse objResponse = (HttpWebResponse)webRequest.GetResponse();
            using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
            {
                result = sr.ReadToEnd();
                // Close and clean up the StreamReader
                sr.Close();
            }

            return result;
        }

        public string Post(string url, string data)
        {
            //PrepareWebRequestForPost(url, data);
            
            //SendRequest(data);

            webRequest = (HttpWebRequest)WebRequest.Create(url);

            byte[] postDataBytes = Encoding.UTF8.GetBytes(data);

            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ContentLength = postDataBytes.Length;

            StreamWriter myWriter = null;
            try
            {
                myWriter = new StreamWriter(webRequest.GetRequestStream());
                myWriter.Write(postDataBytes);
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


            //Stream requestStream = webRequest.GetRequestStream();
            //requestStream.Write(postDataBytes, 0, postDataBytes.Length);
            //requestStream.Close();

            //String result = "";
            //var resp = (HttpWebResponse)webRequest.GetResponse();
            //StreamReader responseReader = new StreamReader(resp.GetResponseStream(), Encoding.UTF8);
            //result = responseReader.ReadToEnd();
            //resp.Close();

            return GetResponse();
        }
    }
}