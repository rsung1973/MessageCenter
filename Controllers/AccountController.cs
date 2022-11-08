using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Newtonsoft.Json;
using Utility;
using WebHome.DataModels;
using WebHome.DataPort;
using WebHome.Helper;
using WebHome.Helper.Jobs;
using WebHome.Models.DataEntity;
using WebHome.Models.Helper;
using WebHome.Models.Locale;
using WebHome.Models.ViewModel;
using WebHome.Properties;

namespace WebHome.Controllers
{
    public class AccountController : SampleController<LiveDevice>
    {
        // GET: Account
        public ActionResult UserIndex()
        {
            return View();
        }

        public ActionResult Inquire(UserAccountQueryViewModel viewModel)
        {
            //ViewBag.HasQuery = true;

            ViewBag.ViewModel = viewModel;

            IQueryable<UserProfile> items = models.GetTable<UserProfile>();

            viewModel.PID = viewModel.PID.GetEfficientString();
            if (viewModel.PID != null)
            {
                items = items.Where(t => t.PID.Contains(viewModel.PID));
            }

            viewModel.UserName = viewModel.UserName.GetEfficientString();
            if (viewModel.UserName != null)
            {
                items = items.Where(t => t.UserName.Contains(viewModel.UserName));
            }

            if (viewModel.PageIndex.HasValue)
            {
                viewModel.PageIndex = viewModel.PageIndex - 1;
                return View("~/Views/Account/Module/ItemList.cshtml", items);
            }
            else
            {
                viewModel.PageIndex = 0;
                return View("~/Views/Account/Module/QueryResult.cshtml", items);
            }
        }

        public ActionResult CommitAlarmSetting(UserProfileViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            var item = models.GetTable<UserProfile>().Where(i => i.UID == viewModel.UID).FirstOrDefault();
            if (item == null)
            {
                return View("~/Views/Shared/MessageView.cshtml", model: "資料錯誤!!");
            }

            if(viewModel.EnableAlarm==true)
            {
                item.SubscribedAlarm = (int)Naming.AlarmSubscription.公共設施;
            }
            else
            {
                item.SubscribedAlarm = null;
            }
            models.SubmitChanges();
            return Json(new { result = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AllSettings()
        {
            return Json(AppSettings.Default, JsonRequestBehavior.AllowGet);
        }

    }
}