﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.42000.
// 
#pragma warning disable 1591

namespace WebHome.BA_Service {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="sr_BA_DeviceWebServiceSoap", Namespace="http://tempuri.org/")]
    public partial class sr_BA_DeviceWebService : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback Set_StatusOperationCompleted;
        
        private System.Threading.SendOrPostCallback Set_EventOperationCompleted;
        
        private System.Threading.SendOrPostCallback Set_Event_ByDeviceDetailOperationCompleted;
        
        private System.Threading.SendOrPostCallback Release_EventOperationCompleted;
        
        private System.Threading.SendOrPostCallback Sync_Device_URIOperationCompleted;
        
        private System.Threading.SendOrPostCallback Sync_CompleteOperationCompleted;
        
        private System.Threading.SendOrPostCallback New_Rule_TriggerOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public sr_BA_DeviceWebService() {
            this.Url = global::WebHome.Properties.Settings.Default.MessageCenter_BA_Service_sr_BA_DeviceWebService;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event Set_StatusCompletedEventHandler Set_StatusCompleted;
        
        /// <remarks/>
        public event Set_EventCompletedEventHandler Set_EventCompleted;
        
        /// <remarks/>
        public event Set_Event_ByDeviceDetailCompletedEventHandler Set_Event_ByDeviceDetailCompleted;
        
        /// <remarks/>
        public event Release_EventCompletedEventHandler Release_EventCompleted;
        
        /// <remarks/>
        public event Sync_Device_URICompletedEventHandler Sync_Device_URICompleted;
        
        /// <remarks/>
        public event Sync_CompleteCompletedEventHandler Sync_CompleteCompleted;
        
        /// <remarks/>
        public event New_Rule_TriggerCompletedEventHandler New_Rule_TriggerCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/Set_Status", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string Set_Status(string prm_auth_code, string l1_device_type, string device_uri, string device_status, string device_status_value, System.DateTime status_date) {
            object[] results = this.Invoke("Set_Status", new object[] {
                        prm_auth_code,
                        l1_device_type,
                        device_uri,
                        device_status,
                        device_status_value,
                        status_date});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void Set_StatusAsync(string prm_auth_code, string l1_device_type, string device_uri, string device_status, string device_status_value, System.DateTime status_date) {
            this.Set_StatusAsync(prm_auth_code, l1_device_type, device_uri, device_status, device_status_value, status_date, null);
        }
        
        /// <remarks/>
        public void Set_StatusAsync(string prm_auth_code, string l1_device_type, string device_uri, string device_status, string device_status_value, System.DateTime status_date, object userState) {
            if ((this.Set_StatusOperationCompleted == null)) {
                this.Set_StatusOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSet_StatusOperationCompleted);
            }
            this.InvokeAsync("Set_Status", new object[] {
                        prm_auth_code,
                        l1_device_type,
                        device_uri,
                        device_status,
                        device_status_value,
                        status_date}, this.Set_StatusOperationCompleted, userState);
        }
        
        private void OnSet_StatusOperationCompleted(object arg) {
            if ((this.Set_StatusCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.Set_StatusCompleted(this, new Set_StatusCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/Set_Event", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string Set_Event(string prm_auth_code, string l1_device_type, string device_uri, bool need_release, string event_type, string event_sub_type, int event_level, int event_timeout, string event_desc, string event_memo, bool isLog, System.DateTime device_time) {
            object[] results = this.Invoke("Set_Event", new object[] {
                        prm_auth_code,
                        l1_device_type,
                        device_uri,
                        need_release,
                        event_type,
                        event_sub_type,
                        event_level,
                        event_timeout,
                        event_desc,
                        event_memo,
                        isLog,
                        device_time});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void Set_EventAsync(string prm_auth_code, string l1_device_type, string device_uri, bool need_release, string event_type, string event_sub_type, int event_level, int event_timeout, string event_desc, string event_memo, bool isLog, System.DateTime device_time) {
            this.Set_EventAsync(prm_auth_code, l1_device_type, device_uri, need_release, event_type, event_sub_type, event_level, event_timeout, event_desc, event_memo, isLog, device_time, null);
        }
        
        /// <remarks/>
        public void Set_EventAsync(string prm_auth_code, string l1_device_type, string device_uri, bool need_release, string event_type, string event_sub_type, int event_level, int event_timeout, string event_desc, string event_memo, bool isLog, System.DateTime device_time, object userState) {
            if ((this.Set_EventOperationCompleted == null)) {
                this.Set_EventOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSet_EventOperationCompleted);
            }
            this.InvokeAsync("Set_Event", new object[] {
                        prm_auth_code,
                        l1_device_type,
                        device_uri,
                        need_release,
                        event_type,
                        event_sub_type,
                        event_level,
                        event_timeout,
                        event_desc,
                        event_memo,
                        isLog,
                        device_time}, this.Set_EventOperationCompleted, userState);
        }
        
        private void OnSet_EventOperationCompleted(object arg) {
            if ((this.Set_EventCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.Set_EventCompleted(this, new Set_EventCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/Set_Event_ByDeviceDetail", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string Set_Event_ByDeviceDetail(string prm_auth_code, string l1_device_type, string device_uri, bool need_release, string event_type, string event_sub_type, int event_level, int event_timeout, string device_status, string card_no, string event_detail, bool isLog, System.DateTime device_time) {
            object[] results = this.Invoke("Set_Event_ByDeviceDetail", new object[] {
                        prm_auth_code,
                        l1_device_type,
                        device_uri,
                        need_release,
                        event_type,
                        event_sub_type,
                        event_level,
                        event_timeout,
                        device_status,
                        card_no,
                        event_detail,
                        isLog,
                        device_time});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void Set_Event_ByDeviceDetailAsync(string prm_auth_code, string l1_device_type, string device_uri, bool need_release, string event_type, string event_sub_type, int event_level, int event_timeout, string device_status, string card_no, string event_detail, bool isLog, System.DateTime device_time) {
            this.Set_Event_ByDeviceDetailAsync(prm_auth_code, l1_device_type, device_uri, need_release, event_type, event_sub_type, event_level, event_timeout, device_status, card_no, event_detail, isLog, device_time, null);
        }
        
        /// <remarks/>
        public void Set_Event_ByDeviceDetailAsync(string prm_auth_code, string l1_device_type, string device_uri, bool need_release, string event_type, string event_sub_type, int event_level, int event_timeout, string device_status, string card_no, string event_detail, bool isLog, System.DateTime device_time, object userState) {
            if ((this.Set_Event_ByDeviceDetailOperationCompleted == null)) {
                this.Set_Event_ByDeviceDetailOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSet_Event_ByDeviceDetailOperationCompleted);
            }
            this.InvokeAsync("Set_Event_ByDeviceDetail", new object[] {
                        prm_auth_code,
                        l1_device_type,
                        device_uri,
                        need_release,
                        event_type,
                        event_sub_type,
                        event_level,
                        event_timeout,
                        device_status,
                        card_no,
                        event_detail,
                        isLog,
                        device_time}, this.Set_Event_ByDeviceDetailOperationCompleted, userState);
        }
        
        private void OnSet_Event_ByDeviceDetailOperationCompleted(object arg) {
            if ((this.Set_Event_ByDeviceDetailCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.Set_Event_ByDeviceDetailCompleted(this, new Set_Event_ByDeviceDetailCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/Release_Event", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string Release_Event(string prm_auth_code, string l1_device_type, string event_id) {
            object[] results = this.Invoke("Release_Event", new object[] {
                        prm_auth_code,
                        l1_device_type,
                        event_id});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void Release_EventAsync(string prm_auth_code, string l1_device_type, string event_id) {
            this.Release_EventAsync(prm_auth_code, l1_device_type, event_id, null);
        }
        
        /// <remarks/>
        public void Release_EventAsync(string prm_auth_code, string l1_device_type, string event_id, object userState) {
            if ((this.Release_EventOperationCompleted == null)) {
                this.Release_EventOperationCompleted = new System.Threading.SendOrPostCallback(this.OnRelease_EventOperationCompleted);
            }
            this.InvokeAsync("Release_Event", new object[] {
                        prm_auth_code,
                        l1_device_type,
                        event_id}, this.Release_EventOperationCompleted, userState);
        }
        
        private void OnRelease_EventOperationCompleted(object arg) {
            if ((this.Release_EventCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.Release_EventCompleted(this, new Release_EventCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/Sync_Device_URI", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string Sync_Device_URI(string prm_auth_code, string l1_device_type, string device_uri, string errorMsg) {
            object[] results = this.Invoke("Sync_Device_URI", new object[] {
                        prm_auth_code,
                        l1_device_type,
                        device_uri,
                        errorMsg});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void Sync_Device_URIAsync(string prm_auth_code, string l1_device_type, string device_uri, string errorMsg) {
            this.Sync_Device_URIAsync(prm_auth_code, l1_device_type, device_uri, errorMsg, null);
        }
        
        /// <remarks/>
        public void Sync_Device_URIAsync(string prm_auth_code, string l1_device_type, string device_uri, string errorMsg, object userState) {
            if ((this.Sync_Device_URIOperationCompleted == null)) {
                this.Sync_Device_URIOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSync_Device_URIOperationCompleted);
            }
            this.InvokeAsync("Sync_Device_URI", new object[] {
                        prm_auth_code,
                        l1_device_type,
                        device_uri,
                        errorMsg}, this.Sync_Device_URIOperationCompleted, userState);
        }
        
        private void OnSync_Device_URIOperationCompleted(object arg) {
            if ((this.Sync_Device_URICompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.Sync_Device_URICompleted(this, new Sync_Device_URICompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/Sync_Complete", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string Sync_Complete(string prm_auth_code, string l1_device_type, string errorMsg) {
            object[] results = this.Invoke("Sync_Complete", new object[] {
                        prm_auth_code,
                        l1_device_type,
                        errorMsg});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void Sync_CompleteAsync(string prm_auth_code, string l1_device_type, string errorMsg) {
            this.Sync_CompleteAsync(prm_auth_code, l1_device_type, errorMsg, null);
        }
        
        /// <remarks/>
        public void Sync_CompleteAsync(string prm_auth_code, string l1_device_type, string errorMsg, object userState) {
            if ((this.Sync_CompleteOperationCompleted == null)) {
                this.Sync_CompleteOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSync_CompleteOperationCompleted);
            }
            this.InvokeAsync("Sync_Complete", new object[] {
                        prm_auth_code,
                        l1_device_type,
                        errorMsg}, this.Sync_CompleteOperationCompleted, userState);
        }
        
        private void OnSync_CompleteOperationCompleted(object arg) {
            if ((this.Sync_CompleteCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.Sync_CompleteCompleted(this, new Sync_CompleteCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/New_Rule_Trigger", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string New_Rule_Trigger(string prm_auth_code, string l1_device_type, int rule_id, string event_id, int rule_s_nbr) {
            object[] results = this.Invoke("New_Rule_Trigger", new object[] {
                        prm_auth_code,
                        l1_device_type,
                        rule_id,
                        event_id,
                        rule_s_nbr});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void New_Rule_TriggerAsync(string prm_auth_code, string l1_device_type, int rule_id, string event_id, int rule_s_nbr) {
            this.New_Rule_TriggerAsync(prm_auth_code, l1_device_type, rule_id, event_id, rule_s_nbr, null);
        }
        
        /// <remarks/>
        public void New_Rule_TriggerAsync(string prm_auth_code, string l1_device_type, int rule_id, string event_id, int rule_s_nbr, object userState) {
            if ((this.New_Rule_TriggerOperationCompleted == null)) {
                this.New_Rule_TriggerOperationCompleted = new System.Threading.SendOrPostCallback(this.OnNew_Rule_TriggerOperationCompleted);
            }
            this.InvokeAsync("New_Rule_Trigger", new object[] {
                        prm_auth_code,
                        l1_device_type,
                        rule_id,
                        event_id,
                        rule_s_nbr}, this.New_Rule_TriggerOperationCompleted, userState);
        }
        
        private void OnNew_Rule_TriggerOperationCompleted(object arg) {
            if ((this.New_Rule_TriggerCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.New_Rule_TriggerCompleted(this, new New_Rule_TriggerCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    public delegate void Set_StatusCompletedEventHandler(object sender, Set_StatusCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class Set_StatusCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal Set_StatusCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    public delegate void Set_EventCompletedEventHandler(object sender, Set_EventCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class Set_EventCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal Set_EventCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    public delegate void Set_Event_ByDeviceDetailCompletedEventHandler(object sender, Set_Event_ByDeviceDetailCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class Set_Event_ByDeviceDetailCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal Set_Event_ByDeviceDetailCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    public delegate void Release_EventCompletedEventHandler(object sender, Release_EventCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class Release_EventCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal Release_EventCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    public delegate void Sync_Device_URICompletedEventHandler(object sender, Sync_Device_URICompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class Sync_Device_URICompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal Sync_Device_URICompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    public delegate void Sync_CompleteCompletedEventHandler(object sender, Sync_CompleteCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class Sync_CompleteCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal Sync_CompleteCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    public delegate void New_Rule_TriggerCompletedEventHandler(object sender, New_Rule_TriggerCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class New_Rule_TriggerCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal New_Rule_TriggerCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591