<%@ Page Title="" Language="C#" MasterPageFile="~/Template/MvcBlankPage.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>

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


<asp:Content ID="header" ContentPlaceHolderID="headContent" runat="server">
</asp:Content>
<asp:Content ID="mainContent" ContentPlaceHolderID="mainContent" runat="server">
    <table style="background-image: url('../../images/main_bkg.png');background-repeat:round; position: absolute; right: 0px; top: 0px; bottom: 0px; left: 0px; width: 100%; height: 100%;text-align:center;">
        <tr>
            <td style="vertical-align:bottom;"><a href="http://wa.taipei.youbike.com.tw" style="float:right;"><div style="text-align:center;font-size:x-large;color:white;"><img src="../../images/main_btn_logger.png" /><br />U-Bike</div></a></td>
            <td><a href="http://1.34.110.45/pad/ws_quick_login.aspx?p=eyJpZCI6ImF3dGVrIiwicHN3ZCI6ImF3dGVrIiwiZGF0ZSI6IjIwNDgvMDYvMjIgMDA6MDA6MDAifQ=="><div style="text-align:center;font-size:x-large;color:white;"><img src="../../images/main_btn_apps.png" /><br />物業管理</div></a></td>
            <td style="vertical-align:bottom;"><a href="http://www.taipeibus.gov.taipei/" style="float:left;"><div style="text-align:center;font-size:x-large;color:white;"><img src="../../images/main_btn_cook.png" /><br />公車</div></a></td>
        </tr>
        <tr>
            <td style="vertical-align:top;"><a href="<%= Url.Action("SurroundingIndex") %>" style="float:right;"><div style="text-align:center;font-size:x-large;color:white;"><img src="../../images/main_btn_mall.png" /><br />週邊環境</div></a></td>
            <td><a><div style="text-align:center;font-size:x-large;color:white;"><img src="../../images/main_btn_stock.png" /><br />其它</div></a></td>
            <td style="vertical-align:top;"><a style="float:left;"><div style="text-align:center;font-size:x-large;color:white;"><img src="../../images/main_btn_map.png" /><br />能源管理</div></a></td>
        </tr>
    </table>
</asp:Content>
<script runat="server">

    String _inquiryView;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
    }

</script>