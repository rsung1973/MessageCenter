using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.Composition.Primitives;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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

        public ActionResult ResidentDeviceIndex(DeviceQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            ViewBag.ResidentOnly = true;
            ViewBag.InquiryView = "~/Views/DeviceEvents/Module/ResidentEventLogs.cshtml";
            return View("~/Views/DeviceEvents/Index.cshtml");
        }

        public ActionResult MoreServices(DeviceQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            ViewBag.ResidentOnly = true;
            ViewBag.InquiryView = "~/Views/DeviceEvents/Module/MoreServices.cshtml";
            return View("~/Views/DeviceEvents/Index.cshtml");
        }

        public ActionResult AccessCard(DeviceQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            ViewBag.ResidentOnly = true;
            ViewBag.InquiryView = "~/Views/DeviceEvents/Module/AccessCard.cshtml";
            return View("~/Views/DeviceEvents/Index.cshtml");
        }

        public ActionResult InitStorageBox(StorageBoxViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            ViewBag.InquiryView = "~/Views/DeviceEvents/Module/InitStorageBox.cshtml";
            return View("~/Views/DeviceEvents/Index.cshtml");
        }

        public ActionResult InitElevatorBox(ElevatorBoxViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            ViewBag.InquiryView = "~/Views/DeviceEvents/Module/InitElevatorBox.cshtml";
            return View("~/Views/DeviceEvents/Index.cshtml");
        }

        public ActionResult ShowStorageBox(StorageBoxViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            var agent = StorageBoxAgent.AcquireAgent(viewModel.BoxDeviceIndex);
            if (agent == null)
            {
                return Json(new { result = false,message = "郵箱設備未設定!!" }, JsonRequestBehavior.AllowGet); 
            }

            return View("~/Views/DeviceEvents/Module/StorageBox.cshtml", agent.ViewModel);
        }


        public ActionResult Inquire(DeviceQueryViewModel viewModel)
        {
            //ViewBag.HasQuery = true;

            ViewBag.ViewModel = viewModel;

            var items = models.GetTable<LiveDevice>().Where(t => true);

            viewModel.DeviceUri = viewModel.DeviceUri.GetEfficientString();
            if (viewModel.DeviceUri != null)
            {
                items = items.Where(t => t.UserRegister.DeviceUri.Contains(viewModel.DeviceUri));
            }

            ViewBag.DataItemView = "~/Views/DeviceEvents/Module/DataItem.cshtml";

            if (viewModel.PageIndex.HasValue)
            {
                viewModel.PageIndex = viewModel.PageIndex - 1;
                return View("~/Views/DeviceEvents/Module/ItemList.cshtml", items);
            }
            else
            {
                viewModel.PageIndex = 0;
                return View("~/Views/DeviceEvents/Module/QueryResult.cshtml", items);
            }
        }

        public ActionResult InquireResidentEventLog(DeviceQueryViewModel viewModel)
        {
            //ViewBag.HasQuery = true;

            ViewBag.ViewModel = viewModel;

            viewModel.ResidentID = viewModel.ResidentID.GetEfficientString();
            var items = models.GetTable<UserProfile>().Where(t => t.PID == viewModel.ResidentID)
                    .Join(models.GetTable<UserRegister>(), u => u.UID, r => r.UID, (u, r) => r)
                    .Join(models.GetTable<LiveDevice>(), r => r.UID, d => d.UID, (r, d) => d)
                    .Join(models.GetTable<DeviceEventLog>(), d => d.LiveID, v => v.LiveID, (d, v) => v);


            ViewBag.DataItemView = "~/Views/DeviceEvents/EventLog/DataItem.cshtml";

            if (viewModel.PageIndex.HasValue)
            {
                viewModel.PageIndex = viewModel.PageIndex - 1;
                return View("~/Views/DeviceEvents/EventLog/EventLogList.cshtml", items);
            }
            else
            {
                viewModel.PageIndex = 0;
                return View("~/Views/DeviceEvents/Module/ResidentEventLogQueryResult.cshtml", items);
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
                return View("~/Views/DeviceEvents/EventLog/EventLogList.cshtml", items);
            }
            else
            {
                viewModel.PageIndex = 0;
                return View("~/Views/DeviceEvents/EventLog/EventLogReport.cshtml", items);
            }
        }

        public ActionResult SendEvent(int? logID)
        {
            var item = models.GetTable<DeviceEventLog>().Where(l => l.LogID == logID).FirstOrDefault();
            String message;
            if (item != null)
            {
                dynamic result = MessageOutbound.Instance.ReportDeviceEvent(item.LiveDevice.UserRegister.DeviceUri, item.EventCode, "1");
                message = JsonConvert.SerializeObject(result);
            }
            else
            {
                message = "事件記錄不存在!!";
            }
            return View("~/Views/Shared/MessageView.cshtml", model: message);
        }

        public ActionResult SendStatus(int? logID)
        {
            var item = models.GetTable<DeviceEventLog>().Where(l => l.LogID == logID).FirstOrDefault();
            String message;
            if (item != null)
            {
                dynamic result = MessageOutbound.Instance.UpdateDeviceStatus(item.LiveDevice.UserRegister.DeviceUri, item.EventCode);
                message = JsonConvert.SerializeObject(result);
            }
            else
            {
                message = "事件記錄不存在!!";
            }
            return View("~/Views/Shared/MessageView.cshtml", model: message);
        }

        public ActionResult DeleteDevice(int? liveID)
        {
            String message;
            message = deleteDeviceItem(liveID);
            return View("~/Views/Shared/MessageView.cshtml", model: message);
        }

        public ActionResult DeleteDevices(int?[] liveID)
        {
            String message;
            message = String.Join("<br/>", liveID.Select(i => deleteDeviceItem(i)));
            return View("~/Views/Shared/MessageView.cshtml", model: message);
        }

        public ActionResult DeleteDevicesByUri(String uri)
        {
            String[] deviceUri = uri.Split(new String[] { ",", ";", "\r\n","\n" }, StringSplitOptions.RemoveEmptyEntries);
            String message;
            message = String.Join("<br/>", deviceUri.Select(i => deleteDeviceItem(i)));
            return View("~/Views/Shared/MessageView.cshtml", model: message);
        }


        private string deleteDeviceItem(int? liveID)
        {
            string message;
            var item = models.GetTable<LiveDevice>().Where(l => l.LiveID == liveID).FirstOrDefault();
            var userReg = item.UserRegister;

            if (item != null)
            {
                models.ExecuteCommand("delete DeviceEventReport where LiveID = {0}", liveID);
                models.ExecuteCommand("delete DeviceEventLog where LiveID = {0}", liveID);
                models.DeleteAny<LiveDevice>(d => d.LiveID == liveID);

                if (userReg != null)
                {
                    var count = models.GetTable<LiveDevice>().Where(d => d.UID == userReg.UID).Count();
                    if (count == 0) //該用戶已裝置
                    {
                        dynamic queryValues = new DynamicQueryStringParameter();
                        queryValues.prm_l1_device_type = Settings.Default.PRMType;
                        queryValues.prm_uri = userReg.DeviceUri;
                        var result = MessageOutbound.Instance.DeleteDevice(queryValues);

                        if (result != null && String.IsNullOrEmpty((String)result[0].return_message))
                        {
                            message = userReg.DeviceUri + "=>資料已刪除!!";
                            models.ExecuteCommand("delete UserProfile where UID = {0}", userReg.UID);
                        }
                        else
                        {
                            message = userReg.DeviceUri + "=>" + JsonConvert.SerializeObject(result);
                            Logger.Warn(message);
                        }
                    }
                    else
                    {
                        message = liveID + ":設備已刪除!!";
                    }
                }
                else
                {
                    message = liveID + ":設備已刪除!!";
                }
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
                    return Content(JsonConvert.SerializeObject((new TouchLifeDispatcher()).PushMessage(message)), "application/json");
                }
            }
        }

        public ActionResult CheckAuthToken()
        {
            return Json(MessageOutbound.Instance.CheckAuthToken(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult UserAlarmXlsx()
        {

            DataTable table = new DataTable();

            table.Columns.Add(new DataColumn("編號", typeof(int)));
            table.Columns.Add(new DataColumn("棟別", typeof(int)));
            table.Columns.Add(new DataColumn("房號", typeof(int)));
            table.Columns.Add(new DataColumn("號碼", typeof(String)));
            table.Columns.Add(new DataColumn("保全迴路", typeof(String)));
            table.Columns.Add(new DataColumn("名稱", typeof(String)));
            table.Columns.Add(new DataColumn("狀態", typeof(int)));
            table.Columns.Add(new DataColumn("警報時間", typeof(DateTime)));

            using (dnakeDB db = new dnakeDB("dnake"))
            {
                var loggers = db.GetTable<alarm_logger>().Where(a => a.confirm == 0)
                    .OrderByDescending(a => a.ts)
                    .ToList();

                //foreach (var item in loggers)
                //{
                //    var zone = db.alarm_zone.Where(z => z.user == item.user && z.zone == item.zone).FirstOrDefault();
                //    var user = db.users.Where(u => u.user_Column == item.user).FirstOrDefault();
                //    if (zone != null)

                //    if (zone!=null)
                //    {
                //            alarm_sensor sensor = db.alarm_sensor.Where(s => s.sensor == zone.sensor).FirstOrDefault();
                //        var r = table.NewRow();
                //        r[0] = zone.id;
                //        r[1] = user.build;
                //        r[2] = user.room;
                //        r[3] = $"{user.floor:0}-{user.unit}";
                //        r[4] = zone.name;
                //        r[5] = sensor.name;
                //        r[6] = 0;
                //        if (defence != null && defence.defence > 0)
                //        {
                //            if (defence.defence > 0 && guarded[item.Zone.zone] == 1)
                //            {
                //                r[6] = defence.defence;
                //            }
                //        }

                //        table.Rows.Add(r);
                //    }

                foreach (var user in db.users.ToArray())
                {
                    var defence = db.GetTable<alarm_defence>().Where(d => d.user == user.user_Column)
                                    .OrderByDescending(d => d.id).FirstOrDefault();
                    int[] guarded = null;
                    if (defence != null)
                    {
                        guarded = defence.io.Split(',').Select(s => int.Parse(s)).ToArray();
                    }

                    var items = db.alarm_zone.Where(i => i.user == user.user_Column)
                                    .Join(db.alarm_sensor, i => i.sensor, s => s.sensor, (i, s) => new { Zone = i, Sensor = s }).ToList();
                    foreach (var item in items)
                    {
                        var logger = loggers.Where(l => l.zone == item.Zone.zone && l.user == item.Zone.user).FirstOrDefault();
                        var r = table.NewRow();
                        r[0] = item.Zone.id;
                        r[1] = user.build;
                        r[2] = user.room;
                        //r[3] = $"{user.floor:0}-{user.unit}";
                        r[3] = $"{user.floor:0}";
                        r[4] = item.Zone.name;
                        r[5] = item.Sensor.name;
                        r[6] = 0;

                        if (logger != null)
                        {
                            r[6] = 3;
                            r[7] = logger.ts;
                        }
                        else if (defence != null && defence.defence > 0)
                        {
                            if (defence.defence > 0 && guarded[item.Zone.zone] == 1)
                            {
                                r[6] = defence.defence;
                            }
                        }

                        table.Rows.Add(r);
                    }
                }

            }

            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.AddHeader("Cache-control", "max-age=1");
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-Disposition", $"attachment;filename={HttpUtility.UrlEncode("保全")}.xlsx");

            using (DataSet ds = new DataSet())
            {
                ds.Tables.Add(table);

                using (var xls = ds.ConvertToExcel())
                {
                    xls.Worksheets.ElementAt(0).Name = "保全";
                    xls.SaveAs(Response.OutputStream);
                }
            }

            return new EmptyResult();
        }

        public ActionResult TaxiOrderIndex(TaxiOrderViewModel viewModel)
        {
            TaiwanTaxiAgent agent = TaiwanTaxiAgent.PrepareTaxiAgent(viewModel);
            var lastOrder = agent.LoadData<TaxiOrderViewModel>("TaxiOrderViewModel.json");

            if(lastOrder!=null)
            {
                viewModel = lastOrder;
            }

            ViewBag.ViewModel = viewModel;
            ViewBag.ResidentOnly = true;
            ViewBag.InquiryView = "~/Views/DeviceEvents/Module/BuildingTaxiOrder.cshtml";
            return View("~/Views/DeviceEvents/Index.cshtml");
        }

        public ActionResult YoxiOrderIndex(YoxiAgentViewModel viewModel)
        {
            YoxiAgent agent = YoxiAgent.PrepareTaxiAgent(viewModel);
            var lastOrder = agent.LoadData<YoxiAgentViewModel>($"Yoxi-{viewModel.ResidentID}.json");

            if (lastOrder != null)
            {
                viewModel = lastOrder;
            }

            ViewBag.ViewModel = viewModel;
            ViewBag.ResidentOnly = true;
            ViewBag.InquiryView = "~/Views/DeviceEvents/Module/BuildingYoxiOrder.cshtml";
            return View("~/Views/DeviceEvents/Index.cshtml");
        }

        public ActionResult CommitAccessCard(InfoQueryViewModel viewModel)
        {
            Request.SaveAs(Path.Combine(Logger.LogDailyPath, "AccessCard-" + DateTime.Now.Ticks + ".txt"), true);

            viewModel.BranchNo = viewModel.BranchNo.GetEfficientString();
            if (viewModel.BranchNo == null)
            {
                this.ModelState.AddModelError("BranchNo", "請輸入分機號碼!!");
            }

            if (!viewModel.Cards.HasValue)
            {
                this.ModelState.AddModelError("Cards", "請輸卡片張數!!");
            }

            viewModel.ResidentID = viewModel.ResidentID.GetEfficientString();
            //if (viewModel.ResidentID == null)
            //{
            //    this.ModelState.AddModelError("ResidentID", "請輸入識別碼!!");
            //}

            viewModel.CustomerID = viewModel.CustomerID.GetEfficientString();
            //if (viewModel.CustomerID == null)
            //{
            //    this.ModelState.AddModelError("CustomerID", "請輸入客戶代碼!!");
            //}

            if (!ModelState.IsValid)
            {
                ViewBag.ModelState = this.ModelState;
                return View("~/Views/Shared/ReportInputError.cshtml");
            }

            Task.Run(() => 
            {
                try
                {
                    if (AppSettings.Default.RequestContentType == AppSettings.OutputContentType.FormValues)
                    {
                        NameValueCollection postParams = HttpUtility.ParseQueryString(string.Empty);
                        postParams.Add("MachineKey", viewModel.ResidentID ?? AppSettings.Default.DefaultAccessCardMachineKey);
                        postParams.Add("CustID", viewModel.CustomerID ?? AppSettings.Default.DefaultAccessCardCustomerID);
                        postParams.Add("CardCount", $"{viewModel.Cards}");
                        postParams.Add("Extension", viewModel.BranchNo);

                        using (WebClient client = new WebClient())
                        {
                            var result = client.UploadValues(AppSettings.Default.SetAccessCardUrl, postParams);
                            Logger.Info(Encoding.UTF8.GetString(result));
                        }
                    }
                    else
                    {
                        using (WebClient client = new WebClient())
                        {
                            client.Headers.Add("Content-Type", "application/json");
                            var dataItem = JsonConvert.SerializeObject(
                                                new
                                                {
                                                    MachineKey = viewModel.ResidentID ?? AppSettings.Default.DefaultAccessCardMachineKey,
                                                    CustID = viewModel.CustomerID ?? AppSettings.Default.DefaultAccessCardCustomerID,
                                                    CardCount = $"{viewModel.Cards}",
                                                    Extension = viewModel.BranchNo
                                                });

                            String result = client.UploadString(AppSettings.Default.SetAccessCardUrl, dataItem);
                            Logger.Info($"push access card:{dataItem},result:{result}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            });

            return Json(new { result = true }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult CommitAccessQRCode(InfoQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            viewModel.ElevatorNo = viewModel.ElevatorNo.GetEfficientString();
            if (viewModel.ElevatorNo == null)
            {
                this.ModelState.AddModelError("ElevatorNo", "請選擇門牌號碼!!");
            }

            if (!viewModel.Floor.HasValue)
            {
                this.ModelState.AddModelError("Floor", "請選擇樓層!!");
            }

            viewModel.ResidentID = viewModel.ResidentID.GetEfficientString();
            if (viewModel.ResidentID == null)
            {
                this.ModelState.AddModelError("ResidentID", "請輸入識別碼!!");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ModelState = this.ModelState;
                return View("~/Views/Shared/ReportInputError.cshtml");
            }

            var item = AppSettings.Default.ElevatorBoxArray.Where(l => l.No == viewModel.ElevatorNo)
                            .Skip(viewModel.Floor.Value / 16)
                            .FirstOrDefault();

            if (item != null)
            {
                int boxSize = Array.IndexOf<StorageBoxSettings>(AppSettings.Default.ElevatorBoxArray, item);
                int boxPort = viewModel.Floor.Value % 16;
                String message = $"{DateTime.Now:yyyy/MM/dd HH:mm}，歡迎搭乘{viewModel.ElevatorNo}號電梯至第{viewModel.Floor + 1}樓。";
                PushBoxStorageMessageToLine(viewModel.ResidentID, boxSize, boxPort, "訪客通行證", message);
            }

            return Json(new { result = true }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult CommitStorageBox(StorageBoxViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            if (!viewModel.BoxSize.HasValue)
            {
                this.ModelState.AddModelError("BoxSize", "請選擇郵箱櫃大小!!");
            }

            viewModel.StorageBoxUrl = viewModel.StorageBoxUrl.GetEfficientString();
            if (viewModel.StorageBoxUrl == null)
            {
                this.ModelState.AddModelError("StorageBoxUrl", "請輸入郵箱控制盒IP!!");
            }

            var agent = StorageBoxAgent.AcquireAgent(viewModel.BoxDeviceIndex);
            if (agent == null)
            {
                this.ModelState.AddModelError("Message", "郵箱設備未設定!!");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ModelState = this.ModelState;
                return View("~/Views/Shared/ReportInputError.cshtml");
            }

            agent.ViewModel.StorageBoxUrl = viewModel.StorageBoxUrl;
            agent.ViewModel.BoxSize = viewModel.BoxSize.Value;
            agent.ViewModel.Enabled = agent.GetBoxPortList() != null;
            agent.Save();

            return Json(new { result = true }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult AddStorageBox(StorageBoxViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            StorageBoxAgent.AddStorageBox();
            AppSettings.Default.Save();

            return Json(new { result = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ResetStorageBox(StorageBoxViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            var agent = StorageBoxAgent.AcquireAgent(viewModel.BoxDeviceIndex);
            if (agent == null)
            {
                return Json(new { result = false, message = "郵箱設備未設定!!" }, JsonRequestBehavior.AllowGet);
            }
            agent.ResetBox();

            return Json(new { result = true }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult RemoveStorageBox(StorageBoxViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            StorageBoxAgent.RemoveStorageBox(viewModel.BoxDeviceIndex);
            AppSettings.Default.Save();

            return Json(new { result = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PrepareToStoreItem(StorageBoxViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            if (!(viewModel.Port >= 0))
            {
                this.ModelState.AddModelError("Message", "存放櫃資料錯誤!!");
            }

            var agent = StorageBoxAgent.AcquireAgent(viewModel.BoxDeviceIndex);
            if (agent == null)
            {
                this.ModelState.AddModelError("Message", "郵箱設備未設定!!");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ModelState = this.ModelState;
                return View("~/Views/Shared/ReportInputError.cshtml");
            }

            ViewBag.BoxAgent = agent;
            return View("~/Views/DeviceEvents/Module/PrepareToStoreItem.cshtml", agent.ViewModel);

        }

        public ActionResult SetBoxStatus(StorageBoxViewModel viewModel)
        {
            var result = PrepareToStoreItem(viewModel);
            StorageBoxAgent agent = ViewBag.BoxAgent as StorageBoxAgent;
            if (agent == null)
            {
                return result;
            }

            if (viewModel.Disabled == true)
            {
                if(!agent.ViewModel.Disabled.Contains(viewModel.Port.Value))
                {
                    agent.ViewModel.Disabled.Add(viewModel.Port.Value);
                }
            }
            else
            {
                if (agent.ViewModel.Disabled.Contains(viewModel.Port.Value))
                {
                    agent.ViewModel.Disabled.Remove(viewModel.Port.Value);
                }
            }

            agent.Save();
            return Json(new { result = true }, JsonRequestBehavior.AllowGet);

        }


        public ActionResult CommitStorageItem(StorageBoxViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            viewModel.ResidentID = viewModel.ResidentID.GetEfficientString();
            if (viewModel.ResidentID == null)
            {
                this.ModelState.AddModelError("Message", "請輸入存放戶號!!");
            }

            if (!(viewModel.Port >= 0))
            {
                this.ModelState.AddModelError("Message", "存放櫃資料錯誤!!");
            }

            var agent = StorageBoxAgent.AcquireAgent(viewModel.BoxDeviceIndex);
            if (agent == null)
            {
                this.ModelState.AddModelError("Message", "郵箱設備未設定!!");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ModelState = this.ModelState;
                return View("~/Views/Shared/ReportInputError.cshtml");
            }

            if (viewModel.ActionType == 1)
            {
                agent.AddBoxItem(viewModel.Port.Value, viewModel.ResidentID);
                PushBoxStorageItemToLine(viewModel.ResidentID, agent.ViewModel.BoxSize, viewModel.Port.Value);
            }
            else if(viewModel.ActionType == 0)
            {
                agent.RemoveBoxItem(viewModel.Port.Value, viewModel.ResidentID);
                PushBoxStorageRemovalToLine(viewModel.ResidentID, agent.ViewModel.BoxSize, viewModel.Port.Value);
                models.ExecuteCommand(@"UPDATE  BoxStorageLog
                    SET        PopDate = GETDATE()
                    FROM     UserProfile INNER JOIN
                                 BoxStorageLog ON UserProfile.UID = BoxStorageLog.UID
                    WHERE   (UserProfile.PID = {0}) AND (BoxStorageLog.BoxSize = {1}) AND (BoxStorageLog.BoxPort = {2})",
                    viewModel.ResidentID, (int)agent.ViewModel.BoxSize, viewModel.Port.Value);
            }

            return Json(new { result = true }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult PrintStorageItem(StorageBoxViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            var logItem = models.GetTable<BoxStorageLog>()
                .Where(l => l.LogID == viewModel.LogID)
                .FirstOrDefault();

            return View("~/Views/DeviceEvents/Module/PrintStorageItem.cshtml", logItem);

        }

        public ActionResult BookingStorageItem(StorageBoxViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            if (viewModel.LogID.HasValue)
            {
                return PrintStorageItem(viewModel);
            }

            viewModel.ResidentID = viewModel.ResidentID.GetEfficientString();
            if (viewModel.ResidentID == null)
            {
                this.ModelState.AddModelError("Message", "請輸入存放戶號!!");
            }

            if (!viewModel.BoxSize.HasValue)
            {
                this.ModelState.AddModelError("Message", "未指定存放櫃大小!!");
            }

            if (!ModelState.IsValid)
            {
                return Json(new { port = -1, message = ModelState.ErrorMessage() }, JsonRequestBehavior.AllowGet);
            }

            var agent = StorageBoxAgent.AcquireAgent(viewModel.BoxSize.Value);
            if (agent == null)
            {
                return Json(new { port = -1, message = "郵箱設備未設定!!" }, JsonRequestBehavior.AllowGet);
            }

            var boxItem = agent.AcquireVacantBox();
            if (boxItem?.port.HasValue != true)
            {
                return Json(new { port = -1, message = "郵箱已滿!!" }, JsonRequestBehavior.AllowGet);
            }

            agent.AddBoxItem(boxItem.port.Value, viewModel.ResidentID);
            ViewBag.DataItem = PushBoxStorageItemToLine(viewModel.ResidentID, agent.ViewModel.BoxSize, boxItem.port.Value);

            return View("~/Views/DeviceEvents/Module/BookingStorageItem.cshtml", boxItem);

        }


        private BoxStorageLog PushBoxStorageItemToLine(String residentID, StorageBoxSize boxSize, int boxPort)
        {
            String message = $"{DateTime.Now:yyyy/MM/dd HH:mm}，有您的快遞寄放於{boxSize}型郵箱第{boxPort + 1}櫃。";
            return PushBoxStorageMessageToLine(residentID, (int)boxSize, boxPort, "快遞通知", message);
        }

        private BoxStorageLog PushBoxStorageRemovalToLine(String residentID, StorageBoxSize boxSize, int boxPort)
        {
            String message = $"{DateTime.Now:yyyy/MM/dd HH:mm}，您的快遞寄放於{boxSize}型郵箱第{boxPort + 1}櫃已取件。";
            return PushBoxStorageMessageToLine(residentID, (int)boxSize, boxPort, "快遞取件通知", message, true);
        }

        private BoxStorageLog PushBoxStorageMessageToLine(String residentID, int boxSize, int boxPort, String title, String message, bool removal = false)
        {
            if (string.IsNullOrEmpty(residentID))
            {
                return null;
            }

            UserProfile user;
            bool lineUser = true;
            if (residentID.Any(c => c == '-'))
            {
                user = models.GetTable<UserProfile>().Where(p => p.PID == residentID).FirstOrDefault();
            }
            else
            {
                String id = $"-{residentID}";
                user = models.GetTable<UserProfile>().Where(p => p.PID.Contains(id)).FirstOrDefault();
                if (user == null)
                {
                    lineUser = false;
                    user = models.GetTable<UserProfile>().Where(p => p.PID == residentID).FirstOrDefault();
                }
            }

            BoxStorageLog logItem = null;
            if (user != null)
            {
                if (removal)
                {
                    logItem = models.GetTable<BoxStorageLog>()
                                    .Where(b => b.UID == user.UID)
                                    .Where(b => b.BoxPort == boxPort)
                                    .Where(b => b.BoxSize == boxSize)
                                    .FirstOrDefault();

                    if (logItem != null)
                    {
                        logItem.PopDate = DateTime.Now;
                        models.SubmitChanges();
                    }

                    if (lineUser)
                    {
                        String url = $"{AppSettings.Default.LineMessageCenter}/MessageCenter/LineEvents/PushMessage";
                        url.PushToLineMessageCenter(new UserAccountQueryViewModel
                        {
                            PID = user.PID,
                            //QRCode = logItem.LogID.EncryptKey(),
                            Title = title,
                            Message = message
                        });
                    }
                }
                else
                {
                    logItem = new BoxStorageLog
                    {
                        UserProfile = user,
                        BoxSize = boxSize,
                        BoxPort = boxPort,
                        PushDate = DateTime.Now,
                    };
                    models.GetTable<BoxStorageLog>().InsertOnSubmit(logItem);
                    models.SubmitChanges();

                    if (lineUser)
                    {
                        String url = $"{AppSettings.Default.LineMessageCenter}/MessageCenter/LineEvents/PushQRCode";
                        url.PushToLineMessageCenter(new UserAccountQueryViewModel
                        {
                            PID = user.PID,
                            QRCode = logItem.LogID.EncryptKey(),
                            Title = title,
                            Message = message
                        });
                    }
                }
            }

            return logItem;
        }

        public ActionResult CommitElevatorStep0(ElevatorBoxViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            if (!(viewModel.Floors > 0))
            {
                this.ModelState.AddModelError("Floors", "請輸入樓層數!!");
            }

            if (!(viewModel.ElevatorCount > 0))
            {
                this.ModelState.AddModelError("ElevatorCount", "請輸入電梯組數!!");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ModelState = this.ModelState;
                return View("~/Views/Shared/ReportInputError.cshtml");
            }

            AppSettings.Default.BuildingFloors = viewModel.Floors;
            AppSettings.Default.ElevatorCount = viewModel.ElevatorCount;

            List<StorageBoxSettings> items = new List<StorageBoxSettings>();
            if(AppSettings.Default.ElevatorBoxArray!=null)
            {
                items.AddRange(AppSettings.Default.ElevatorBoxArray);
            }

            int total = (viewModel.Floors.Value + 15) / 16 * viewModel.ElevatorCount.Value;
            for (int i = items.Count; i < total; i++)
            {
                items.Add(new StorageBoxSettings { });
            }

            AppSettings.Default.ElevatorBoxArray = items.Take(total).ToArray();
            AppSettings.Default.Save();

            return View("~/Views/DeviceEvents/Module/ElevatorBoxStep1.cshtml");

        }

        public ActionResult CommitElevatorStep1(ElevatorBoxViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            if (!(AppSettings.Default.BuildingFloors > 0))
            {
                this.ModelState.AddModelError("Floors", "請輸入樓層數!!");
            }

            if (!(AppSettings.Default.ElevatorCount > 0))
            {
                this.ModelState.AddModelError("ElevatorCount", "請輸入電梯組數!!");
            }

            if (AppSettings.Default.ElevatorBoxArray == null 
                || viewModel.StorageBoxUrl == null || viewModel.StorageBoxUrl.Length != AppSettings.Default.ElevatorBoxArray.Length
                || viewModel.No == null || viewModel.No.Length != AppSettings.Default.ElevatorBoxArray.Length)
            {
                this.ModelState.AddModelError("Message", "資料錯誤!!");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ModelState = this.ModelState;
                return View("~/Views/Shared/ReportInputError.cshtml");
            }

            for (int i = 0; i < AppSettings.Default.ElevatorBoxArray.Length; i++)
            {
                var item = AppSettings.Default.ElevatorBoxArray[i];
                item.No = viewModel.No[i].GetEfficientString();
                item.StorageBoxUrl = viewModel.StorageBoxUrl[i].GetEfficientString();
            }

            AppSettings.Default.Save();

            return View("~/Views/DeviceEvents/Module/ElevatorBoxStep2.cshtml");

        }

        public ActionResult Notify(int? door)
        {
            if(door.HasValue)
            {
                UrgentEventHandler.Instance.MainDoorStatus = door.Value;
            }

            return Json(new { UrgentEventHandler.Instance.MainDoorStatus }, JsonRequestBehavior.AllowGet);
        }

    }
}