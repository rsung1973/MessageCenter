<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Linq.Expressions" %>
<%@ Import Namespace="System.Web.Mvc" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="Utility" %>
<%@ Import Namespace="Newtonsoft.Json" %>
<%@ Import Namespace="Newtonsoft.Json.Linq" %>

<tr>
    <%  foreach (JProperty item in _model)
        {   %>
    <td><%= item.Value %></td>
    <%  }   %>
</tr>

<script runat="server">

    JToken _model;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        _model = (JToken)this.Model;
    }

</script>
