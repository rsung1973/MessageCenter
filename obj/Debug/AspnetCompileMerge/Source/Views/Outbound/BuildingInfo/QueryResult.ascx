<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Linq.Expressions" %>
<%@ Import Namespace="System.Web.Mvc" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="Utility" %>
<%@ Import Namespace="Newtonsoft.Json" %>
<%@ Import Namespace="Newtonsoft.Json.Linq" %>

<div class="border_gray" style="overflow-x: auto; max-width: 1024px;">
    <%  var recordCount = _model.Count();
        if (recordCount > 0)
        { %>
    <%  Html.RenderPartial("~/Views/Outbound/BuildingInfo/ItemList.ascx", _model); %>
    <%  }
        else
        { %>
    <font color="red">查無資料!!</font>
    <%  } %>
</div>

<script runat="server">

    JArray _model;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        _model = (JArray)this.Model;
    }

</script>

