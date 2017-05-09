<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Linq.Expressions" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="WebHome.Controllers" %>
<%@ Import Namespace="WebHome.Helper" %>
<%@ Import Namespace="WebHome.Models" %>
<%@ Import Namespace="WebHome.DataModels" %>
<%@ Import Namespace="WebHome.Models.DataEntity" %>
<%@ Import Namespace="WebHome.Models.Locale" %>
<%@ Import Namespace="WebHome.Models.ViewModel" %>
<%@ Import Namespace="Utility" %>
<%@ Import Namespace="Newtonsoft.Json" %>

<table class="table01 itemList">
    <thead>
        <tr>
            <th></th>
            <th style="min-width: 120px;">Device Uri</th>
            <th style="min-width: 120px;">使用者名稱</th>
            <th style="min-width: 120px;">目前狀態</th>
            <th style="min-width: 150px" aria-sort="other">管理</th>
        </tr>
    </thead>
    <tbody>
        <%  int idx = 0;
            foreach (var item in _items)
            {
                idx++;
                var user = _db.alarm_zone.Where(z => z.id == item.DeviceID).FirstOrDefault();
                if(user!=null)
                {
                    ViewBag.UserName = user.user;
                }
                else
                {
                    ViewBag.UserName = null;
                }
                Html.RenderPartial(_dataItemView, item);
            }
        %>
    </tbody>
    <tfoot>
        <tr>
            <td colspan="5">
                <button type="button" onclick="uiDeviceQuery.deleteAll();">全部刪除</button>
            </td>
        </tr>
    </tfoot>
</table>

<script runat="server">

    IEnumerable<LiveDevice> _model;
    IEnumerable<LiveDevice> _items;
    String _dataItemView;
    DeviceQueryViewModel _viewModel;
    dnakeDB _db;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        
        _model = (IEnumerable<LiveDevice>)this.Model;
        _dataItemView = ViewBag.DataItemView ?? "~/Views/DeviceEvents/Module/DataItem.ascx";
        _viewModel = (DeviceQueryViewModel)ViewBag.ViewModel;
        _items = _model.Skip((int)_viewModel.PageIndex * _viewModel.PageSize).Take(_viewModel.PageSize);
        _db = new dnakeDB("dnake");
    }

    public override void Dispose()
    {
        _db.Dispose();
        base.Dispose();
    }


</script>
