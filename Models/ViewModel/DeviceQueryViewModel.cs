using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Utility;
using WebHome.DataPort;
using WebHome.Models.Locale;
using WebHome.Properties;

namespace WebHome.Models.ViewModel
{
    public class DeviceQueryViewModel  : InfoQueryViewModel
    {
        public String DeviceUri { get; set; }
        public int? LiveID { get; set; }
    }

    public class StorageBoxViewModel : InfoQueryViewModel
    {
        public string StorageBoxUrl { get; set; }
        public int BoxDeviceIndex { get; set; } = 0;
        public StorageBoxSize? BoxSize { get; set; }
        public int? Port { get; set; }
        public int? ActionType { get; set; }
    }

    public class ElevatorBoxViewModel : InfoQueryViewModel
    {
        public int? ElevatorCount { get; set; }
        public int? Floors { get; set; }
        public string[] StorageBoxUrl { get; set; }
        public string[] No { get; set; }
    }

    public class QueryViewModel
    {
        public int? id { get; set; }
        public String KeyID { get; set; }
        public String HKeyID { get; set; }
        public String DialogID { get; set; }
        public String FileDownloadToken { get; set; }
        public int? CurrentIndex { get; set; }
        public int? PagingSize { get; set; }
        public int? RecordCount { get; set; }

        public QueryViewModel Duplicate()
        {
            return (QueryViewModel)this.MemberwiseClone();
        }

        public String CustomQuery { get; set; }
        //public Naming.DataOperationMode? DataOperation { get; set; }
        public String ViewID { get; set; }
        public int PageSize { get; set; } = 50;
        public int? PageIndex { get; set; }

        public static T LoadData<T>(String fileName = null)
            where T : QueryViewModel, new()
        {
            String filePath = Path.Combine(CheckViewModelStorePath(), $"{fileName ?? typeof(T).Name}.json");
            if (File.Exists(filePath))
            {
                T result = JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath));
                return result;
            }
            return new T();
        }

        public void SaveData(String fileName = null)
        {
            String filePath = Path.Combine(CheckViewModelStorePath(), $"{fileName ?? this.GetType().Name}.json");
            File.WriteAllText(filePath, this.JsonStringify());
        }

        public static String CheckViewModelStorePath()
        {
            return Path.Combine(Logger.LogPath, "ViewModel").CheckStoredPath();
        }

    }

    public class InfoQueryViewModel : QueryViewModel
    {
        public string ResidentID { get; set; }

        public bool? CheckConnection { get; set; }

        public DeviceTransactionViewModel.UrgentEventDefinition? ShowEvent { get; set; }

        public string InstanceID { get; set; }
        public String LineID { get; set; }
        public Naming.DefenceStatus? DefenceStatus { get; set; }
        public string CustomerID { get; set; }
        public String BranchNo { get; set; }
        public int? Cards { get; set; }
        public String Caller { get => ResidentID; set => ResidentID = value; }
        public String Callee { get => BranchNo; set => BranchNo = value; }
        public String Message { get; set; }
        public String Title { get; set; }
        public int? Floor { get; set; }
        public String ElevatorNo { get; set; }
    }

    public class TaiwanTaxiAgentViewModel : DeviceQueryViewModel
    {
        public TaiwanTaxiAgent.DispatchAuthResponse AuthResponse { get; set; }
    }

    public class YoxiAgentViewModel : DeviceQueryViewModel
    {
        public String CustPhone { get; set; }
        public String CustName { get; set; }
        public String CustTitle { get; set; }
        public String SvcType { get; set; }
        public int Amount { get; set; }
        public DateTime? BookDate { get; set; }
        public int? Hour { get; set; }
        public int? Minute { get; set; }
        public String Memo { get; set; }
        public int PaidType { get; set; }
        public String City { get; set; }
        public String District { get; set; }
        public String Road { get; set; }
        public String Section { get; set; }
        public String Lane { get; set; }
        public String Alley { get; set; }
        public String No { get; set; }
        public String JobID { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }
        public int? UnitNo { get; set; }
        public String spotAccount { get; set; }
    }

    public class UserGuideQueryViewModel : QueryViewModel
    {
        public String UserGuide { get; set; }
    }

    public class EnergyQueryViewModel : InfoQueryViewModel
    {
        public int EnergyType { get; set; }
        public int? Degree { get; set; } = 0;
        public int? Year { get; set; } = DateTime.Today.Year;
        public int? Month { get; set; } = DateTime.Today.AddMonths(-1).Month;
        public bool? Update { get; set; }
    }

    public class DeviceCommandViewModel
    {
        public int? CommandID { get; set; }
        public string TokenID { get; set; }
    }

    public class UserAccountQueryViewModel : InfoQueryViewModel
    {
        public String PID { get; set; }
        public String UserName { get; set; }
        public string Password { get; set; }
        public string Password2 { get; set; }
        public int? UID { get; set; }
        public String RealName { get; set; }
        public String CardID { get; set; }
        public int? LogID { get; set; }
        public String QRCode { get; set; }
    }

    public class DefenceQueryViewModel : InfoQueryViewModel
    {
        public int? UID { get; set; }
        public DeviceTransactionViewModel.DefenceStatus? Defence { get; set; }
    }

    public class TaxiOrderViewModel : TaiwanTaxiAgentViewModel
    {
        public String CustPhone { get; set; }
        public String CustName { get; set; }
        public String CustTitle { get; set; }
        public String SvcType { get; set; }
        public int Amount { get; set; }
        public DateTime? BookDate { get; set; }
        public int? Hour { get; set; }
        public int? Minute { get; set; }
        public String Memo { get; set; }
        public int PaidType { get; set; }
        public String City { get; set; }
        public String District { get; set; }
        public String Road { get; set; }
        public String Section { get; set; }
        public String Lane { get; set; }
        public String Alley { get; set; }
        public String No { get; set; }
        public String JobID { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }
        public int? UnitNo { get; set; }
    }



    public class UserProfileViewModel
    {
        public int? UID { get; set; }
        public bool? EnableAlarm { get; set; }
    }

}