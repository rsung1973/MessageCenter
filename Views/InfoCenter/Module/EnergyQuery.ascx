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
<!--交易畫面標題-->
<%  Html.RenderPartial("~/Views/SiteAction/FunctionTitleBar.ascx", "瓦斯度數維護"); %>
<div class="border_gray">
    <!--表格 開始-->
    <table width="100%" border="0" cellpadding="0" cellspacing="0" class="left_title">
        <tr>
            <th colspan="4" class="Head_style_a">瓦斯度數資料
            </th>
        </tr>
        <tr>
            <th>住戶識別碼(resident ID)
            </th>
            <td class="tdleft" colspan="3">
                <input type="text" name="ResidentID" value="<%= _viewModel.ResidentID.GetEfficientString() %>"></input>
            </td>
        </tr>
        <tr>
            <th>年份(西元年)
            </th>
            <td class="tdleft">
                <input type="text" name="Year" value="<%= _viewModel.Year %>"></input>
            </td>
            <th>月份
            </th>
            <td class="tdleft">
                <input type="text" name="Month" value="<%= _viewModel.Month %>"></input>
            </td>
        </tr>
        <tr>
            <th>瓦斯度數
            </th>
            <td class="tdleft">
                <input type="text" name="Degree" value="<%= _viewModel.Degree %>"></input>
            </td>
            <td colspan="2">
                <button type="button" onclick="uiDeviceQuery.reportEnergyDegree(0);">新增</button> 或
                <button type="button" onclick="uiDeviceQuery.reportEnergyDegree(1);">修改</button>
            </td>
        </tr>
<%--        <tr>
            <th>每頁資料筆數
            </th>
            <td class="tdleft">
                <input name="pageSize" type="text" value="<%= _viewModel.PageSize %>" />
            </td>
        </tr>   --%> 
    </table>
    <!--表格 結束-->
</div>
<!--按鈕-->
<table border="0" cellspacing="0" cellpadding="0" width="100%" class="queryAction">
    <tbody>
        <tr>
            <td class="Bargain_btn">
                <button type="button" onclick="uiDeviceQuery.inquireEnergyDegree();">查詢</button>
                <button type="button" onclick="uiDeviceQuery.reportEnergyDegree(2);;">刪除</button>
            </td>
        </tr>
    </tbody>
</table>
<!--表格 開始-->
<%  Html.RenderPartial("~/Views/DeviceEvents/ScriptHelper/Common.ascx"); %>
<script>
    var uiDeviceQuery = {};
    uiDeviceQuery.viewModel = <%= JsonConvert.SerializeObject(_viewModel) %>;
</script>
<script runat="server">

    ModelSource<LiveDevice> models;
    EnergyQueryViewModel _viewModel;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        models = ((SampleController<LiveDevice>)ViewContext.Controller).DataSource;
        _viewModel = (EnergyQueryViewModel)ViewBag.ViewModel;
    }
</script>

