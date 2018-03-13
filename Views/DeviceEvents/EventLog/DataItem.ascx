<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Linq.Expressions" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="WebHome.Controllers" %>
<%@ Import Namespace="WebHome.Helper" %>
<%@ Import Namespace="WebHome.Models" %>
<%@ Import Namespace="WebHome.Models.DataEntity" %>
<%@ Import Namespace="WebHome.Models.Locale" %>
<%@ Import Namespace="WebHome.Models.ViewModel" %>
<%@ Import Namespace="Utility" %>
<%@ Import Namespace="Newtonsoft.Json" %>

<tr>
    <td><%= _model.LiveID.HasValue ? _model.LiveDevice.UserRegister.DeviceUri : null %></td>
    <td><%= _model.LogDate.ToString() %></td>
    <td><%= _model.EventCode %></td>
    <td><%= _model.Tx %></td>
    <td><%= _model.Rx %></td>
    <td>
        <a href="javascript:uiDeviceQuery.sendEvent(<%= _model.LogID %>);">重送事件</a>
        <a href="javascript:uiDeviceQuery.sendStatus(<%= _model.LogID %>);">重送狀態</a>
    </td>
</tr>


<script runat="server">

    DeviceEventLog _model;
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        _model = (DeviceEventLog)this.Model;
    }

</script>
