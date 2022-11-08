<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Linq.Expressions" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>

<div class="navbar-default sidebar" role="navigation">
    <div class="sidebar-nav navbar-collapse">
        <ul class="nav" id="side-menu">
            <li>
                <a href="<%= Url.Action("ApplyCenterBuildingInfo","Outbound") %>"><i class="fa fa-hand-o-right" aria-hidden="true"></i>檢視樓層資訊</a>
            </li>
            <li>
                <a href="<%= Url.Action("Index","DeviceEvents") %>"><i class="fa fa-hand-o-right" aria-hidden="true"></i>設備端狀態查詢</a>
            </li>
        </ul>
    </div>
    <!-- /.sidebar-collapse -->
</div>


<script runat="server">

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

    }


</script>
