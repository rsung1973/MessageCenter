﻿<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Linq.Expressions" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>

<%  if (ViewBag.ResidentOnly == true)
    { %>
<%  Html.RenderPartial("~/Views/SiteAction/ResidentMenu.ascx"); %>
<%  }
    else
    { %>
<%  Html.RenderPartial("~/Views/SiteAction/UserMenu.ascx"); %>
<%  } %>

<script runat="server">

    //Model.Security.MembershipManagement.UserProfileMember _userProfile;
    //String _menuView;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        //_userProfile = Business.Helper.WebPageUtility.UserProfile;
        //_menuView = "~/Views/SiteAction/" + Path.GetFileNameWithoutExtension(_userProfile.CurrentSiteMenu) + ".ascx";
    }


</script>
