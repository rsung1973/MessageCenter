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

<table class="table01 eventLogList">
    <thead>
        <tr>
            <th style="min-width: 120px;">Device Uri</th>
            <th style="min-width: 120px;">傳送時間</th>
            <th style="min-width: 120px;">事件代碼</th>
            <th style="min-width: 120px;">Tx</th>
            <th style="min-width: 120px;">Rx</th>
        </tr>
    </thead>
    <tbody>
        <%  int idx = 0;
            foreach (var item in _items)
            {
                idx++;
                Html.RenderPartial(_dataItemView, item);
            }
        %>
    </tbody>
    <tfoot>
        <tr>
            <td colspan="5">&nbsp;
            </td>
        </tr>
    </tfoot>
</table>

<script runat="server">

    IEnumerable<DeviceEventLog> _model;
    IEnumerable<DeviceEventLog> _items;
    String _dataItemView;
    DeviceQueryViewModel _viewModel;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        _model = (IEnumerable<DeviceEventLog>)this.Model;
        _dataItemView = ViewBag.DataItemView ?? "~/Views/DeviceEvents/EventLog/DataItem.ascx";
        _viewModel = (DeviceQueryViewModel)ViewBag.ViewModel;
        _items = _model.Skip((int)_viewModel.PageIndex * _viewModel.PageSize).Take(_viewModel.PageSize);

    }

</script>
