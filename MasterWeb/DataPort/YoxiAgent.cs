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

namespace WebHome.DataPort
{
    public class YoxiAgent
    {

        public const int TripOrderWaitingIntervalInSeconds = 30;

        public static YoxiAgent PrepareTaxiAgent(YoxiAgentViewModel viewModel)
        {
            if (viewModel.spotAccount == null)
            {
                viewModel.spotAccount = AppSettings.Default.YoxiTaxi.DefaultSpotAccount;
            }

            YoxiAgent agent = new YoxiAgent(viewModel);
            return agent;
        }

        public YoxiAgentViewModel AuthRequest
        { 
            get; 
            set; 
        }

        public YoxiAuthResponse AuthResponse
        {
            protected set;
            get;
        }

        public YoxiAgent(YoxiAgentViewModel request)
        {
            AuthRequest = request;
        }


        public class AuthData
        {
            public string token { get; set; }
        }

        public class YoxiAuthResponse : DispatchResponse
        {
            public AuthData data { get; set; }
        }


        public String AccessToken
        {
            get
            {
                if (AuthResponse == null)
                {
                    RefreshAccessToken();
                }

                return AuthResponse?.data?.token;
            }
        }

        public bool RefreshAccessToken()
        {
            try
            {
                AuthResponse = null;
                using (WebClient client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    String request = AuthRequest.JsonStringify();
                    Logger.Info("RefreshAccessToken:" + request);
                    var json = client.UploadString(AppSettings.Default.YoxiTaxi.ConnectToProxy, request);
                    Logger.Info("RefreshAccessToken:" + json);
                    AuthResponse = JsonConvert.DeserializeObject<YoxiAuthResponse>(json);
                    return AuthResponse.success;
                }
            }
            catch(Exception ex)
            {
                Logger.Error(ex);
            }

            return false;
        }


        void LogOrder(DispatchOrderResponse result)
        {
            String storePath = Path.Combine(AppSettings.Default.YoxiTaxi.LoggingPath)
                                    .CheckStoredPath();

            AuthRequest.BookDate = DateTime.Now;
            File.WriteAllText(Path.Combine(storePath, $"Yoxi-{AuthRequest.ResidentID}.json"), AuthRequest.JsonStringify());
            for (int i = 0; i < result.data.Length; i++)
            {
                File.WriteAllText(Path.Combine(storePath, $"TripOrder-{result.data[i].hashId}-{AuthRequest.ResidentID}-{DateTime.Today:yyyyMMdd}.json"), AuthRequest.JsonStringify());
            }
        }

        public void DeleteTrip(String hashId)
        {
            String storePath = Path.Combine(AppSettings.Default.YoxiTaxi.LoggingPath)
                                    .CheckStoredPath();
            foreach (var item in Directory.GetFiles(storePath, $"TripOrder-{hashId}*.json"))
            {
                File.Delete(item);
            }
        }

        public T LoadData<T>(String fileName)
        {
            String filePath = Path.Combine(AppSettings.Default.YoxiTaxi.LoggingPath, fileName);
            if (File.Exists(filePath))
            {
                T result = JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath));
                return result;
            }
            return default;
        }

        public IEnumerable<String> GetTripOrderList()
        {
            IEnumerable<String> items = Directory.GetFiles(AppSettings.Default.YoxiTaxi.LoggingPath.CheckStoredPath(), "TripOrder*.json");
            if (AuthRequest.ResidentID != null)
            {
                items = items.Where(n => n.Contains(AuthRequest.ResidentID));
            }
            return items;
        }

        public DispatchOrderResponse DispatchOrder()
        {
            try
            {
                TripOrder order = new TripOrder
                {
                    carNumber = AuthRequest.Amount,
                    remark = AuthRequest.Memo,
                    riderName = AuthRequest.CustName,
                    riderPhone = AuthRequest.CustPhone,
                    spotAccount = AuthRequest.spotAccount,
                };

                using (WebClient client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    client.Headers["token"] = AccessToken;
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    var request = order?.JsonStringify();
                    Logger.Info("DispatchOrder:" + request);
                    var json = client.UploadString(AppSettings.Default.YoxiTaxi.DispatchOrder, request);
                    Logger.Info("DispatchOrder:" + json);
                    var result = JsonConvert.DeserializeObject<DispatchOrderResponse>(json);
                    if(result.success)
                    {
                        Task.Run(() =>
                        {
                            LogOrder(result);
                        });
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return null;
        }

        public void UpdateSpotAccount()
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    client.Headers["token"] = AccessToken;
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    var request = (new
                            {
                                AuthRequest.spotAccount,
                                name = AuthRequest.Memo ?? AuthRequest.spotAccount,
                                city = AuthRequest.City,
                                town = AuthRequest.District,
                                address = String.Concat(
                                            AuthRequest.Road,
                                            AuthRequest.Section.GetEfficientString("段"),
                                            AuthRequest.Lane.GetEfficientString("巷"),
                                            AuthRequest.Alley.GetEfficientString("弄"),
                                            $"{(AuthRequest.No.GetEfficientString() ?? "0")}號",
                                            AuthRequest.UnitNo.HasValue ? $"之{AuthRequest.UnitNo}" : null),
                                lat = AuthRequest.Latitude,
                                lng = AuthRequest.Longitude
                            }
                        ).JsonStringify();
                    Logger.Info("UpdateSpotAccount:" + request);
                    var json = client.UploadString(AppSettings.Default.YoxiTaxi.UpdateSpotAccount, request);
                    if (json != null)
                    {
                        Logger.Info("UpdateSpotAccount:" + json);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public DispatchQueryResponse DispatchQuery(String hashId)
        {                
            using (WebClient client = new WebClient())
            {
                try
                {

                    client.Encoding = Encoding.UTF8;
                    client.Headers["token"] = AccessToken;
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    String url = $"{AppSettings.Default.YoxiTaxi.DispatchQuery}?spotAccount={AuthRequest.spotAccount}&hashId={hashId}";
                    Logger.Info("DispatchQuery:" + url);
                    var json = client.DownloadString(url);
                    Logger.Info("DispatchQuery:" + json);
                    json = json.Replace(hashId, "vehicle");

                    DispatchQueryResponse result = JsonConvert.DeserializeObject<DispatchQueryResponse>(json);
                    if (result.success)
                    {
                        result.data.hashId = hashId;
                    }

                    return result;

                }
                catch(WebException ex)
                {
                    Logger.Info("DispatchQuery:" + ex.Response.GetString(client.Encoding));
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }
            return null;
        }


        public DispatchResponse DispatchCancel(String hashId)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    client.Headers["token"] = AccessToken;
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    String request = (new
                    {
                        AuthRequest.spotAccount,
                        hashId = hashId,
                    }).JsonStringify();
                    Logger.Info("DispatchCancel:" + request);
                    var json = client.UploadString(AppSettings.Default.YoxiTaxi.DispatchCancel, request);
                    Logger.Info("DispatchCancel:" + json);
                    var result = JsonConvert.DeserializeObject<DispatchResponse>(json);

                    return result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return null;
        }


        public class DispatchResponse
        {
            public bool success { get; set; }
            public string error { get; set; }
            public int? errorCode { get; set; }
        }


        public class DispatchOrderResponse : DispatchResponse
        {
            public OrderResult[] data { get; set; }
        }

        public class DispatchQueryResponse : DispatchResponse
        {
            public DispatchQueryResult data { get; set; }
        }

        public class DispatchQueryResult
        {
            public string status { get; set; }
            public OrderInfo orderInfo { get; set; }
            public string spotAccount { get; set; }
            public string hashId { get; set; }

        }

        public class OrderInfo
        {
            public string driverName { get; set; }
            public string driverPhone { get; set; }
            public string vehicleColor { get; set; }
            public string vehicleLicensePlate { get; set; }
            public string vehicleBrandName { get; set; }
            public string vehicleSeriesName { get; set; }
            public int? duration { get; set; }
        }


        public class TripOrder
        {
            public string spotAccount { get; set; }
            public string riderName { get; set; }
            public string riderPhone { get; set; }
            public int carNumber { get; set; }
            public string remark { get; set; }

        }


        public class OrderResult
        {
            public int carIndex { get; set; }
            public string spotAccount { get; set; }
            public string hashId { get; set; }
        }

    }
}