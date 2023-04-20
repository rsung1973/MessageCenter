using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Utility;
using WebHome.Properties;

namespace WebHome.Helper
{
    public static class MessageExtensions
    {
        public static void PushLineMessage(this JObject jsonData)
        {
            jsonData.ToString().PushLineMessage();
        }

        public static void PushLineMessage(this String dataItem)
        {
            Task.Run(() =>
            {
                try
                {
                    using (WebClient client = new WebClient())
                    {
                        var encoding = new UTF8Encoding(false);
                        client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                        client.Headers.Add("Authorization", $"Bearer {Settings.Default.ChannelToken}");

                        var result = client.UploadData(Settings.Default.LinePushMulticast, encoding.GetBytes(dataItem));

                        Logger.Info($"push:{dataItem},result:{(result != null ? encoding.GetString(result) : "")}");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            });
        }

    }
}