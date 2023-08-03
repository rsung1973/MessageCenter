using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebHome.DataModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebHome.Models.Helper;
using WebHome.DataPort;
using WebHome.Helper;
using WebHome.Properties;
using Utility;
using WebHome.Models.DataEntity;

namespace WebHome.Controllers
{
    public class OutboundController : SampleController<LiveDevice>
    {
        public OutboundController() : base()
        {

        }
        // GET: Outbound
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Test()
        {
            Logger.Info("Test...");
            dnakeDB db = new DataModels.dnakeDB("dnake");
            dynamic t = new DynamicQueryStringParameter();
            t.aaa = "hello...";

            return new EmptyResult();
        }

        public ActionResult GetAlarmZone()
        {
            using (dnakeDB db = new dnakeDB("dnake"))
            {
                var msg = String.Join(",", db.alarm_zone.Select(a => a.id.ToString()));
                return Content(msg);
            }
        }

        public ActionResult ApplyCenterBuildingInfo()
        {
            var result = MessageOutbound.Instance.ApplyBuildingInfo();
            if (result != null)
            {
                return View("~/Views/Outbound/BuildingInfo.cshtml", result);
            }
            else
            {
                return new EmptyResult();
            }
        }

        public ActionResult ApplyDevicesInfo()
        {
            var result = MessageOutbound.Instance.QueryAllDevices();
            if (result != null)
            {
                return View("~/Views/Outbound/DevicesInfo.aspx", result);
            }
            else
            {
                return new EmptyResult();
            }
        }

        public ActionResult ResidentInfo()
        {
            var result = MessageOutbound.Instance.GetResidentInfo();
            if (result != null)
            {
                return View("~/Views/Outbound/ResidentInfo.aspx", result);
            }
            else
            {
                return new EmptyResult();
            }
        }

        public ActionResult SynchronizeDevices()
        {
            BusinessExtensionMethods.SynchronizeUserDevices();
            return View("~/Views/Shared/MessageView.cshtml", model: "登錄設備已啟動!!");
        }

        public ActionResult CheckDeviceAlive()
        {
            BusinessExtensionMethods.CheckDeviceAlive();
            return Content("Action done !!");
        }

        public ActionResult TestIndex()
        {
            return View();
        }
    }
}