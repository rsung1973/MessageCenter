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

<tr>
    <td><a href="javascript:uiDeviceQuery.inquireEventLog(<%= _model.LiveID %>);"><%= _model.DeviceUri %></a> </td>
    <td><%= (String)ViewBag.UserName %></td>
    <td><%= _model.CurrentLevel.HasValue ? _model.LevelExpression.Description : null %></td>
    <td>
        <%--<div class="btn-group dropdown" data-toggle="dropdown">
            <button class="btn bg-color-blueLight" data-toggle="dropdown" aria-expanded="false">請選擇功能</button>
            <button class="btn bg-color-blueLight dropdown-toggle" data-toggle="dropdown" aria-expanded="true"><span class="caret"></span></button>
            <ul class="dropdown-menu">
                <%  if (_model.CurrentLevel == (int)Naming.MemberStatusDefinition.Mark_To_Delete)
                    { %>
                <li><a class="btn" onclick="uiInquireBusiness.activate(<%= _model.BusinessID %>,<%= _model.MasterID %>,<%= _model.RelativeID %>);">啟用</a></li>
                <%  }
                    else
                    { %>
                <li><a class="btn" onclick="uiInquireBusiness.deactivate(<%= _model.BusinessID %>,<%= _model.MasterID %>,<%= _model.RelativeID %>);">停用</a></li>
                <%  } %>
                <%  if (_model.Counterpart.OrganizationStatus.Entrusting != true)
                    { %>
                <li><a class="btn" onclick="uiInquireBusiness.entrusting(<%= _model.BusinessID %>,<%= _model.MasterID %>,<%= _model.RelativeID %>,true);">設定自動接收</a></li>
                <%  }
                    else
                    { %>
                <li><a class="btn" onclick="uiInquireBusiness.entrusting(<%= _model.BusinessID %>,<%= _model.MasterID %>,<%= _model.RelativeID %>,false);">停用自動接收</a></li>
                <%  } %>
                <%  if (_model.Counterpart.OrganizationStatus.EntrustToPrint != true)
                    { %>
                <li><a class="btn" onclick="uiInquireBusiness.entrustToPrint(<%= _model.BusinessID %>,<%= _model.MasterID %>,<%= _model.RelativeID %>,true);">啟用列印</a></li>
                <%  }
                    else
                    { %>
                <li><a class="btn" onclick="uiInquireBusiness.entrustToPrint(<%= _model.BusinessID %>,<%= _model.MasterID %>,<%= _model.RelativeID %>,false);">停用列印</a></li>
                <%  } %>
                <li><a class="btn" onclick="uiInquireBusiness.deleteItem(<%= _model.BusinessID %>,<%= _model.MasterID %>,<%= _model.RelativeID %>);">刪除</a></li>
            </ul>
        </div>--%>
    </td>
</tr>


<script runat="server">

    LiveDevice _model;
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        _model = (LiveDevice)this.Model;
    }

</script>
