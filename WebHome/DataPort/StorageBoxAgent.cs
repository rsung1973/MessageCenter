using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebHome.Properties;
using Utility;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.IO;
using WebHome.Models.ViewModel;
using System.Collections.Specialized;
using DocumentFormat.OpenXml.Vml.Spreadsheet;

namespace WebHome.DataPort
{
    public class StorageBoxAgent
    {
        public StorageBoxAgent(StorageBoxSettings settings)
        {
            ViewModel = settings;
        }

        public static void AddStorageBox()
        {
            lock(typeof(StorageBoxAgent))
            {
                List<StorageBoxSettings> items = new List<StorageBoxSettings>(AppSettings.Default.StorageBoxArray);
                items.Add(new StorageBoxSettings 
                {
                    BoxSize = StorageBoxSize.小,
                });
                AppSettings.Default.StorageBoxArray = items.ToArray();
            }
        }

        public static StorageBoxAgent AcquireAgent(int boxIndex)
        {
            if (AppSettings.Default.StorageBoxArray != null && AppSettings.Default.StorageBoxArray.Length > 0
                && AppSettings.Default.StorageBoxArray.Length > boxIndex && boxIndex >= 0)
            {
                var settings = AppSettings.Default.StorageBoxArray[boxIndex];
                return new StorageBoxAgent(settings);
            }
            return null;
        }

        public static void RemoveStorageBox(int boxIndex)
        {
            lock (typeof(StorageBoxAgent))
            {
                List<StorageBoxSettings> items = new List<StorageBoxSettings>(AppSettings.Default.StorageBoxArray);
                items.RemoveAt(boxIndex);
                AppSettings.Default.StorageBoxArray = items.ToArray();
            }
        }


        public static StorageBoxAgent AcquireAgent(StorageBoxSize size)
        {
            if (AppSettings.Default.StorageBoxArray != null && AppSettings.Default.StorageBoxArray.Length > 0)
            {
                var settings = AppSettings.Default.StorageBoxArray
                    .Where(s => s.Enabled == true)
                    .Where(s => s.BoxSize == size).FirstOrDefault();
                if (settings != null)
                {
                    return new StorageBoxAgent(settings);
                }
            }
            return null;
        }

        public static bool RetrieveAll(String userID)
        {
            int uid;
            bool result = false;
            if (!int.TryParse(userID, out uid))
            {
                uid = BitConverter.ToInt32(Encoding.Default.GetBytes(userID), 0);
            }

            for (int idx = 0; idx < AppSettings.Default.StorageBoxArray.Length; idx++)
            {
                var agent = AcquireAgent(idx);
                if (agent != null && agent.ViewModel.Enabled)
                {
                    var items = agent.GetBoxPortList();
                    if (items?.ports?.Any(p => p.room == uid) == true)
                    {
                        var ports = items.ports.Where(p => p.room == uid);

                        BoxPortAction boxValues = new BoxPortAction
                        {
                            action = "del",
                            ports = ports.Select(p => new BoxItemPort
                            {
                                port = p.port,
                                room = uid,
                            }).ToArray()
                        };

                        agent.DoBox(boxValues);

                        BoxPortRelay relayValues = new BoxPortRelay
                        {
                            relays = ports.Select(p => new PortRelay
                            {
                                id = p.port.Value,
                                timing = agent.ViewModel.RelayTiming,
                            }).ToArray()
                        };

                        agent.TriggerBox(relayValues);
                        result = true;
                    }
                }
            }

            return result;
        }

        public void Save()
        {
            AppSettings.Default.Save();
        }

        public StorageBoxSettings ViewModel 
        { 
            get; 
            private set; 
        }

        public class BoxItemPort
        {
            public int? port { get; set; }
            public int? room { get; set; }
        }

        public class BoxPortList
        {
            public BoxItemPort[] ports { get; set; }
        }

        public class BoxPortAction
        {
            public string action { get; set; }
            public BoxItemPort[] ports { get; set; }
        }

        public BoxPortList GetBoxPortList()
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    client.Credentials = new NetworkCredential(ViewModel.UserID, ViewModel.Password);

                    String url = $"http://{ViewModel.StorageBoxUrl}/cgi-bin/webapi.cgi?api=ports";
                    var data = client.DownloadString(url);

                    Logger.Info(url);
                    Logger.Info(data);

                    return JsonConvert.DeserializeObject<BoxPortList>(data);
                }
            }
            catch(Exception ex)
            {
                Logger.Error(ex);
            }

            return null;
        }

        public void AddBoxItem(int port, String userID)
        {
            DoBoxItem(port, userID, "add");
        }

        private void DoBoxItem(int port, String userID,String action)
        {
            int uid;
            if (!int.TryParse(userID, out uid))
            {
                uid = BitConverter.ToInt32(Encoding.Default.GetBytes(userID), 0);
            }

            BoxPortAction values = new BoxPortAction
            {
                action = action,
                ports = new BoxItemPort[]
                {
                        new BoxItemPort
                        {
                            port = port,
                            room = uid,
                        }
                }
            };

            DoBox(values);

            TriggerBoxPort(port, ViewModel.RelayTiming);

        }

        private void DoBox(BoxPortAction values)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    client.Credentials = new NetworkCredential(ViewModel.UserID, ViewModel.Password);
                    client.Headers[HttpRequestHeader.ContentType] = "text/plain";

                    String url = $"http://{ViewModel.StorageBoxUrl}/cgi-bin/webapi.cgi?api=ports";

                    var data = values.JsonStringify();
                    var result = client.UploadString(url, data);
                    Logger.Info(data);
                    Logger.Info(result);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public void RemoveBoxItem(int port, String userID)
        {
            if (port < 0)
                return;

            var items = GetBoxPortList();
            if (items == null || items.ports == null || port >= items.ports.Length)
            {
                return;
            }

            if ($"{items.ports[port].room}" != userID)
            {
                return;
            }

            DoBoxItem(port, userID, "del");

        }

        public BoxItemPort AcquireVacantBox()
        {
            var items = GetBoxPortList()?.ports;
            if (items != null && items.Length > 0)
            {
                for (int i = 0; i < items.Length; i++)
                {
                    if (ViewModel.Disabled.Contains(i))
                    {
                        continue;
                    }

                    var port = items[i];
                    if (!port.room.HasValue)
                    {
                        return port;
                    }
                }
            }
            return null;
        }

        public void ResetBox()
        {
            DoBox(new BoxPortAction { action = "delall" });
        }

        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class PortRelay
        {
            public int id { get; set; }
            public int timing { get; set; }
        }

        public class BoxPortRelay
        {
            public PortRelay[] relays { get; set; }
        }

        public void TriggerBoxPort(int port,int timing = 3000)
        {
            BoxPortRelay values = new BoxPortRelay
            {
                relays = new PortRelay[]
                {
                    new PortRelay
                    {
                        id = port,
                        timing = timing,
                    }
                }
            };

            TriggerBox(values);
        }

        public void TriggerMultiBoxPort(IEnumerable<int> ports, int timing = 3000)
        {
            BoxPortRelay values = new BoxPortRelay
            {
                relays = ports.Select(port => new PortRelay
                {
                    id = port,
                    timing = timing,
                }).ToArray()
            };

            TriggerBox(values);
        }

        private void TriggerBox(BoxPortRelay values)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    client.Credentials = new NetworkCredential(ViewModel.UserID, ViewModel.Password);
                    client.Headers[HttpRequestHeader.ContentType] = "text/plain";

                    String url = $"http://{ViewModel.StorageBoxUrl}/cgi-bin/webapi.cgi?api=relays";

                    var data = values.JsonStringify();
                    var result = client.UploadString(url, data);
                    Logger.Info(data);
                    Logger.Info(result);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

    }

}