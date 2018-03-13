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
using System.IO;
using WebHome.Helper.Jobs;
using WebHome.Helper;
using Newtonsoft.Json.Linq;

namespace WebHome.Controllers
{
    public class InfoCenterController : SampleController<LiveDevice>
    {
        // GET: InfoCenter
        public ActionResult Index(InfoQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            return View();
        }

        public ActionResult ResidentIndex(InfoQueryViewModel viewModel)
        {
            ViewBag.ResidentOnly = true;
            ViewBag.ViewModel = viewModel;
            return View("Index");
        }


        public ActionResult InquireResidentMessage(InfoQueryViewModel viewModel)
        {
            var result = MessageOutbound.Instance.GetResidentMessage(viewModel.ResidentID);
            if (result != null)
            {
                return View("~/Views/Outbound/BuildingInfo/QueryResult.ascx", result);
            }
            else
            {
                return new EmptyResult();
            }
        }

        public ActionResult PushMessage()
        {
            if (Request.TotalBytes <= 0)
                return new EmptyResult();

            dynamic message;

            using (StreamReader reader = new StreamReader(Request.InputStream, Request.ContentEncoding))
            {
                using (JsonTextReader jsonReader = new JsonTextReader(reader))
                {
                    message = JsonSerializer.Create().Deserialize(jsonReader);
                    if (message is JArray)
                    {
                        BusinessExtensionMethods.PushMessage((JArray)message);
                    }
                }
            }
            return Json(new { result = true });
        }

        public ActionResult EnergyIndex(EnergyQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            return View();
        }

        public ActionResult ResidentEnergyIndex(EnergyQueryViewModel viewModel)
        {
            ViewBag.ResidentOnly = true;
            ViewBag.ViewModel = viewModel;
            return View("EnergyIndex");
        }

        public ActionResult ReportEnergyDegree(EnergyQueryViewModel viewModel,int actionType)
        {
            var result = MessageOutbound.Instance.ReportEnergyDegree(viewModel,actionType);
            if (result != null)
            {
                return View("~/Views/Shared/MessageView.ascx", model: JsonConvert.SerializeObject(result));
            }
            else
            {
                return View("~/Views/Shared/MessageView.ascx", model: "資料錯誤!!");
            }
        }

        public ActionResult InquireEnergyDegree(EnergyQueryViewModel viewModel)
        {
            var result = MessageOutbound.Instance.ReportEnergyDegree(viewModel, 3);
            if (result != null)
            {
                return View("~/Views/Outbound/BuildingInfo/QueryResult.ascx", result);
            }
            else
            {
                return new EmptyResult();
            }
        }

        public ActionResult RegisterFCMToken(String userID,String fcmToken)
        {
            if (!String.IsNullOrEmpty(fcmToken))
            {
                var profile = models.GetTable<UserProfile>().Where(u => u.PID == userID || u.UserName == userID).FirstOrDefault();
                if (profile != null && !profile.UserFCM.Any(t => t.FCMToken == fcmToken))
                {
                    profile.UserFCM.Add(new UserFCM
                    {
                        FCMToken = fcmToken
                    });
                    models.SubmitChanges();
                    return Json(new { result = true });
                }
            }
            return Json(new { result = false });
        }

    }
}