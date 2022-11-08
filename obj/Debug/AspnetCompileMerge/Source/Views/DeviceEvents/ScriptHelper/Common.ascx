<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Linq.Expressions" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="WebHome.Helper" %>
<%@ Import Namespace="WebHome.Models.Locale" %>
<%@ Import Namespace="WebHome.Models.ViewModel" %>
<%@ Import Namespace="WebHome.Models.DataEntity" %>
<%@ Import Namespace="WebHome.Controllers" %>

<script>

    $(function () {
        var $postForm;
        uiDeviceQuery.inquire = function (pageNum, onPaging) {
            if (pageNum) {
                $('input[name="PageIndex"]').val(pageNum);
            } else {
                $('input[name="PageIndex"]').val('');
                $('input[name="sort"]').remove();
            }
            $('#theForm').ajaxForm({
                url: "<%= Url.Action("Inquire","DeviceEvents",new { resultAction = ViewBag.ResultAction }) %>",
                beforeSubmit: function () {
                    showLoading();
                },
                success: function (data) {
                    hideLoading();
                    if (data) {
                        if (onPaging) {
                            onPaging(data);
                        } else {
                            if (uiDeviceQuery.$result)
                                uiDeviceQuery.$result.remove();
                            uiDeviceQuery.$result = $(data);
                            $('.queryAction').after(uiDeviceQuery.$result);
                        }
                    }
                },
                error: function () {
                    hideLoading();
                }
            }).submit();
        };

        uiDeviceQuery.inquireEventLog = function (value) {
            $.post('<%= Url.Action("EventLogList","DeviceEvents") %>', { 'LiveID': value }, function (data) {
                $global.createTab('listEvents' + (Math.random() * 100000).toFixed(0).toString(), '傳送訊息', data, true);
            });
        };

    });
</script>
<script runat="server">

    ModelSource<LiveDevice> models;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

    }
</script>

