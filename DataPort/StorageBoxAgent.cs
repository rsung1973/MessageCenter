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

namespace WebHome.DataPort
{
    public class StorageBoxAgent
    {
        public static StorageBoxAgent Instance
                { get; } = new StorageBoxAgent();

        private StorageBoxAgent()
        {
            ViewModel = QueryViewModel.LoadData<StorageBoxViewModel>();
        }

        public void Save()
        {
            ViewModel.SaveData();
        }

        public void InitializeLayer(int layerCount)
        {
            if (layerCount > 0)
            {
                ViewModel.LayerCount = layerCount;
                var storageItem = ViewModel.StorageItem;
                Array.Resize<String>(ref storageItem, layerCount * AppSettings.Default.StorageBox.PortCount);
                ViewModel.StorageItem = storageItem;
                for (int i = 0; i < ViewModel.StorageItem.Length; i++)
                {
                    if (ViewModel.StorageItem[i] == null)
                    {
                        ViewModel.StorageItem[i] = ViewModel.ResidentID;
                        AddBoxItem(i % AppSettings.Default.StorageBox.PortCount, ViewModel.StorageItem[i]);
                    }
                }

                Save();
            }
        }

        public void ResetLayer()
        {
            ViewModel.StorageItem.SetValue(ViewModel.ResidentID, 0);
            ResetBox();
            Save();
        }

        public StorageBoxViewModel ViewModel 
        { 
            get; 
            private set; 
        }

        public void AddBoxItem(int port, String userID)
        {
            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                client.Credentials = new NetworkCredential(AppSettings.Default.StorageBox.UserID, AppSettings.Default.StorageBox.Password);
                NameValueCollection values = new NameValueCollection();
                values["port"] = $"{port}";
                values["room"] = userID;
                values["roomAct"] = "3";

                var data = client.UploadValues($"{AppSettings.Default.StorageBox.StorageBoxUrl}/cgi-bin/advanced.cgi", values);
                Logger.Info(Encoding.UTF8.GetString(data));
            }
        }

        public void RemoveBoxItem(int port, String userID)
        {
            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                client.Credentials = new NetworkCredential(AppSettings.Default.StorageBox.UserID, AppSettings.Default.StorageBox.Password);
                NameValueCollection values = new NameValueCollection();
                values["port"] = $"{port}";
                values["room"] = userID;
                values["roomAct"] = "2";

                var data = client.UploadValues($"{AppSettings.Default.StorageBox.StorageBoxUrl}/cgi-bin/advanced.cgi", values);
                Logger.Info(Encoding.UTF8.GetString(data));
            }
        }

        public void ResetBox()
        {
            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                client.Credentials = new NetworkCredential(AppSettings.Default.StorageBox.UserID, AppSettings.Default.StorageBox.Password);
                NameValueCollection values = new NameValueCollection();
                values["port"] = "0";
                values["room"] = "";
                values["roomAct"] = "1";

                var data = client.UploadValues($"{AppSettings.Default.StorageBox.StorageBoxUrl}/cgi-bin/advanced.cgi", values);
                Logger.Info(Encoding.UTF8.GetString(data));
            }

            for (int i = 0; i < ViewModel.StorageItem.Length; i++)
            {
                ViewModel.StorageItem[i] = ViewModel.ResidentID;
                AddBoxItem(i % AppSettings.Default.StorageBox.PortCount, ViewModel.StorageItem[i]);
            }
        }

        public void RemoveItem(int port, String userID, String replacementID)
        {
            if (!(port < ViewModel.StorageItem.Length
                    && ViewModel.StorageItem[port] == userID))
            {
                return;
            }

            ViewModel.StorageItem[port] = replacementID;

            int arrayLessCount = 0;
            int arrayGreatCount = 0;
            int arrayPort = port % AppSettings.Default.StorageBox.PortCount;
            for (int i = 0; i < ViewModel.StorageItem.Length; i++)
            {
                if (i % AppSettings.Default.StorageBox.PortCount == arrayPort)
                {
                    if (ViewModel.StorageItem[i] == userID)
                    {
                        if (i < port)
                        {
                            arrayLessCount++;
                        }
                        else if (i > port)
                        {
                            arrayGreatCount++;
                        }
                    }
                }
            }

            RemoveBoxItem(port, userID);

            if (arrayLessCount > 0)
            {
                for (int i = 0; i < arrayLessCount; i++)
                {
                    AddBoxItem(port, userID);
                }
            }

            AddBoxItem(port, replacementID);

            if (arrayGreatCount > 0)
            {
                for (int i = 0; i < arrayGreatCount; i++)
                {
                    AddBoxItem(port, userID);
                }
            }

            Save();

        }

        public void StoreItem(int port, String userID)
        {
            RemoveItem(port, ViewModel.ResidentID, userID);
        }

    }

}