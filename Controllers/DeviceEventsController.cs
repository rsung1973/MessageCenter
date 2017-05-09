using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebHome.Models.DataEntity;
using WebHome.Models.ViewModel;
using Utility;
using WebHome.DataPort;
using WebHome.Models.Locale;
using Newtonsoft.Json;
using WebHome.Models.Helper;
using WebHome.Properties;

namespace WebHome.Controllers
{
    public class DeviceEventsController : SampleController<LiveDevice>
    {
        public DeviceEventsController() : base()
        {

        }
        // GET: DeviceEvents
        public ActionResult Index(DeviceQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            return View();
        }

        public ActionResult Inquire(DeviceQueryViewModel viewModel)
        {
            //ViewBag.HasQuery = true;

            ViewBag.ViewModel = viewModel;

            var items = models.GetTable<LiveDevice>().Where(t => true);

            viewModel.DeviceUri = viewModel.DeviceUri.GetEfficientString();
            if (viewModel.DeviceUri != null)
            {
                items = items.Where(t => t.DeviceUri.Contains(viewModel.DeviceUri));
            }

            ViewBag.DataItemView = "~/Views/DeviceEvents/Module/DataItem.ascx";

            if (viewModel.PageIndex.HasValue)
            {
                viewModel.PageIndex = viewModel.PageIndex - 1;
                return View("~/Views/DeviceEvents/Module/ItemList.ascx", items);
            }
            else
            {
                viewModel.PageIndex = 0;
                return View("~/Views/DeviceEvents/Module/QueryResult.ascx", items);
            }
        }

        public ActionResult EventLogList(DeviceQueryViewModel viewModel)
        {
            //ViewBag.HasQuery = true;

            ViewBag.ViewModel = viewModel;

            var items = models.GetTable<DeviceEventLog>().Where(t => t.LiveID == viewModel.LiveID);

            if (viewModel.PageIndex.HasValue)
            {
                viewModel.PageIndex = viewModel.PageIndex - 1;
                return View("~/Views/DeviceEvents/EventLog/EventLogList.ascx", items);
            }
            else
            {
                viewModel.PageIndex = 0;
                return View("~/Views/DeviceEvents/EventLog/EventLogReport.ascx", items);
            }
        }

        public ActionResult SendEvent(int? logID)
        {
            var item = models.GetTable<DeviceEventLog>().Where(l => l.LogID == logID).FirstOrDefault();
            String message;
            if (item != null)
            {
                dynamic result = MessageOutbound.Instance.ReportDeviceEvent(item.LiveDevice, item.EventCode, "1");
                message = JsonConvert.SerializeObject(result);
            }
            else
            {
                message = "事件記錄不存在!!";
            }
            return View("~/Views/Shared/MessageView.ascx", model: message);
        }

        public ActionResult SendStatus(int? logID)
        {
            var item = models.GetTable<DeviceEventLog>().Where(l => l.LogID == logID).FirstOrDefault();
            String message;
            if (item != null)
            {
                dynamic result = MessageOutbound.Instance.UpdateDeviceStatus(item.LiveDevice, item.EventCode);
                message = JsonConvert.SerializeObject(result);
            }
            else
            {
                message = "事件記錄不存在!!";
            }
            return View("~/Views/Shared/MessageView.ascx", model: message);
        }

        public ActionResult DeleteDevice(int? liveID)
        {
            String message;
            message = deleteDeviceItem(liveID);
            return View("~/Views/Shared/MessageView.ascx", model: message);
        }

        public ActionResult DeleteDevices(int?[] liveID)
        {
            String message;
            message = String.Join("<br/>", liveID.Select(i => deleteDeviceItem(i)));
            return View("~/Views/Shared/MessageView.ascx", model: message);
        }

        public ActionResult DeleteDevicesByUri(String uri)
        {
            String[] deviceUri = uri.Split(new String[] { ",", ";", "\r\n","\n" }, StringSplitOptions.RemoveEmptyEntries);
            String message;
            message = String.Join("<br/>", deviceUri.Select(i => deleteDeviceItem(i)));
            return View("~/Views/Shared/MessageView.ascx", model: message);
        }


        private string deleteDeviceItem(int? liveID)
        {
            string message;
            var item = models.GetTable<LiveDevice>().Where(l => l.LiveID == liveID).FirstOrDefault();
            if (item != null)
            {
                dynamic queryValues = new DynamicQueryStringParameter();
                queryValues.prm_l1_device_type = Settings.Default.PRMType;
                queryValues.prm_uri = item.DeviceUri;
                var result = MessageOutbound.Instance.DeleteDevice(queryValues);

                if (result != null && String.IsNullOrEmpty((String)result[0].return_message))
                {
                    message = item.DeviceUri + "=>資料已刪除!!";
                }
                else
                {
                    message = item.DeviceUri + "=>" + JsonConvert.SerializeObject(result);
                    Logger.Warn(message);
                }

                models.ExecuteCommand("delete DeviceEventReport where LiveID = {0}", liveID);
                models.ExecuteCommand("delete DeviceEventLog where LiveID = {0}", liveID);
                models.DeleteAny<LiveDevice>(d => d.LiveID == liveID);

            }
            else
            {
                message = "設備記錄不存在!!";
            }

            return message;
        }

        private string deleteDeviceItem(String deviceUri)
        {
            string message;
            dynamic queryValues = new DynamicQueryStringParameter();
            queryValues.prm_l1_device_type = Settings.Default.PRMType;
            queryValues.prm_uri = deviceUri;
            var result = MessageOutbound.Instance.DeleteDevice(queryValues);

            if (result != null && String.IsNullOrEmpty((String)result[0].return_message))
            {
                message = deviceUri + "=>資料已刪除!!";
            }
            else
            {
                message = deviceUri + "=>" + JsonConvert.SerializeObject(result);
                Logger.Warn(message);
            }

            return message;
        } 

    }
}