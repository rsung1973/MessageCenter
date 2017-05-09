﻿<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>

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

<%  Html.RenderPartial("~/Views/SiteAction/FunctionTitleBar.ascx", "檢視傳送訊息"); %>

<div class="border_gray" style="overflow-x: auto; max-width: 1024px;">
    <%  var recordCount = _model.Count();
        if (recordCount > 0)
        { %>
    <%  Html.RenderPartial("~/Views/DeviceEvents/EventLog/EventLogList.ascx", _model); %>
    <input type="hidden" name="PageIndex" />
    <nav aria-label="Page navigation">
        <ul class="pagination" id="itemPagination<%= _suffix %>"></ul>
        <button type="button" id="btnReload<%= _suffix %>">重新整理</button>
    </nav>
    <script>
        $(function () {
            var obj = $('#itemPagination<%= _suffix %>').twbsPagination({
                totalPages: <%= (recordCount+_viewModel.PageSize-1) / _viewModel.PageSize %>,
                totalRecordCount: <%= recordCount %>,
                visiblePages: 10,
                first: '最前',
                prev: '上頁',
                next: '下頁',
                last: '最後',
                initiateStartPageClick: false,
                onPageClick: function (event, page) {
                    uiDeviceQuery.listEventLog(<%= _viewModel.LiveID %>,page,function(data){
                        var $target = $('#itemPagination<%= _suffix %>').parent().prev().prev();
                        var $node = $target.next();
                        $target.remove();
                        $node.before(data);
                    });
                }
            });

            $('#btnReload<%= _suffix %>').on('click',function(evt){
                uiDeviceQuery.listEventLog(<%= _viewModel.LiveID %>,1,function(data){
                    //var $node = $('.eventLogList').next();
                    //$('.eventLogList').remove();
                    //$node.before(data);
                    var $target = $('#itemPagination<%= _suffix %>').parent().prev().prev();
                    var $node = $target.next();
                    $target.remove();
                    $node.before(data);
                });
            });

            if(!uiDeviceQuery.listEventLog)   {
                uiDeviceQuery.listEventLog = function (liveID,pageNum, onPaging) {
                    if (pageNum) {
                        $('input[name="PageIndex"]').val(pageNum);
                    } else {
                        $('input[name="PageIndex"]').val('');
                        $('input[name="sort"]').remove();
                    }
                    showLoading();
                    $.post('<%= Url.Action("EventLogList","DeviceEvents") %>',{ 'LiveID': liveID,'PageIndex':pageNum },function (data) {
                        hideLoading();
                        if (data) {
                            if (onPaging) {
                                onPaging(data);
                            } else {
                                //if (uiDeviceQuery.$result)
                                //    uiDeviceQuery.$result.remove();
                                //uiDeviceQuery.$result = $(data);
                                //$('.queryAction').after(uiDeviceQuery.$result);
                            }
                        }
                    });
                }
            }
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

    IEnumerable<DeviceEventLog> _model;
    DeviceQueryViewModel _viewModel;
    String _suffix = DateTime.Now.Ticks.ToString();

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        _model = (IEnumerable<DeviceEventLog>)this.Model;
        _viewModel = (DeviceQueryViewModel)ViewBag.ViewModel;
    }

</script>

