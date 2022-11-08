using WebHome.Helper;
using WebHome.Models.Helper;
using WebHome.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using WebHome.BA_Service;
using WebHome.Models.DataEntity;
using WebHome.Models.ViewModel;
using Utility;

namespace WebHome.DataPort
{
    public class MessageOutbound
    {
        public static MessageOutbound _instance;

        static MessageOutbound()
        {
            _instance = new MessageOutbound();
        }

        public static MessageOutbound Instance
        {
            get
            {
                return _instance;
            }
        }

        private KeyValuePair<String, DateTime>? _authToken;

        private MessageOutbound()
        {

        }

        public KeyValuePair<String, DateTime>? CheckAuthToken()
        {
            if (!_authToken.HasValue || (DateTime.Now - _authToken.Value.Value).TotalMinutes >= Settings.Default.ValidTokenDurationInMinutes)
            {
                ResetToken();
            }
            return _authToken;
        }

        public void ResetToken()
        {
            _authToken = null;

            dynamic queryValues = new DynamicQueryStringParameter();
            queryValues.prm_id = Settings.Default.CenterID;

            if (AppSettings.Default.UseCustomBA)
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    var json = client.UploadString(Settings.Default.GetAuthToken , queryValues.ToJsonString());
                    Logger.Info(Settings.Default.GetAuthToken + queryValues.ToQueryString());
                    Logger.Info(json);
                    JObject result = JObject.Parse(json);
                    if (result.ContainsKey("auth_code"))
                    {
                        _authToken = new KeyValuePair<string, DateTime>(result.Value<String>("auth_code"), DateTime.MaxValue);
                    }
                }

            }
            else
            {
                using (WebClient client = new WebClient())
                {
                    Logger.Debug(Settings.Default.GetAuthToken + queryValues.ToQueryString());
                    var json = client.DownloadString(Settings.Default.GetAuthToken + queryValues.ToQueryString());
                    var result = JsonConvert.DeserializeObject(json);
                    if (result.Count > 0)
                    {
                        _authToken = new KeyValuePair<string, DateTime>(Encoding.UTF8.GetString(Convert.FromBase64String((String)result[0].auth_code)), DateTime.Now);
                    }
                }
            }
        }

        public JArray ApplyBuildingInfo()
        {
            CheckAuthToken();

            if (_authToken.HasValue)
            {

                dynamic queryValues = new DynamicQueryStringParameter();
                queryValues.prm_auth_code = _authToken.Value.Key;
                queryValues.prm_building_id = 9999;
                queryValues.prm_floor_id = 9999;

                using (WebClient client = new WebClient())
                {
                    Logger.Debug(Settings.Default.GetBuildingInfo + queryValues.ToQueryString());
                    var json = client.DownloadString(Settings.Default.GetBuildingInfo + queryValues.ToQueryString());
                    JArray result = JsonConvert.DeserializeObject(json) as JArray;
                    if (result != null && result.Count > 0)
                    {
                        result.DecodeValue();
                        return result;
                    }
                }
            }
            return null;
        }

        public JArray QueryAllDevices()
        {
            CheckAuthToken();

            if (_authToken.HasValue)
            {

                dynamic queryValues = new DynamicQueryStringParameter();
                queryValues.prm_auth_code = _authToken.Value.Key;
                queryValues.prm_l1_device_type = Settings.Default.PRMType;
                queryValues.prm_uri = "NONE";
                queryValues.prm_query_mode = "A";

                using (WebClient client = new WebClient())
                {
                    var json = client.DownloadString(Settings.Default.QueryAllDevices + queryValues.ToQueryString());
                    JArray result = JsonConvert.DeserializeObject(json) as JArray;
                    if (result != null && result.Count > 0)
                    {
                        result.DecodeValue();
                        return result;
                    }
                }
            }
            return null;
        }

        public JArray GetResidentInfo()
        {
            CheckAuthToken();

            if (_authToken.HasValue)
            {

                dynamic queryValues = new DynamicQueryStringParameter();
                queryValues.prm_auth_code = _authToken.Value.Key;

                using (WebClient client = new WebClient())
                {
                    var json = client.DownloadString(Settings.Default.GetResidentInfo + queryValues.ToQueryString());
                    JArray result = JsonConvert.DeserializeObject(json) as JArray;
                    if (result != null && result.Count > 0)
                    {
                        result.DecodeValue();
                        return result;
                    }
                }
            }
            return null;
        }

        public JArray GetResidentMessage(String residentID)
        {
            CheckAuthToken();

            if (_authToken.HasValue)
            {

                dynamic queryValues = new DynamicQueryStringParameter();
                queryValues.prm_auth_code = _authToken.Value.Key;
                queryValues.prm_a01_class = "-9999";
                queryValues.prm_resident_id = residentID;

                using (WebClient client = new WebClient())
                {
                    var json = client.DownloadString(Settings.Default.GetResidentMessage + queryValues.ToQueryString());
                    JArray result = JsonConvert.DeserializeObject(json) as JArray;
                    if (result != null && result.Count > 0)
                    {
                        result.DecodeValue();
                        return result;
                    }
                }
            }
            return null;
        }

        public JArray ReportEnergyDegree(EnergyQueryViewModel viewModel, int actionType = 0)
        {
            CheckAuthToken();

            if (_authToken.HasValue)
            {

                dynamic queryValues = new DynamicQueryStringParameter();
                queryValues.prm_auth_code = _authToken.Value.Key;
                queryValues.prm_resident_id = viewModel.ResidentID;
                queryValues.prm_energy_type = "1";
                queryValues.prm_energy_yyyy = String.Format("{0:0000}", viewModel.Year);
                queryValues.prm_energy_mm = String.Format("{0:00}", viewModel.Month);

                string actionUrl = null;
                switch (actionType)
                {
                    case 0:
                        queryValues.prm_degree = viewModel.Degree;
                        actionUrl = Settings.Default.EnergyDegreeInsert;
                        break;

                    case 1:
                        queryValues.prm_degree = viewModel.Degree;
                        actionUrl = Settings.Default.EnergyDegreeUpdate;
                        break;

                    case 2:
                        actionUrl = Settings.Default.EnergyDegreeDelete;
                        break;

                    case 3:
                        actionUrl = Settings.Default.EnergyDegreeQuery;
                        break;

                }

                using (WebClient client = new WebClient())
                {
                    var json = client.DownloadString(actionUrl + queryValues.ToQueryString());
                    JArray result = JsonConvert.DeserializeObject(json) as JArray;
                    if (result != null && result.Count > 0)
                    {
                        result.DecodeValue();
                        return result;
                    }
                }
            }
            return null;
        } 


        public JArray InsertDevice(dynamic queryValues)
        {
            return ProcessDevice(Settings.Default.InsertDevice, queryValues);
        }

        public JArray DeleteDevice(dynamic queryValues)
        {
            return ProcessDevice(Settings.Default.DeleteDevice, queryValues);
        }

        public JArray ProcessDevice(String actionUrl,dynamic queryValues)
        {
            CheckAuthToken();

            if (_authToken.HasValue)
            {
                queryValues.prm_auth_code = _authToken.Value.Key;

                if (AppSettings.Default.UseCustomBA)
                {
                    String jsonRequest = queryValues.ToJsonString();

                    using (WebClient client = new WebClient())
                    {
                        //Logger.Debug(Settings.Default.GetAuthToken + queryValues.ToQueryString());
                        client.Headers[HttpRequestHeader.ContentType] = "application/json";
                        var json = client.UploadString(actionUrl, jsonRequest);
                        Logger.Info($"{actionUrl} => {jsonRequest}");
                        Logger.Info(json);
                        JObject result = JObject.Parse(json);
                        return new JArray(result);
                    }
                }
                else
                {

                    using (WebClient client = new WebClient())
                    {
                        Logger.Debug(actionUrl + queryValues.ToQueryString());
                        var json = client.DownloadString(actionUrl + queryValues.ToQueryString());
                        JArray result = JsonConvert.DeserializeObject(json) as JArray;
                        if (result != null && result.Count > 0)
                        {
                            result.DecodeValue();
                            return result;
                        }
                    }
                }

            }
            return null;
        }

        public JArray UpdateDeviceStatus(String deviceUri,String status)
        {
            CheckAuthToken();

            if (_authToken.HasValue)
            {
                if(AppSettings.Default.UseCustomBA)
                {
                    String jsonRequest = (new
                    {
                        prm_auth_code = _authToken.Value.Key,
                        l1_device_type = Settings.Default.PRMType,
                        device_uri = deviceUri,
                        device_status = status,
                        device_status_value = "",
                        status_date = DateTime.Now,
                    }).JsonStringify();

                    using (WebClient client = new WebClient())
                    {
                        //Logger.Debug(Settings.Default.GetAuthToken + queryValues.ToQueryString());
                        client.Headers[HttpRequestHeader.ContentType] = "application/json";
                        var json = client.UploadString(Settings.Default.MessageCenter_BA_Service_sr_BA_DeviceWebService, jsonRequest);
                        Logger.Info($"{Settings.Default.MessageCenter_BA_Service_sr_BA_DeviceWebService} => {jsonRequest}");
                        Logger.Info(json);
                        JObject result = JObject.Parse(json);
                        return new JArray(result);
                    }

                }
                else
                {
                    using (sr_BA_DeviceWebService service = new sr_BA_DeviceWebService())
                    {
                        String json = service.Set_Status(_authToken.Value.Key, Settings.Default.PRMType, deviceUri, status, "", DateTime.Now);
                        JArray result = JsonConvert.DeserializeObject(json) as JArray;
                        if (result != null && result.Count > 0)
                        {
                            ((dynamic)result[0]).result = ((dynamic)result[0]).reulst;
                            return result;
                        }
                    }
                }
            }
            return null;

        }

        public JArray ReportDeviceEvent(String deviceUri, String status,String eventType)
        {
            CheckAuthToken();

            if (_authToken.HasValue)
            {
                if (AppSettings.Default.UseCustomBA)
                {
                    String jsonRequest = (new
                    {
                        prm_auth_code = _authToken.Value.Key,
                        l1_device_type = Settings.Default.PRMType,
                        device_uri = deviceUri,
                        need_release = true,
                        event_type = eventType,
                        event_sub_type = "0",
                        event_level = 0,
                        event_timeout = 0,
                        device_status = status,
                        card_no = "",
                        event_detail = "",
                        isLog = false,
                        device_time = DateTime.Now,
                    }).JsonStringify();

                    using (WebClient client = new WebClient())
                    {
                        //Logger.Debug(Settings.Default.GetAuthToken + queryValues.ToQueryString());
                        client.Headers[HttpRequestHeader.ContentType] = "application/json";
                        var json = client.UploadString(Settings.Default.MessageCenter_BA_Service_sr_BA_DeviceWebService, jsonRequest);
                        Logger.Info($"{Settings.Default.MessageCenter_BA_Service_sr_BA_DeviceWebService} => {jsonRequest}");
                        Logger.Info(json);
                        JObject result = JObject.Parse(json);
                        return new JArray(result);
                    }
                }
                else
                {
                    using (sr_BA_DeviceWebService service = new sr_BA_DeviceWebService())
                    {
                        Logger.Debug(service.Url);
                        String json = service.Set_Event_ByDeviceDetail(
                            _authToken.Value.Key,
                            Settings.Default.PRMType,
                            deviceUri,
                            true,
                            eventType,
                            "0",
                            0,
                            0,
                            status,
                            "",
                            "",
                            false,
                            DateTime.Now);
                        JArray result = JsonConvert.DeserializeObject(json) as JArray;
                        if (result != null && result.Count > 0)
                        {
                            ((dynamic)result[0]).result = ((dynamic)result[0]).reulst;

                            if (((dynamic)result[0]).event_id != null)
                            {
                                try
                                {
                                    json = service.New_Rule_Trigger(
                                        _authToken.Value.Key,
                                        Settings.Default.PRMType,
                                        1,
                                        ((dynamic)result[0]).event_id as String,
                                        1);

                                    Logger.Debug("連動回應: " + json);

                                    JArray jsonResult = JsonConvert.DeserializeObject(json) as JArray;
                                    if (jsonResult != null && jsonResult.Count > 0)
                                    {
                                        json = service.Sync_Complete
                                            (
                                            _authToken.Value.Key,
                                            Settings.Default.PRMType,
                                            ((dynamic)jsonResult[0]).reulst == "OK" ? "" : $"{((dynamic)jsonResult[0]).error_msg}");

                                        Logger.Debug("同步回應: " + json);

                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                            }

                            return result;
                        }
                    }
                }
            }
            return null;
        }

        public void ApplyDefaultBuildingInfo()
        {
            var result = ApplyBuildingInfo();
            if (result != null)
            {
                dynamic building = result[0] as dynamic;
                WebHome.Properties.AppSettings.Default.BuildingID = building.building_id;
                WebHome.Properties.AppSettings.Default.FloorID = building.floor_id;
            }
        }

    }
}