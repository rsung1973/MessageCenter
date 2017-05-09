<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<% if (_message != null)
    { %>
<div id="msgDialog" title="系統訊息" class="bg-color-darken">
    <div class="panel panel-default bg-color-darken">
        <div class="panel-body status smart-form vote">
            <div class="who clearfix">
                <%= _message %>
            </div>
        </div>
    </div>
    <script>
        $('#msgDialog').dialog({
            width: "auto",
            height: "auto",
            resizable: true,
            //modal: true,
            closeText: "關閉",
            title: "系統訊息",
            buttons: [{
                html: "關閉",
                "class": "btn btn-primary",
                click: function () {
                    $('#msgDialog').dialog("close");
                }
            }],
            close: function (evt, ui) {
                $('#msgDialog').remove();
            }
        });
    </script>
</div>
<%  } %>
<script runat="server">
    String _message;
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        _message = (String)this.Model ?? (String)ViewBag.Message ?? Request["Message"];
    }

</script>
