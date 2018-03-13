<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Linq.Expressions" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>

<div class="navbar-default sidebar" role="navigation">
    <div class="sidebar-nav navbar-collapse">
        <ul class="nav" id="side-menu">
            <li>
                <a href="<%= Url.Action("ResidentIndex","InfoCenter") %>"><i class="fa fa-hand-o-right" aria-hidden="true"></i>住戶訊息查詢</a>
            </li>
            <li>
                <a href="<%= Url.Action("ResidentEnergyIndex","InfoCenter") %>"><i class="fa fa-hand-o-right" aria-hidden="true"></i>瓦斯度維護</a>
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
