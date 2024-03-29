﻿<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>

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
<%  Html.RenderPartial("~/Views/SiteAction/FunctionTitleBar.cshtml", "住戶訊息查詢"); %>
<div class="border_gray">
    <!--表格 開始-->
    <table width="100%" border="0" cellpadding="0" cellspacing="0" class="left_title">
        <tr>
            <th colspan="2" class="Head_style_a">查詢條件
            </th>
        </tr>
        <tr>
            <th>住戶識別碼(resident ID)
            </th>
            <td class="tdleft">
                <input type="text" name="ResidentID" value="<%= _viewModel.ResidentID.GetEfficientString() %>"></input>
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
                <button type="button" onclick="uiDeviceQuery.inquireResidentMessage();">查詢</button>
            </td>
        </tr>
    </tbody>
</table>
<!--表格 開始-->
<%  Html.RenderPartial("~/Views/DeviceEvents/ScriptHelper/Common.cshtml"); %>
<script>
    var uiDeviceQuery = {};
    uiDeviceQuery.viewModel = <%= JsonConvert.SerializeObject(_viewModel) %>;
</script>
<script runat="server">

    ModelSource<LiveDevice> models;
    InfoQueryViewModel _viewModel;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        models = ((SampleController<LiveDevice>)ViewContext.Controller).DataSource;
        _viewModel = (InfoQueryViewModel)ViewBag.ViewModel;
    }
</script>

