﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebHome.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.8.1.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("AWTK")]
        public string CenterID {
            get {
                return ((string)(this["CenterID"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("EXAWTK")]
        public string PRMType {
            get {
                return ((string)(this["PRMType"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("756001")]
        public string PRMUsed {
            get {
                return ((string)(this["PRMUsed"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("30")]
        public int ValidTokenDurationInMinutes {
            get {
                return ((int)(this["ValidTokenDurationInMinutes"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.ConnectionString)]
        [global::System.Configuration.DefaultSettingValueAttribute("Server=localhost;Port=4406;Database=dnake;Uid=root;Pwd=;charset=utf8;")]
        public string dnake {
            get {
                return ((string)(this["dnake"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("2")]
        public int DeviceHeartBeatPeriodInMinutes {
            get {
                return ((int)(this["DeviceHeartBeatPeriodInMinutes"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10000")]
        public int JobSchedulerInMilliseconds {
            get {
                return ((int)(this["JobSchedulerInMilliseconds"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string LeaderID {
            get {
                return ((string)(this["LeaderID"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10")]
        public int CheckEventIntervalInSeconds {
            get {
                return ((int)(this["CheckEventIntervalInSeconds"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://nsst.tw:8080/c148/monitor/intercom.php")]
        public string TouchLifeOutbound {
            get {
                return ((string)(this["TouchLifeOutbound"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("360")]
        public int PushMessageMaxLength {
            get {
                return ((int)(this["PushMessageMaxLength"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://fcm.googleapis.com/fcm/send")]
        public string GoogleFCMUrl {
            get {
                return ((string)(this["GoogleFCMUrl"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("key=AIzaSyDrDyQRVavEWLBRYnsJmwy3mSksPqm9_qo")]
        public string GoogleFCMAuthorization {
            get {
                return ((string)(this["GoogleFCMAuthorization"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("[\"迴路一\",\r\n\"迴路二\",\r\n\"迴路三\",\r\n\"迴路四\",\r\n\"迴路五\",\r\n\"迴路六\",\r\n\"迴路七\",\r\n\"迴路八\"]")]
        public string JS_PublicAlarm {
            get {
                return ((string)(this["JS_PublicAlarm"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("bf3656056b404e99c0c33784bd294ee8")]
        public string ChannelSecret {
            get {
                return ((string)(this["ChannelSecret"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("yokZqO4GpXsjzpYKfvTNx+iyZeoJ5z+706piWVS96nHnme2x0uUISL7+9yq+BFGC779LtWD9F3TcDECa+" +
            "P17GilKOL1fBOdUYtHMFlgDhv9F90EUYcrmgaOdCWaO4aXehPHDRLgs7V9Q3dYCbw07kAdB04t89/1O/" +
            "w1cDnyilFU=")]
        public string ChannelToken {
            get {
                return ((string)(this["ChannelToken"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://api.line.me/v2/bot/message/push")]
        public string LinePushMessage {
            get {
                return ((string)(this["LinePushMessage"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://api.line.me/v2/bot/message/multicast")]
        public string LinePushMulticast {
            get {
                return ((string)(this["LinePushMulticast"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://www.worthitstudio.idv.tw")]
        public string HostUrl {
            get {
                return ((string)(this["HostUrl"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("20160")]
        public int UserTimeoutInMinutes {
            get {
                return ((int)(this["UserTimeoutInMinutes"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://1.34.110.45/service/SecomBAService.svc/authorization/query")]
        public string GetAuthToken {
            get {
                return ((string)(this["GetAuthToken"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://1.34.110.45/service/SecomBAService.svc/building/query")]
        public string GetBuildingInfo {
            get {
                return ((string)(this["GetBuildingInfo"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://1.34.110.45/service/SecomBAService.svc/device/insert_1")]
        public string InsertDevice {
            get {
                return ((string)(this["InsertDevice"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://1.34.110.45/service/SecomBAService.svc/device/update_1")]
        public string UpdateDevice {
            get {
                return ((string)(this["UpdateDevice"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://1.34.110.45/service/SecomBAService.svc/device/delete")]
        public string DeleteDevice {
            get {
                return ((string)(this["DeleteDevice"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://1.34.110.45/service/SecomBAService.svc/device/query_1")]
        public string QueryAllDevices {
            get {
                return ((string)(this["QueryAllDevices"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://1.34.110.45/service/SecomBAService.svc/resident/query")]
        public string GetResidentInfo {
            get {
                return ((string)(this["GetResidentInfo"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://1.34.110.45/service/SecomBAService.svc/message_board/query")]
        public string GetResidentMessage {
            get {
                return ((string)(this["GetResidentMessage"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://1.34.110.45/service/SecomBAService.svc/energy/degree/query")]
        public string EnergyDegreeQuery {
            get {
                return ((string)(this["EnergyDegreeQuery"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://1.34.110.45/service/SecomBAService.svc/energy/degree/insert")]
        public string EnergyDegreeInsert {
            get {
                return ((string)(this["EnergyDegreeInsert"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://1.34.110.45/service/SecomBAService.svc/energy/degree/delete")]
        public string EnergyDegreeDelete {
            get {
                return ((string)(this["EnergyDegreeDelete"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://1.34.110.45/service/SecomBAService.svc/energy/degree/update")]
        public string EnergyDegreeUpdate {
            get {
                return ((string)(this["EnergyDegreeUpdate"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.WebServiceUrl)]
        [global::System.Configuration.DefaultSettingValueAttribute("http://1.34.110.45/service/sr_BA_DeviceWebService.asmx")]
        public string MessageCenter_BA_Service_sr_BA_DeviceWebService {
            get {
                return ((string)(this["MessageCenter_BA_Service_sr_BA_DeviceWebService"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("4")]
        public int CommunicationMode {
            get {
                return ((int)(this["CommunicationMode"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("5")]
        public int AlertBeforeMinutes {
            get {
                return ((int)(this["AlertBeforeMinutes"]));
            }
        }
    }
}