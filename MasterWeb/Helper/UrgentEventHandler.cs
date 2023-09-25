using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinqToDB.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebHome.Models.Locale;

namespace WebHome.Helper
{
    public class UrgentEventHandler
    {
        private static UrgentEventHandler _instance;

        static UrgentEventHandler()
        {
            _instance = new UrgentEventHandler();
        }

        private dynamic _message;

        private UrgentEventHandler()
        {

        }

        public static UrgentEventHandler Instance
        {
            get
            {
                lock(typeof(UrgentEventHandler))
                {
                    if (_instance == null)
                    {
                        _instance = new UrgentEventHandler();
                    }
                    return _instance;
                }
            }
        }

        public List<DeviceStatus> DeviceStatusList
        {
            get;
        } = new List<DeviceStatus>();

        public void PushMessage(String json)
        {
            if (!String.IsNullOrEmpty(json))
            {
                JArray data = JsonConvert.DeserializeObject(json) as JArray;
                if (data != null && data.Count > 0)
                {
                    _message = data[0];
                }
            }
        }

        public void Reset()
        {
            _message = null;
        }

        public bool CurrentFire
        {
            get
            {
                return _message != null && _message.messageType == "R" && _message.Action == "F";
            }
        }

        public int? MainDoorStatus
        {
            get; set;
        }

        public bool CurrentEarthquake
        {
            get
            {
                return _message != null && _message.messageType == "R" && _message.Action == "EE";
            }
        }

        public bool Clear
        {
            get
            {
                return _message != null && _message.messageType == "R" && _message.Action == "Clear";
            }
        }



    }

    public class DeviceStatus
    {
        public String InstanceID { get; set; }
        public Naming.DefenceStatus? DefenceStatus { get; set; }
    }
}