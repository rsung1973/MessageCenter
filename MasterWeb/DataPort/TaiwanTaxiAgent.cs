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
    public class TaiwanTaxiAgent
    {
        public static TaiwanTaxiAgent DefaultAgent
        {
            get;
        } = new TaiwanTaxiAgent(AppSettings.Default.TaiwanTaxi.DefaultAuthRequest, null);

        public const int DispathQueryIntervalInSeconds = 8;

        public static TaiwanTaxiAgent PrepareTaxiAgent(TaiwanTaxiAgentViewModel viewModel)
        {
            TaiwanTaxiAuthRequest authRequest = new TaiwanTaxiAuthRequest
            {
                UserId = viewModel.ResidentID,
                ServiceToken = AppSettings.Default.TaiwanTaxi.DefaultAuthRequest.ServiceToken,
                CompanySecret = AppSettings.Default.TaiwanTaxi.DefaultAuthRequest.CompanySecret
            };

            TaiwanTaxiAgent agent = new TaiwanTaxiAgent(authRequest, viewModel.AuthResponse);
            return agent;
        }

        public TaiwanTaxiAuthRequest AuthRequest
        { 
            get; 
            set; 
        }

        public DispatchAuthResponse AuthResponse
        {
            protected set;
            get;
        }

        public TaiwanTaxiAgent(TaiwanTaxiAuthRequest request, DispatchAuthResponse lastResponse)
        {
            AuthRequest = request;
            AuthResponse = lastResponse ?? GetLastAuthResponse();
        }

        public class ResultObj
        {
            public int? State { get; set; }
            public string Msg { get; set; }
        }

        public class DispatchAuthResponse
        {
            public string AccessToken { get; set; }
            public DateTime? Expired { get; set; }
            public long? ExpiredUTCT { get; set; }
            public long? ServerUTCT { get; set; }
            public ResultObj Result { get; set; }
        }

        public String AccessToken
        {
            get
            {
                if (AuthResponse == null || !AuthResponse.Expired.HasValue || AuthResponse.Expired < DateTime.Now)
                {
                    RefreshAccessToken();
                }

                return AuthResponse?.AccessToken;
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
                    Logger.Info(request);
                    var json = client.UploadString(AppSettings.Default.TaiwanTaxi.DispatchAuth, request);
                    Logger.Info(json);
                    AuthResponse = JsonConvert.DeserializeObject<DispatchAuthResponse>(json);
                    return true;
                }
            }
            catch(Exception ex)
            {
                Logger.Error(ex);
            }

            return false;
        }

        public GISGeocodingResponse GetGISGeoCode(String addr)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    String request = (new
                    {
                        AuthRequest.UserId,
                        AccessToken,
                        Mode = 1,
                        Addr = addr,
                        Signature = GetServiceSignature("Order"),
                    }).JsonStringify();
                    Logger.Info(request);
                    var json = client.UploadString(AppSettings.Default.TaiwanTaxi.GISGeocoding, request);
                    Logger.Info(json);
                    var result = JsonConvert.DeserializeObject<GISGeocodingResponse>(json);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return null;
        }

        void LogOrder(OrderObj order, DispatchOrderResponse result)
        {
            String storePath = Path.Combine(AppSettings.Default.TaiwanTaxi.LoggingPath, AuthRequest.UserId)
                                    .CheckStoredPath();
            File.WriteAllText(Path.Combine(storePath, "DispatchAuthResponse.json"), AuthResponse.JsonStringify());
            File.WriteAllText(Path.Combine(storePath, "OrderObj.json"), order.JsonStringify());
            File.WriteAllText(Path.Combine(storePath, "DispatchOrderResponse.json"), result.JsonStringify());

            String lastCancel = Path.Combine(storePath, "DispatchCancel.json");
            if(File.Exists(lastCancel))
            {
                File.Delete(lastCancel);
            }
        }

        public void LogData(Object dataItem, String fileName)
        {
            String storePath = Path.Combine(AppSettings.Default.TaiwanTaxi.LoggingPath, AuthRequest.UserId)
                                    .CheckStoredPath();
            File.WriteAllText(Path.Combine(storePath, fileName), dataItem.JsonStringify());
        }

        DispatchAuthResponse GetLastAuthResponse()
        {
            String authFile = Path.Combine(AppSettings.Default.TaiwanTaxi.LoggingPath, AuthRequest.UserId, "DispatchAuthResponse.json");
            if(File.Exists(authFile))
            {
                return JsonConvert.DeserializeObject<DispatchAuthResponse>(File.ReadAllText(authFile));
            }
            return null;
        }

        public bool HasCancelled()
        {
            String authFile = Path.Combine(AppSettings.Default.TaiwanTaxi.LoggingPath, AuthRequest.UserId, "DispatchCancel.json");
            return File.Exists(authFile);
        }

        DispatchQueryResponse GetLastQueryResponse()
        {
            String authFile = Path.Combine(AppSettings.Default.TaiwanTaxi.LoggingPath, AuthRequest.UserId, "DispatchQueryResponse.json");
            if (File.Exists(authFile))
            {
                DispatchQueryResponse result = JsonConvert.DeserializeObject<DispatchQueryResponse>(File.ReadAllText(authFile));
                if ((DateTime.Now - result.DispatchTime).TotalSeconds < DispathQueryIntervalInSeconds)
                {
                    return result;
                }
            }
            return null;
        }

        public T LoadData<T>(String fileName)
        {
            String filePath = Path.Combine(AppSettings.Default.TaiwanTaxi.LoggingPath, AuthRequest.UserId, fileName);
            if (File.Exists(filePath))
            {
                T result = JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath));
                return result;
            }
            return default;
        }

        public static IEnumerable<String> GetOrderPathList()
        {
            return Directory.GetDirectories(AppSettings.Default.TaiwanTaxi.LoggingPath.CheckStoredPath())
                        .Where(p => File.Exists(Path.Combine(p, "OrderObj.json")));
        }



        public String GetServiceSignature(String service)
        {
            if (String.IsNullOrEmpty(AccessToken))
            {
                return null;
            }
            else
            {
                String data = $"{AuthRequest.CompanySecret}{AuthRequest.ServiceToken}{AccessToken}{service}";
                using (SHA1 hash = SHA1.Create())
                {
                    return Convert.ToBase64String(hash.ComputeHash(Encoding.UTF8.GetBytes(data)));
                }
            }
        }

        public DispatchOrderResponse DispatchOrder(OrderObj order)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    var request = (new
                    {
                        AuthRequest.UserId,
                        Order = order,
                        AccessToken,
                        Signature = GetServiceSignature("Order"),
                    }).JsonStringify();
                    Logger.Info(request);
                    var json = client.UploadString(AppSettings.Default.TaiwanTaxi.DispatchOrder, request);
                    Logger.Info(json);
                    var result = JsonConvert.DeserializeObject<DispatchOrderResponse>(json);
                    Task.Run(() => 
                    {
                        LogOrder(order, result);
                    });
                    return result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return null;
        }

        public DispatchQueryResponse DispatchQuery()
        {
            try
            {
                DispatchQueryResponse result = GetLastQueryResponse();
                if (result != null)
                    return result;

                using (WebClient client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    String request = (new
                        {
                            AuthRequest.UserId,
                            AccessToken,
                            Signature = GetServiceSignature("Query"),
                        }).JsonStringify();
                    Logger.Info(request);
                    var json = client.UploadString(AppSettings.Default.TaiwanTaxi.DispatchQuery, request);
                    Logger.Info(json);
                    result = JsonConvert.DeserializeObject<DispatchQueryResponse>(json);
                    Task.Run(() =>
                    {
                        LogData(result, "DispatchQueryResponse.json");
                    });
                    return result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return null;
        }


        public DispatchResponse DispatchCancel(String jobID)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    String request = (new
                    {
                        AuthRequest.UserId,
                        AccessToken,
                        Signature = GetServiceSignature("Cancel"),
                        JobId = jobID,
                        Cause = "取消叫車",
                    }).JsonStringify();
                    Logger.Info(request);
                    var json = client.UploadString(AppSettings.Default.TaiwanTaxi.DispatchCancel, request);
                    Logger.Info(json);
                    var result = JsonConvert.DeserializeObject<DispatchQueryResponse>(json);
                    Task.Run(() =>
                    {
                        LogData(result, "DispatchCancel.json");
                    });
                    return result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return null;
        }

        public CommonSettingsResponse CommonSettings()
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    String request = (new
                    {
                        AuthRequest.UserId,
                        AccessToken,
                        Signature = GetServiceSignature("Settings"),
                    }).JsonStringify();
                    Logger.Info(request);
                    var json = client.UploadString(AppSettings.Default.TaiwanTaxi.CommonSettings, request);
                    Logger.Info(json);
                    var result = JsonConvert.DeserializeObject<CommonSettingsResponse>(json);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return null;
        }


        public class GISGeocodingResponse
        {
            public ResultObj Result { get; set; }
            public GISGeocodeObj[] Data { get; set; }
        }

        public class DispatchResponse
        {
            public ResultObj Result { get; set; }
            public DateTime DispatchTime { get; set; } = DateTime.Now;
        }


        public class DispatchOrderResponse : DispatchResponse
        {
            public OrderResultObj[] Jobs { get; set; }
            public OrderExtInfoObj ExtInfo { get; set; }
        }

        public class DispatchQueryResponse : DispatchResponse
        {
            public VehicleObj[] Vehicles { get; set; }
        }

        public class CommonSettingsResponse : DispatchResponse
        {
            public JToken Param { get; set; }
        }



        public class GISGeocodeObj
        {
            public String AddrId { get; set; }
            public String City { get; set; }
            public decimal Lat { get; set; }
            public decimal Lng { get; set; }
            public String Address { get; set; }
            public String Memo { get; set; }
            public int Provider { get; set; }
            public String ProviderKey { get; set; }
            public decimal Accuracy { get; set; }
        }

        public class OrderObj
        {
            public String CustPhone { get; set; }
            public String CustName { get; set; }
            public String CustTitle { get; set; }
            public String SvcType { get; set; }
            public int Amount { get; set; }
            public DateTime? BookTime { get; set; }
            public String Memo { get; set; }
            public int PaidType { get; set; }
            public SpecOrdObj SpecOrder { get; set; }
            public AddressObj Address { get; set; }
            public AddressObj TargetAddr { get; set; }

        }

        public class SpecOrdObj
        {
            public bool Trunk { get; set; }
            public bool Pet { get; set; }
            public bool WheelChair { get; set; }
            public bool VIP { get; set; }
            public bool DeliverGoods { get; set; }
            public bool MoveHouse { get; set; }
            public bool DriveAgent { get; set; }
            public bool WomenDriver { get; set; }
            public bool ElectricalConn { get; set; }
            public bool Coupon { get; set; }
            public bool AirportDropOff { get; set; }
            public bool ResponseNoCar { get; set; }
        }

        public class AddressObj
        {
            public String City { get; set; }
            public String Distrcit { get; set; }
            public String Road { get; set; }
            public String Section { get; set; }
            public String Lane { get; set; }
            public String Alley { get; set; }
            public String No { get; set; }
            public bool? IsFuzzy { get; set; }
            public decimal? Lat { get; set; }
            public decimal? Lng { get; set; }
            public String FuzzyAddr { get; set; }
        }

        public class OrderResultObj
        {
            public string OrderId { get; set; }
            public string JobId { get; set; }
        }

        public class OrderExtInfoObj
        {
            public int CarType { get; set; }
            public decimal Rate { get; set; }
            public int Fee { get; set; }
            public int Discount { get; set; }
            public String SoSweetRemind { get; set; }
            public decimal PayAmt { get; set; }
            public bool IsCreditPaySuccess { get; set; }
            public int FailCode { get; set; }
        }

        public class DriverInfoObj
        {
            public String CarNo { get; set; }
            public String Name { get; set; }
            public String HomePhone { get; set; }
            public String CellPhone1 { get; set; }
            public String CellPhone2 { get; set; }
            public String CarLicNum { get; set; }
            public bool DeviceCredit { get; set; }
            public bool DeviceEasyCard { get; set; }
            public int PortraitType { get; set; }
            public String PortraitData { get; set; }
            public DexInfoObj ExtensionInfo { get; set; }
            public String CarColor { get; set; }
            public int CompType { get; set; }

        }

        public class DexInfoObj
        {
            public String Series { get; set; }
            public String Brand { get; set; }
            public String ManufactureYM { get; set; }
            public decimal Rate { get; set; }
            public int BusiRegCertType { get; set; }
            public String BusiRegCertData { get; set; }

        }

        public class AirportInfoObj
        {
            public int AirportName { get; set; }
            public int People { get; set; }
            public int Baggage { get; set; }
            public String FlightNo { get; set; }
            public DateTime? FlightTime { get; set; }
            public Boolean BigCar { get; set; }
            public String OneMsg { get; set; }
            public int FareA { get; set; }
            public int FareB { get; set; }
            public int SP { get; set; }

        }

        public class DetailInfoObj
        {
            public int Type { get; set; }
            public string Title { get; set; }
            public int Value { get; set; }
            //public object RefInfo { get; set; }
            //public object TicketVendor { get; set; }
        }

        public class PaymentStatusObj
        {
            public int PayType { get; set; }
            public int Status { get; set; }
            public decimal? Amount { get; set; }
            public String AASCode { get; set; }
            public String AASTranNo { get; set; }
            public DetailInfoObj[] Detail { get; set; }
        }

        public class DynamicExtObj
        {
        }

        public class VehicleObj
        {
            public int SrvType { get; set; }
            public string OrderId { get; set; }
            public string JobId { get; set; }
            public int CustPayType { get; set; }
            public long BookTimeUTCT { get; set; }
            public String JobSvc { get; set; }
            public int OrderSvc { get; set; }
            public int? ConfirmRuleId { get; set; }
            public DateTime BookTime { get; set; }
            public string SvcType { get; set; }
            public TransactionStatus? TraState { get; set; }
            public String BookState { get; set; }
            public String CarNo { get; set; }
            public DriverInfoObj DriverInfo { get; set; }
            public int? ETA { get; set; }
            public DateTime? PushCust { get; set; }
            public bool? CustGetIn { get; set; }
            public int? FleetId { get; set; }
            public string UnionNa { get; set; }
            public bool? IsUnion { get; set; }
            public decimal? Lat { get; set; }
            public decimal? Lng { get; set; }
            public string Memo { get; set; }
            public AddressObj Address { get; set; }
            public AddressObj OffAddr { get; set; }
            public GISGeocodeObj GisFrom { get; set; }
            public GISGeocodeObj GisTo { get; set; }
            public DateTime? OnTime { get; set; }
            public DateTime? OffTime { get; set; }
            public decimal? MeterOff_Lat { get; set; }
            public decimal? MeterOff_Lng { get; set; }
            public OrderExtInfoObj OrdExInfo { get; set; }
            public AirportInfoObj AirportInfo { get; set; }
            //public object AgentInfo { get; set; }
            public PaymentStatusObj PaymentStatus { get; set; }
            //public object CouponToken { get; set; }
            public int? NormalFee { get; set; }
            public int DropIn { get; set; }
            //public object HappyGoInfo { get; set; }
            //public object UUPONInfo { get; set; }
            //public object CtbcBpInfo { get; set; }
            //public object CtbcBankInfo { get; set; }
            public DynamicExtObj DynamicExt { get; set; }
            public long? ETATimeUTCT { get; set; }
            //public object SrvTypeExt { get; set; }
            public object ExtInfo { get; set; }
            public SpecOrdObj SpecOrder { get; set; }
            public String JobQuotation { get; set; }
            public String PreQuotaJob { get; set; }
            /***
             * 1=搜車中 
             * 2=查無車輛 
             * 3=等車中 
             * 4=任務進行中 
             * 5=任務完成 
             * 6=任務已取消 
             * 7=司機取消任務 
             * 8=司機回報乘客未現 
             */
            public enum TransactionStatus 
            {
                搜車中 = 1,
                查無車輛,
                等車中,
                任務進行中,
                任務完成,
                任務已取消,
                司機取消任務,
                司機回報乘客未現,
            };
        }
    }
}