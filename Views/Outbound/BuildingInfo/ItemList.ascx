<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Linq.Expressions" %>
<%@ Import Namespace="System.Web.Mvc" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="Utility" %>
<%@ Import Namespace="Newtonsoft.Json" %>
<%@ Import Namespace="Newtonsoft.Json.Linq" %>

<table class="table01 itemList">
    <thead>
        <tr>
            <%  foreach (JProperty p in _model.First.Children())
                { %>
                    <th <%--style="min-width: 80px;"--%>><%= p.Name %></th>
            <%  } %>
        </tr>
    </thead>
    <tbody>
        <%  int idx = 0;
            foreach (var item in _model)
            {
                idx++;
                Html.RenderPartial("~/Views/Outbound/BuildingInfo/DataItem.ascx", item);
            }
        %>
    </tbody>
    <tfoot>
        <tr>
            <td colspan="<%= _model.First.Count() %>">&nbsp;
            </td>
        </tr>
    </tfoot>
</table>

<script runat="server">

    JArray _model;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        _model = (JArray)this.Model;
    }

</script>
