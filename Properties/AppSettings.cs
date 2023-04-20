using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace WebHome.Properties
{
    public partial class AppSettings : IDisposable
    {
        public static String AppRoot
        {
            get;
            private set;
        } = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);

        static JObject _Settings;

        static AppSettings()
        {
            _default = Initialize<AppSettings>(typeof(AppSettings).Namespace);
        }

        public AppSettings()
        {

        }

        public static void Reload()
        {
            _default = Initialize<AppSettings>(typeof(AppSettings).Namespace);
        }

        public void Save()
        {
            String fileName = "App.settings.json";
            String filePath = Path.Combine(AppRoot, fileName);
            String propertyName = typeof(AppSettings).Namespace;
            _Settings[propertyName] = JObject.FromObject(this);
            File.WriteAllText(filePath, _Settings.ToString());
        }

        protected static T Initialize<T>(String propertyName)
            where T : AppSettings, new()
        {
            T currentSettings;
            //String fileName = $"{Assembly.GetExecutingAssembly().GetName()}.settings.json";
            String fileName = "App.settings.json";
            String filePath = Path.Combine(AppRoot, fileName);
            if (File.Exists(filePath))
            {
                _Settings = (JObject)JsonConvert.DeserializeObject(File.ReadAllText(filePath));
            }
            else
            {
                _Settings = new JObject();
            }

            //String propertyName = Assembly.GetExecutingAssembly().GetName().Name;
            if (_Settings[propertyName] != null)
            {
                currentSettings = _Settings[propertyName].ToObject<T>();
            }
            else
            {
                currentSettings = new T();
                _Settings[propertyName] = JObject.FromObject(currentSettings);
            }

            File.WriteAllText(filePath, _Settings.ToString());
            return currentSettings;
        }

        public void Dispose()
        {
            dispose(true);
        }

        bool _disposed;
        protected void dispose(bool disposing)
        {
            if (!_disposed)
            {
                this.Save();
            }
            _disposed = true;
        }

        ~AppSettings()
        {
            dispose(false);
        }

        static AppSettings _default;

        public static AppSettings Default => _default;

        int? _buildingID;
        public int? BuildingID
        {
            get => _buildingID;
            set
            {
                _buildingID = value;
            }
        }

        int? _floorID;
        public int? FloorID
        {
            get => _floorID;
            set
            {
                _floorID = value;
            }
        }

        public String LineMessageCenter
        {
            get;
            set;
        } = "https://s1.awtek-security.com.tw";

        public bool PushToLineMessageCenter
        {
            get;
            set;
        }

        public bool RegisterResidentToSECOM
        {
            get;
            set;
        }

        public bool UseCustomBA
        {
            get;
            set;
        }

        public String SetAccessCardUrl { get; set; } = "http://211.23.144.237:8824/api/datavalue/SetAWTEKCard";

        public String DefaultAccessCardMachineKey { get; set; } = "00001-1011111";
        public String DefaultAccessCardCustomerID { get; set; } = "guest01";

        public enum OutputContentType
        {
            Json = 1,
            FormValues = 2,
        }
        public OutputContentType? RequestContentType { get; set; } = OutputContentType.Json;

        public TaiwanTaxiSettings TaiwanTaxi { get; set; } = new TaiwanTaxiSettings { };
        public BuildingLocation Location { get; set; } = new BuildingLocation { };
        public YoxiSettings YoxiTaxi { get; set; } = new YoxiSettings { };
        public StorageBoxSettings[] StorageBoxArray { get; set; } =
                {
                    new StorageBoxSettings
                        {
                            BoxSize = StorageBoxSize.小,
                        },
                    new StorageBoxSettings
                        {
                            BoxSize = StorageBoxSize.中,
                        },
                    new StorageBoxSettings
                        {
                            BoxSize = StorageBoxSize.大,
                        },                    
                    new StorageBoxSettings
                        {
                            BoxSize = StorageBoxSize.餐盒,
                        },
                };
        public int? BuildingFloors { get; set; }
        public int? ElevatorCount { get; set; }
        public StorageBoxSettings[] ElevatorBoxArray { get; set; }
    }

    public partial class TaiwanTaxiAuthRequest
    {
        public int CompanyId { get; set; } = 53;
        public String ServiceToken { get; set; } = "r+MA4bH9Ht0pC9bvF2VJknd/q4A=";
        public String CompanySecret { get; set; } = "85hAg1hyDhNZxOFJZXuWhT/Ij8M=";
        public String Signature
        {
            get
            {
                String data = $"{CompanySecret}{ServiceToken}{UserId}";
                if (String.IsNullOrEmpty(data))
                {
                    return null;
                }
                else
                {
                    using (SHA1 hash = SHA1.Create())
                    {
                        return Convert.ToBase64String(hash.ComputeHash(Encoding.UTF8.GetBytes(data)));
                    }
                }
            }
        }
        public String UserId { get; set; } = "Awtek";

    }

    public partial class TaiwanTaxiSettings
    {
        public String DispatchAuth { get; set; } = "https://app-apitest.taiwantaxi.com.tw/DispatchAuth";
        public String GISGeocoding { get; set; } = "https://app-apitest.taiwantaxi.com.tw/DispatchOrder/GISGeocoding";
        public String DispatchOrder { get; set; } = "https://app-apitest.taiwantaxi.com.tw/DispatchOrder";
        public String DispatchQuery { get; set; } = "https://app-apitest.taiwantaxi.com.tw/DispatchQuery";
        public String DispatchCancel { get; set; } = "https://app-apitest.taiwantaxi.com.tw/DispatchCancel";
        public String CommonSettings { get; set; } = "https://app-apitest.taiwantaxi.com.tw/Settings/Common";
        public TaiwanTaxiAuthRequest DefaultAuthRequest { get; set; } = new TaiwanTaxiAuthRequest { };
        public String LoggingPath { get; set; } = Path.Combine(Logger.LogPath, "TaiwanTaxiOrder");
    }

    public partial class YoxiSettings
    {
        public String ConnectToProxy { get; set; } = "https://hotaiconnected-apim.azure-api.net/AWTEKProxy/connectToProxy";
        public String DispatchOrder { get; set; } = "https://hotaiconnected-apim.azure-api.net/AWTEKProxy/confirmtrip";
        public String UpdateSpotAccount { get; set; } = "https://hotaiconnected-apim.azure-api.net/AWTEKProxy/modifySpotAccount";
        public String DispatchQuery { get; set; } = "https://hotaiconnected-apim.azure-api.net/AWTEKProxy/tripInformation";
        public String DispatchCancel { get; set; } = "https://hotaiconnected-apim.azure-api.net/AWTEKProxy/canceltrip";
        public String LoggingPath { get; set; } = Path.Combine(Logger.LogPath, "YoxiOrder");
        public String DefaultSpotAccount { get; set; } = "00001";
    }

    public partial class StorageBoxSettings
    {
        public String StorageBoxUrl { get; set; } = "192.168.68.18";
        public String UserID { get; set; } = "admin";
        public String Password { get; set; } = "123456";
        public int PortCount { get; set; } = 16;
        public StorageBoxSize BoxSize { get; set; } = StorageBoxSize.小;
        public bool Enabled { get; set; }
        public int RelayTiming { get; set; } = 3000;
        public String No { get; set; }

    }

    public enum StorageBoxSize
    {
        小 = 1,
        中 = 2,
        大 = 3,
        餐盒 = 4,
    }


    public partial class BuildingLocation
    {
        public String City { get; set; }
        public String District { get; set; }
        public String Road { get; set; }
        public String Section { get; set; }
        public String Lane { get; set; }
        public String Alley { get; set; }
        public String No { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }
        public String Memo { get; set; }
        public int? FloorCount { get; set; }
        public int? UnitCount { get; set; }
    }
}