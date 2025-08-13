using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
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
            return View("~/Views/Account/UserIndex.cshtml");
        }

        public ActionResult Inquire(UserAccountQueryViewModel viewModel)
        {
            //ViewBag.HasQuery = true;

            ViewBag.ViewModel = viewModel;

            IQueryable<UserProfile> items = models.GetTable<UserProfile>()
                .Where(u=>u.UserProfileExtension == null || u.UserProfileExtension.InstanceID == null);

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

            viewModel.CardID = viewModel.CardID.GetEfficientString();
            if (viewModel.CardID != null)
            {
                items = items.Where(t => t.UserAccessCard.Any(a => a.CardID == viewModel.CardID));
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

            if(viewModel.EnableAlarm.HasValue)
            {
                item.SubscribedAlarm = viewModel.EnableAlarm.Value;
            }
            else
            {
                item.SubscribedAlarm = null;
            }
            models.SubmitChanges();
            return Json(new { result = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CommitFloorSetting(UserProfileViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            var item = models.GetTable<UserProfile>().Where(i => i.UID == viewModel.UID).FirstOrDefault();
            if (item == null)
            {
                return View("~/Views/Shared/MessageView.cshtml", model: "資料錯誤!!");
            }

            if(item.UserProfileExtension == null)
            {
                item.UserProfileExtension = new UserProfileExtension { };
            }
            item.UserProfileExtension.Floor = viewModel.Floor;
            models.SubmitChanges();
            return Json(new { result = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CommitPowerMeterIP(UserProfileViewModel viewModel,String ipAddr)
        {
            ViewBag.ViewModel = viewModel;
            var item = models.GetTable<UserProfile>().Where(i => i.UID == viewModel.UID).FirstOrDefault();
            if (item == null)
            {
                return View("~/Views/Shared/MessageView.cshtml", model: "資料錯誤!!");
            }

            ipAddr = ipAddr.GetEfficientString();
            if (ipAddr != null)
            {
                if (!IPAddress.TryParse(ipAddr, out IPAddress addr))
                {
                    return View("~/Views/Shared/MessageView.cshtml", model: "IP錯誤!!");
                }
            }

            if (item.UserProfileExtension == null)
            {
                item.UserProfileExtension = new UserProfileExtension ();
            }

            item.UserProfileExtension.PowerMeterIP = ipAddr;
            models.SubmitChanges();

            return Json(new { result = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListCardID(UserAccountQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            var item = models.GetTable<UserProfile>().Where(i => i.UID == viewModel.UID).FirstOrDefault();
            if (item == null)
            {
                return View("~/Views/Shared/MessageView.cshtml", model: "資料錯誤!!");
            }

            return View("~/Views/Account/Module/ListCardID.cshtml", item);
        }

        public ActionResult ApplyCardID(UserAccountQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            var item = models.GetTable<UserProfile>().Where(i => i.UID == viewModel.UID).FirstOrDefault();
            if (item == null)
            {
                return View("~/Views/Shared/MessageView.cshtml", model: "資料錯誤!!");
            }

            viewModel.CardID = viewModel.CardID.GetEfficientString();
            if (viewModel.CardID == null)
            {
                return View("~/Views/Shared/MessageView.cshtml", model: "請輸入卡號!!");
            }

            var card = models.GetTable<UserAccessCard>().Where(c => c.CardID == viewModel.CardID)
                                .FirstOrDefault();
            if (card != null)
            {
                return View("~/Views/Shared/MessageView.cshtml", model: "卡號重複!!");
            }

            card = new UserAccessCard
            {
                UID = item.UID,
                CardID = viewModel.CardID,
            };

            models.GetTable<UserAccessCard>().InsertOnSubmit(card);
            models.SubmitChanges();

            return View("~/Views/Account/Module/CardItem.cshtml", card);
        }

        public ActionResult DeleteCardID(UserAccountQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            var item = models.GetTable<UserProfile>().Where(i => i.UID == viewModel.UID).FirstOrDefault();
            if (item == null)
            {
                return View("~/Views/Shared/MessageView.cshtml", model: "資料錯誤!!");
            }

            var count = models.ExecuteCommand("delete UserAccessCard where UID = {0} and CardID = {1}",
                                    viewModel.UID, viewModel.CardID);

            return Json(new { result = count > 0 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AllSettings()
        {
            return Json(AppSettings.Default, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PopBoxItem(UserAccountQueryViewModel viewModel)
        {
            //ViewBag.HasQuery = true;

            ViewBag.ViewModel = viewModel;

            viewModel.CardID = viewModel.CardID.GetEfficientString();
            bool result = false;

            viewModel.KeyID = viewModel.QRCode = viewModel.QRCode.GetEfficientString();
            if (viewModel.KeyID != null)
            {
                viewModel.LogID = viewModel.DecryptKeyValue();
            }

            String[] pid = null;
            if(viewModel.LogID.HasValue) 
            {
                var logItem = models.GetTable<BoxStorageLog>().Where(b => b.LogID == viewModel.LogID).FirstOrDefault();
                if (logItem != null && !logItem.PopDate.HasValue) 
                {
                    models.ExecuteCommand(@"UPDATE  BoxStorageLog
                        SET        PopDate = GETDATE()
                        FROM     UserProfile INNER JOIN
                                     BoxStorageLog ON UserProfile.UID = BoxStorageLog.UID
                        WHERE   (BoxStorageLog.PopDate IS NULL) AND (BoxStorageLog.UID = {0})", logItem.UID);

                    pid = logItem.UserProfile?.PID.Split('-');
                }
            }
            else
            {
                var cardItem = models.GetTable<UserAccessCard>().Where(a => a.CardID == viewModel.CardID)
                                    .FirstOrDefault();
                if (cardItem != null)
                {
                    pid = cardItem.UserProfile.PID.Split('-');
                }
            }

            if (pid != null) 
            {
                if (pid.Length > 1)
                {
                    result = StorageBoxAgent.RetrieveAll(pid[1]);
                }
                else
                {
                    result = StorageBoxAgent.RetrieveAll(pid[0]);
                }
            }

            return Json(new { result = result ? 1 : 0 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckAccess(UserAccountQueryViewModel viewModel)
        {
            //ViewBag.HasQuery = true;

            ViewBag.ViewModel = viewModel;

            viewModel.KeyID = viewModel.QRCode = viewModel.QRCode.GetEfficientString();
            if (viewModel.KeyID != null)
            {
                viewModel.LogID = viewModel.DecryptKeyValue();
            }

            var logItem = models.GetTable<BoxStorageLog>().Where(b => b.LogID == viewModel.LogID).FirstOrDefault();
            if(logItem == null)
            {
                if (models.GetTable<UserProfile>().Any(p => p.PID == viewModel.ResidentID))
                {
                    return Json(new { result = 1 }, JsonRequestBehavior.AllowGet);
                }
            }
            else if (!logItem.PopDate.HasValue)
            {
                return Json(new { result = 1 }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { result = 0 }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult RequestAccess(UserAccountQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            viewModel.KeyID = viewModel.QRCode = viewModel.QRCode.GetEfficientString();
            if (viewModel.KeyID != null)
            {
                viewModel.LogID = viewModel.DecryptKeyValue();
            }

            var logItem = models.GetTable<BoxStorageLog>().Where(b => b.LogID == viewModel.LogID).FirstOrDefault();
            if (logItem == null)
            {
                var user = models.GetTable<UserProfile>().Where(p => p.PID == viewModel.ResidentID)
                    .FirstOrDefault();
                if (user != null)
                {
                    StorageBoxSettings elevator = null;
                    int floor = -1;

                    if (user.UserProfileExtension.Floor.HasValue)
                    {
                        floor = user.UserProfileExtension.Floor.Value;
                        if (AppSettings.Default.ElevatorCount > 0 && floor >= 0)
                        {
                            var idx = (DateTime.Now.Millisecond % AppSettings.Default.ElevatorCount.Value) * (AppSettings.Default.BuildingFloors + 15) / 16
                                        + (user.UserProfileExtension.Floor / 16);
                            if (idx >= 0 && idx < AppSettings.Default.ElevatorBoxArray?.Length)
                            {
                                elevator = AppSettings.Default.ElevatorBoxArray[idx.Value];
                            }
                        }
                    }
                    else
                    {
                        String No_Floor = user.PID.Right(4);
                        String No = No_Floor.Right(2);
                        if (int.TryParse(No_Floor.Substring(0, 2), out floor))
                        {
                            floor--;
                            elevator = AppSettings.Default.ElevatorBoxArray.Where(e => e.No.EndsWith(No))
                                            .Skip(floor / 16)
                                            .FirstOrDefault();

                        }
                    }

                    if (elevator != null)
                    {
                        var agent = new StorageBoxAgent(elevator);
                        int port = floor % 16;
                        if (agent.ViewModel.Disabled.Contains(port))
                        {
                            return Json(new { result = 0, message = "該樓層管制中" }, JsonRequestBehavior.AllowGet);
                        }
                        agent.TriggerBoxPort(port);
                        return Json(new { result = 1 }, JsonRequestBehavior.AllowGet);
                    }
                    else if(floor == -99)
                    {
                        var allPorts = Enumerable.Range(0, 16);
                        foreach (var elev in AppSettings.Default.ElevatorBoxArray)
                        {
                            var agent = new StorageBoxAgent(elev);
                            agent.TriggerMultiBoxPort(allPorts);
                        }
                        return Json(new { result = 1 }, JsonRequestBehavior.AllowGet);
                    }

                }
            }
            else if (!logItem.PopDate.HasValue || logItem.PopDate.Value.AddMinutes(15) >= DateTime.Now)
            {
                logItem.PopDate = DateTime.Now;
                models.SubmitChanges();

                if (logItem.BoxSize >= 0 && logItem.BoxSize < AppSettings.Default.ElevatorBoxArray?.Length)
                {
                    var agent = new StorageBoxAgent(AppSettings.Default.ElevatorBoxArray[logItem.BoxSize.Value]);
                    if(agent.ViewModel.Disabled.Contains(logItem.BoxPort.Value))
                    {
                        return Json(new { result = 0, message = "該管制中" }, JsonRequestBehavior.AllowGet);
                    }

                    agent.TriggerBoxPort(logItem.BoxPort.Value);
                    return Json(new { result = 1 }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { result = 0 }, JsonRequestBehavior.AllowGet);

        }


    }
}