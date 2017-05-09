<%@ Page Title="" Language="C#" MasterPageFile="~/Template/MvcMainPage.Master" AutoEventWireup="true"  Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Linq.Expressions" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="Utility" %>

<asp:Content ID="headContent" ContentPlaceHolderID="headContent" runat="server">
</asp:Content>
<asp:Content ID="formContent" ContentPlaceHolderID="formContent" runat="server">
</asp:Content>
<asp:Content ID="mainContent" ContentPlaceHolderID="mainContent" runat="server">
    <%  Html.RenderPartial("~/Views/Outbound/BuildingInfo/QueryResult.ascx"); %>
</asp:Content>
<script runat="server">

    String _inquiryView;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        ViewBag.ActionName = "首頁 > 已登錄設備資訊";
    }

</script>