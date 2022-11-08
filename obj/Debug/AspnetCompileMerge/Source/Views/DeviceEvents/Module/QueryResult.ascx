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

<%  Html.RenderPartial("~/Views/SiteAction/FunctionTitleBar.ascx", "查詢結果"); %>

<div class="border_gray" style="overflow-x: auto; max-width: 1024px;">
    <%  var recordCount = _model.Count();
        if (recordCount > 0)
        { %>
    <%  Html.RenderPartial("~/Views/DeviceEvents/Module/ItemList.ascx", _model); %>
    <input type="hidden" name="PageIndex" />
    <nav aria-label="Page navigation">
        <ul class="pagination" id="itemPagination"></ul>
    </nav>
    <script>
        $(function () {
            var obj = $('#itemPagination').twbsPagination({
                totalPages: <%= (recordCount+_viewModel.PageSize-1) / _viewModel.PageSize %>,
                        totalRecordCount: <%= recordCount %>,
                        visiblePages: 10,
                        first: '最前',
                        prev: '上頁',
                        next: '下頁',
                        last: '最後',
                        initiateStartPageClick: false,
                        onPageClick: function (event, page) {
                            uiDeviceQuery.inquire(page,function(data){
                                var $node = $('.itemList').next();
                                $('.itemList').remove();
                                $node.before(data);
                            });
                        }
                    });
                });
    </script>
    <%      if(ViewBag.ResultAction!=null)
            {
                Html.RenderPartial((String)ViewBag.ResultAction);
            } %>
    <%  }
        else
        { %>
    <font color="red">查無資料!!</font>
    <%  } %>
</div>

<script runat="server">

    IEnumerable<LiveDevice> _model;
    DeviceQueryViewModel _viewModel;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        _model = (IEnumerable<LiveDevice>)this.Model;
        _viewModel = (DeviceQueryViewModel)ViewBag.ViewModel;
    }

</script>

