using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace WebHome.Helper
{
    public static class GCM
    {
        public static void SendGCM()
        {
            string BrowserAPIKey = "AIzaSyDrDyQRVavEWLBRYnsJmwy3mSksPqm9_qo";
            string message = "Hello,GCM!!";
            string tickerText = "example test GCM";
            string contentTitle = "content title GCM";
            string postData = JsonConvert.SerializeObject(new
            {
                registration_ids = new String[] { "這裡請填入要傳送的裝置ID" },
                data = new { tickerText, contentTitle, message }
            });
            string response = SendGCMNotification(BrowserAPIKey, postData);
            string reponse = response;
        }
        //GCM功能
        private static string SendGCMNotification(string apiKey, string postData, string postDataContentType = "application/json")
        {
            //  MESSAGE CONTENT
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            //  CREATE REQUEST
            HttpWebRequest Request = (HttpWebRequest)WebRequest.Create("https://android.googleapis.com/gcm/send");
            Request.Method = "POST";

            Request.KeepAlive = false;
            Request.ContentType = postDataContentType;
            Request.Headers.Add(string.Format("Authorization: key={0}", apiKey));
            Request.ContentLength = byteArray.Length;
            Stream dataStream = Request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            //  SEND MESSAGE
            string error;
            try
            {
                WebResponse Response = Request.GetResponse();
                HttpStatusCode ResponseCode = ((HttpWebResponse)Response).StatusCode;
                if (ResponseCode.Equals(HttpStatusCode.Unauthorized) || ResponseCode.Equals(HttpStatusCode.Forbidden))
                {
                    error = "Unauthorized – need new token";
                }
                else if (!ResponseCode.Equals(HttpStatusCode.OK))
                {
                    error = "Response from web service isn’t OK";
                }
                StreamReader Reader = new StreamReader(Response.GetResponseStream());
                string responseLine = Reader.ReadToEnd();
                Reader.Close();
                return responseLine;
            }
            catch (Exception e)
            {
                error = e.ToString();
            }
            return error;
        }

    }
}