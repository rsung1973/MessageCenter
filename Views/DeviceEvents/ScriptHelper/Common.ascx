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

        uiDeviceQuery.sendEvent = function (value) {
            $.post('<%= Url.Action("SendEvent","DeviceEvents") %>', { 'logID': value }, function (data) {
                $(data).appendTo($('body'));
            });
        };

        uiDeviceQuery.sendStatus = function (value) {
            $.post('<%= Url.Action("SendStatus","DeviceEvents") %>', { 'logID': value }, function (data) {
                $(data).appendTo($('body'));
            });
        };


        uiDeviceQuery.deleteItem = function (value) {
            var event = event || window.event;
            if (confirm('確定刪除?')) {
                $.post('<%= Url.Action("DeleteDevice","DeviceEvents") %>', { 'liveID': value }, function (data) {
                    $(data).appendTo($('body'));
                    if (data.indexOf('已刪除') > 0) {
                        $(event.target).closest('tr').remove();
                    }
                });
            }
        };

        uiDeviceQuery.deleteUri = function (value) {
            var event = event || window.event;
            if (confirm('確定刪除?')) {
                $.post('<%= Url.Action("DeleteDevicesByUri","DeviceEvents") %>', { 'uri': $('textarea[name="DeviceUri"]').val() }, function (data) {
                    $(data).appendTo($('body'));
                });
            }
        };

        uiDeviceQuery.deleteAll = function (value) {
            var event = event || window.event;
            if (confirm('確定刪除?')) {
                var values = $('input[name="liveID"]:checked').serialize();
                if (values) {
                    $.post('<%= Url.Action("DeleteDevices","DeviceEvents") %>', values, function (data) {
                        $(data).appendTo($('body'));
                    });
                } else {
                    alert('請選擇刪除項目!!')
                }
            }
        };

        uiDeviceQuery.inquireResidentMessage = function () {

            $('#theForm').ajaxForm({
                url: "<%= Url.Action("InquireResidentMessage","InfoCenter",new { resultAction = ViewBag.ResultAction }) %>",
                beforeSubmit: function () {
                    showLoading();
                },
                success: function (data) {
                    hideLoading();
                    if (data) {
                        if (uiDeviceQuery.$result)
                            uiDeviceQuery.$result.remove();
                        uiDeviceQuery.$result = $(data);
                        $('.queryAction').after(uiDeviceQuery.$result);
                    }
                },
                error: function () {
                    hideLoading();
                }
            }).submit();
        };

        uiDeviceQuery.reportEnergyDegree = function (actionType) {

            $('#theForm').ajaxForm({
                url: "<%= Url.Action("ReportEnergyDegree","InfoCenter",new { resultAction = ViewBag.ResultAction }) %>" + '?actionType=' + actionType,
                beforeSubmit: function () {
                    showLoading();
                },
                success: function (data) {
                    hideLoading();
                    if (data) {
                        $(data).appendTo($('body'));
                    }
                },
                error: function () {
                    hideLoading();
                }
            }).submit();
        };

        uiDeviceQuery.inquireEnergyDegree = function () {

            $('#theForm').ajaxForm({
                url: "<%= Url.Action("InquireEnergyDegree","InfoCenter",new { resultAction = ViewBag.ResultAction }) %>" + '?actionType=3',
                beforeSubmit: function () {
                    showLoading();
                },
                success: function (data) {
                    hideLoading();
                    if (data) {
                        if (uiDeviceQuery.$result)
                            uiDeviceQuery.$result.remove();
                        uiDeviceQuery.$result = $(data);
                        $('.queryAction').after(uiDeviceQuery.$result);
                    }
                },
                error: function () {
                    hideLoading();
                }
            }).submit();
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

