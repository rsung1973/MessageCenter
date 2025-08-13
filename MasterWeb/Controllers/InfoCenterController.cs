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
using System.Threading.Tasks;
using System.Net;
using QRCoder;
using System.Drawing;

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

        public ActionResult AppIndex(InfoQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            return View();
        }

        public ActionResult SurroundingIndex(InfoQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            return View("~/Views/InfoCenter/SurroundingIndex.cshtml");
        }



        public ActionResult ResidentIndex(InfoQueryViewModel viewModel)
        {
            ViewBag.ResidentOnly = true;
            ViewBag.ViewModel = viewModel;
            return View("Index");
        }

        public ActionResult ResetRegister(UserAccountQueryViewModel viewModel)
        {
            ViewResult result = (ViewResult)ResidentInfo(viewModel);
            UserProfile item = result.Model as UserProfile;

            if (item == null)
            {
                return Json(new { result = false, message = "資料錯誤!!" }, JsonRequestBehavior.AllowGet);
            }

            String lineID = item.UserProfileExtension.LineID;
            item.UserProfileExtension.LineID = null;
            models.DeleteAnyOnSubmit<UserBinding>(b => b.UID == item.UID && b.LineID == lineID);
            models.SubmitChanges();

            return Json(new { result = true, KeyID = lineID.EncryptKey() }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult EditResident(UserAccountQueryViewModel viewModel)
        {
            ViewResult result = (ViewResult)ResidentInfo(viewModel);
            UserProfile item = result.Model as UserProfile;

            if (item != null)
            {
                viewModel.UID = item.UID;
            }

            return View(item);
        }

        public ActionResult CommitResidentInfo(UserAccountQueryViewModel viewModel)
        {
            ViewResult result = (ViewResult)ResidentInfo(viewModel);
            UserProfile item = result.Model as UserProfile;

            if (item == null)
            {
                return Json(new { result = false, message = "資料錯誤!!" }, JsonRequestBehavior.AllowGet);
            }

            item.RealName = viewModel.RealName.GetEfficientString();
            models.SubmitChanges();

            return Json(new { result = true }, JsonRequestBehavior.AllowGet);

        }



        public ActionResult RegisterResident(InfoQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            if (viewModel.KeyID != null)
            {
                viewModel.LineID = viewModel.KeyID.DecryptKey();
            }

            var item = models.GetTable<UserProfileExtension>().Where(t => t.LineID == viewModel.LineID)
                    .Select(t => t.UserProfile)
                    .FirstOrDefault();

            if (item == null)
            {
                return View("RegisterResidentDevice");
            }

            viewModel.KeyID = viewModel.LineID.EncryptKey();
            return View("ResidentIndex", item);
        }

        public ActionResult DeleteLineBinding(UserAccountQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            if (viewModel.KeyID != null)
            {
                viewModel.UID = viewModel.DecryptKeyValue();
            }

            var item = models.GetTable<UserProfileExtension>().Where(u => u.UID == viewModel.UID).FirstOrDefault();

            if (item == null)
            {
                viewModel.InstanceID = viewModel.InstanceID.GetEfficientString();
                item = models.GetTable<UserProfileExtension>().Where(t => t.InstanceID == viewModel.InstanceID)
                            .FirstOrDefault();
            }

            if (item == null)
            {
                return Json(new { result = false, message = "資料錯誤!!" }, JsonRequestBehavior.AllowGet);
            } 

            var result = models.ExecuteCommand("delete UserBinding where UID={0} and LineID={1}",
                            viewModel.UID, viewModel.LineID);

            if (result > 0)
            {
                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { result = false, message = "資料錯誤!!" }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ResidentLogin(UserAccountQueryViewModel viewModel)
        {
            ViewResult result = (ViewResult)RegisterResident(viewModel);
            UserProfile item = result.Model as UserProfile;

            viewModel.Password = viewModel.Password.GetEfficientString();
            if (item == null || viewModel.Password == null)
            {
                this.ModelState.AddModelError("Password", "密碼錯誤!!");
            }
            else
            {
                if (item.Password!= ValueValidity.MakePassword(viewModel.Password))
                {
                    this.ModelState.AddModelError("Password", "密碼錯誤!!");
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ModelState = this.ModelState;
                return View("~/Views/Shared/ReportInputError.cshtml");
            }

            return Json(new { result = true, KeyID = item.UID.EncryptKey() }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ResidentInfo(UserAccountQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            if(viewModel.KeyID!=null)
            {
                viewModel.UID = viewModel.DecryptKeyValue();
            }

            var item = models.GetTable<UserProfile>().Where(u => u.UID == viewModel.UID).FirstOrDefault();
            if (item == null)
            {
                return View("RegisterResidentDevice");
            }

            return View("~/Views/InfoCenter/ResidentInfo2021.cshtml", item);

        }

        public ActionResult TuyaInfo(UserAccountQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            if (viewModel.KeyID != null)
            {
                viewModel.UID = viewModel.DecryptKeyValue();
            }

            var item = models.GetTable<UserProfile>().Where(u => u.UID == viewModel.UID).FirstOrDefault();
            if (item == null)
            {
                item = models.GetTable<UserProfile>().Where(u => u.PID == viewModel.InstanceID).FirstOrDefault();
            }

            if (item == null)
            {
                return View("RegisterResidentDevice");
            }

            return View("~/Views/InfoCenter/ResidentInfo2021.cshtml", item);

        }

        public ActionResult BindingInfo(UserAccountQueryViewModel viewModel)
        {
            ViewBag.ViewMoel = viewModel;
            if (viewModel.KeyID != null)
            {
                viewModel.UID = viewModel.DecryptKeyValue();
            }

            var item = models.GetTable<UserBinding>().Where(u => u.UID == viewModel.UID && u.LineID==viewModel.LineID).FirstOrDefault();
            if (item == null)
            {
                return View("RegisterResidentDevice");
            }

            return View("BindingInfo", item);

        }



        UserBinding createBinding(UserProfile profile, UserAccountQueryViewModel viewModel)
        {
            var binding = profile.UserBinding.Where(b => b.LineID == viewModel.LineID).FirstOrDefault();
            if (binding == null)
            {
                binding = new UserBinding
                {
                    UID = profile.UID,
                    LineID = viewModel.LineID,
                    LineUser = viewModel.PID,
                };
                models.GetTable<UserBinding>().InsertOnSubmit(binding);
            }
            return binding;
        }

        public ActionResult CommitResident(UserAccountQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            if (viewModel.KeyID != null)
            {
                viewModel.LineID = viewModel.KeyID.DecryptKey();
            }

            viewModel.InstanceID = viewModel.InstanceID.GetEfficientString();
            var item = models.GetTable<UserProfileExtension>().Where(t => t.InstanceID == viewModel.InstanceID)
                        .FirstOrDefault();

            if (item == null)
            {
                this.ModelState.AddModelError("InstanceID", "設備識別碼錯誤!!");
                ViewBag.ModelState = this.ModelState;
                return View("~/Views/Shared/ReportInputError.cshtml");
            }

            if (String.IsNullOrEmpty(viewModel.Password))
            {
                ModelState.AddModelError("PassWord", "請輸入密碼!!");
            }
            else
            {
                if (viewModel.Password != viewModel.Password2)
                {
                    //檢查密碼
                    ModelState.AddModelError("PassWord2", "二組密碼輸入不同!!");
                }
            }

            viewModel.PID = viewModel.PID.GetEfficientString();
            if (viewModel.PID == null)
            {
                ModelState.AddModelError("PID", "請輸入住戶名稱!!");
            }
            else if (viewModel.PID != item.UserProfile.PID)
            {
                if (item.LineID == null)
                {
                    ModelState.AddModelError("PID", "住戶名稱與設備不相符");
                }

                //var binding = createBinding(item.UserProfile, viewModel);
                //if (binding != null)
                //{
                //    models.SubmitChanges();
                //    return View("LineInfo", binding);
                //}
                //else
                //{
                //    this.ModelState.AddModelError("InstanceID", "您已是此設備警報接收成員!!");
                //    ViewBag.ModelState = this.ModelState;
                //    return View("~/Views/Shared/ReportInputError.cshtml");
                //}
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ModelState = this.ModelState;
                return View("~/Views/Shared/ReportInputError.cshtml");
            }

            bool registerAdmin = item.LineID == null || item.UserProfile.PID == viewModel.PID;
            if (registerAdmin)
            {
                item.LineID = viewModel.LineID;
                item.UserProfile.Password = ValueValidity.MakePassword(viewModel.Password);
            }
            var binding = createBinding(item.UserProfile, viewModel);
            models.SubmitChanges();

            if (registerAdmin)
            {
                return Json(new { result = true, KeyID = item.UID.EncryptKey() }, JsonRequestBehavior.AllowGet);
            }
            //else if (binding == null)
            //{
            //    return Json(new { result = false, message = "您已是此設備警報接收成員!!" }, JsonRequestBehavior.AllowGet);
            //}
            else
            {
                return View("ToBindingInfo", binding);
            }

        }

        public ActionResult CommitResidentDevice(UserAccountQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            if (viewModel.KeyID != null)
            {
                viewModel.LineID = viewModel.KeyID.DecryptKey();
            }

            viewModel.InstanceID = viewModel.InstanceID.GetEfficientString();
            var item = models.GetTable<UserProfileExtension>().Where(t => t.InstanceID == viewModel.InstanceID)
                        .FirstOrDefault();

            if (item == null)
            {
                this.ModelState.AddModelError("InstanceID", "設備識別碼錯誤!!");
                ViewBag.ModelState = this.ModelState;
                return View("~/Views/Shared/ReportInputError.cshtml");
            }

            if (String.IsNullOrEmpty(viewModel.Password))
            {
                ModelState.AddModelError("PassWord", "請輸入密碼!!");
            }
            else
            {
                if (viewModel.Password != viewModel.Password2)
                {
                    //檢查密碼
                    ModelState.AddModelError("PassWord2", "二組密碼輸入不同!!");
                }
            }

            viewModel.PID = viewModel.PID.GetEfficientString();
            if (viewModel.PID == null)
            {
                ModelState.AddModelError("PID", "請輸入APP號碼!!");
            }
            else if (viewModel.PID != item.UserProfile.PID)
            {
                if (item.LineID == null)
                {
                    ModelState.AddModelError("PID", "首次註冊時APP號碼需同住戶名稱!!");
                }

            }
            else if (viewModel.PID == item.UserProfile.PID)
            {
                if (item.LineID != null)
                {
                    ModelState.AddModelError("PID", "設備已註冊，請輸入其他APP號碼!!");
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ModelState = this.ModelState;
                return View("~/Views/Shared/ReportInputError.cshtml");
            }

            bool registerAdmin = item.LineID == null || item.UserProfile.PID == viewModel.PID;
            if (registerAdmin)
            {
                item.LineID = viewModel.LineID;
                item.UserProfile.Password = ValueValidity.MakePassword(viewModel.Password);
            }
            var binding = createBinding(item.UserProfile, viewModel);
            models.SubmitChanges();

            if (registerAdmin)
            {
                return Json(new { result = true, KeyID = item.UID.EncryptKey() }, JsonRequestBehavior.AllowGet);
            }
            //else if (binding == null)
            //{
            //    return Json(new { result = false, message = "您已是此設備警報接收成員!!" }, JsonRequestBehavior.AllowGet);
            //}
            else
            {
                return View("ToBindingInfo", binding);
            }

        }

        [HttpPost]
        public ActionResult RegisterResidentDevice(UserAccountQueryViewModel viewModel)
        {
            if (viewModel.KeyID != null)
            {
                viewModel.LineID = viewModel.KeyID.DecryptKey();
            }

            viewModel.InstanceID = viewModel.InstanceID.GetEfficientString();
            var item = models.GetTable<UserProfileExtension>().Where(t => t.InstanceID == viewModel.InstanceID)
                        .FirstOrDefault();

            if (item == null)
            {
                return Json(new { result = false, message = "設備識別碼錯誤!!" }, JsonRequestBehavior.AllowGet);
            }

            //if (String.IsNullOrEmpty(viewModel.Password))
            //{
            //    ModelState.AddModelError("PassWord", "請輸入密碼!!");
            //}
            //else
            //{
            //    if (viewModel.Password != viewModel.Password2)
            //    {
            //        ModelState.AddModelError("PassWord2", "二組密碼輸入不同!!");
            //    }
            //}

            viewModel.PID = viewModel.PID.GetEfficientString();
            if (viewModel.PID == null)
            {
                ModelState.AddModelError("PID", "請輸入APP號碼!!");
            }
            else if (viewModel.PID != item.UserProfile.PID)
            {
                if (item.LineID == null)
                {
                    ModelState.AddModelError("PID", "首次註冊時APP號碼需同住戶名稱!!");
                }
            }
            else if (viewModel.PID == item.UserProfile.PID)
            {
                if (item.LineID != null)
                {
                    ModelState.AddModelError("PID", "設備已註冊，請輸入其他APP號碼!!");
                }
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new { key = x.Key, errors = x.Value.Errors.Select(e => e.ErrorMessage).ToArray() })
                    .ToArray();
                return Json(new { result = false, errors }, JsonRequestBehavior.AllowGet);
            }

            bool registerAdmin = item.LineID == null || item.UserProfile.PID == viewModel.PID;
            if (registerAdmin)
            {
                item.LineID = viewModel.LineID;
                //item.UserProfile.Password = ValueValidity.MakePassword(viewModel.Password);
            }
            var binding = createBinding(item.UserProfile, viewModel);
            models.SubmitChanges();

            if (registerAdmin)
            {
                return Json(new { result = true, KeyID = item.UID.EncryptKey() }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    result = true,
                    binding = new
                    {
                        binding.BindingID,
                        binding.UID,
                        binding.LineID
                    }
                }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult InquireResidentMessage(InfoQueryViewModel viewModel)
        {
            var result = MessageOutbound.Instance.GetResidentMessage(viewModel.ResidentID);
            if (result != null)
            {
                return View("~/Views/Outbound/BuildingInfo/QueryResult.cshtml", result);
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

        public ActionResult BuildingLocationIndex(TaxiOrderViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            return View();
        }

        public ActionResult BuildingYoxiIndex(YoxiAgentViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            return View();
        }

        public ActionResult ResidentEnergyIndex(EnergyQueryViewModel viewModel)
        {
            ViewBag.ResidentOnly = true;
            ViewBag.ViewModel = viewModel;
            return View("~/Views/InfoCenter/EnergyIndex.cshtml");
        }

        public ActionResult PowerMeterIndex(InfoQueryViewModel viewModel,bool? residentOnly = true)
        {
            ViewBag.ViewModel = viewModel;
            ViewBag.ResidentOnly = residentOnly;

            if (viewModel.CustomQuery == "PowerMeterIndex")
            {
                return View("~/Views/InfoCenter/Module/PowerMeterReader.cshtml");
            }
            else
            {

                ViewBag.InquiryView = "~/Views/InfoCenter/Module/PowerMeterQuery.cshtml";
                return View("~/Views/DeviceEvents/Index.cshtml");
            }
        }

        public ActionResult ReportEnergyDegree(EnergyQueryViewModel viewModel,int actionType)
        {
            var result = MessageOutbound.Instance.ReportEnergyDegree(viewModel,actionType);
            if (result != null)
            {
                return View("~/Views/Shared/MessageView.cshtml", model: JsonConvert.SerializeObject(result));
            }
            else
            {
                return View("~/Views/Shared/MessageView.cshtml", model: "資料錯誤!!");
            }
        }

        public ActionResult ReportEnergyUsage(EnergyQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            viewModel.ResidentID = viewModel.ResidentID.GetEfficientString();
            var user = models.GetTable<UserProfile>().Where(t => t.PID == viewModel.ResidentID).FirstOrDefault();
            if (user == null)
            {
                return View("~/Views/Shared/MessageView.cshtml", model: "住戶資料錯誤!!");
            }

            if (!viewModel.Degree.HasValue)
            {
                return View("~/Views/Shared/MessageView.cshtml", model: "請填瓦斯度數!!");
            }

            GasUsageReport usage = null;
            if (viewModel.Update == true)
            {
                usage = models.GetTable<GasUsageReport>()
                    .Where(r => r.UID == user.UID)
                    .Where(r => r.Year == viewModel.Year)
                    .Where(r => r.Month == viewModel.Month)
                    .OrderByDescending(r => r.ReportID)
                    .FirstOrDefault();
            }

            if (usage == null)
            {
                usage = new GasUsageReport
                {
                    UID = user.UID,
                    Year = viewModel.Year ?? DateTime.Today.Year,
                    Month = viewModel.Month ?? DateTime.Today.Month,
                };

                models.GetTable<GasUsageReport>().InsertOnSubmit(usage);
            }

            usage.ReportDate = DateTime.Now;
            usage.Usage = viewModel.Degree ?? 0;

            models.SubmitChanges();

            return View("~/Views/Shared/MessageView.cshtml", model: "存檔完成!!");
        }

        public ActionResult CommitBuildingLocation(TaxiOrderViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            if (!viewModel.Floor.HasValue || viewModel.Floor < 1)
            {
                ModelState.AddModelError("Floor", "請輸入樓層!!");
            }

            if (viewModel.UnitNo < 1)
            {
                ModelState.AddModelError("UnitNo", "之號錯誤!!");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ModelState = this.ModelState;
                return View("~/Views/Shared/ReportInputError.cshtml");
            }

            AppSettings.Default.Location.City = viewModel.City.GetEfficientString();
            AppSettings.Default.Location.District = viewModel.District.GetEfficientString();
            AppSettings.Default.Location.Road = viewModel.Road.GetEfficientString();
            AppSettings.Default.Location.Section = viewModel.Section.GetEfficientString();
            AppSettings.Default.Location.Lane = viewModel.Lane.GetEfficientString();
            AppSettings.Default.Location.Alley = viewModel.Alley.GetEfficientString();
            AppSettings.Default.Location.No = viewModel.No.GetEfficientString();
            AppSettings.Default.Location.Latitude = viewModel.Latitude;
            AppSettings.Default.Location.Longitude = viewModel.Longitude;
            AppSettings.Default.Location.Memo = viewModel.Memo.GetEfficientString();
            AppSettings.Default.Location.FloorCount = viewModel.Floor;
            AppSettings.Default.Location.UnitCount = viewModel.UnitNo;

            AppSettings.Default.Save();

            if(!AppSettings.Default.Location.Longitude.HasValue || !AppSettings.Default.Location.Latitude.HasValue)
            {
                return View("~/Views/InfoCenter/Module/LookupLocation.cshtml");
            }

            return View("~/Views/Shared/MessageView.cshtml", model: "存檔完成!!");
        }

        public ActionResult InquireEnergyUsage(EnergyQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            IQueryable<UserProfile> users = models.GetTable<UserProfile>();

            viewModel.ResidentID = viewModel.ResidentID.GetEfficientString();
            if (viewModel.ResidentID != null)
            {
                users = users.Where(u => u.PID == viewModel.ResidentID);
            }
            var items = users.Join(models.GetTable<GasUsageReport>().Where(r => r.Year == viewModel.Year && r.Month == viewModel.Month),
                                u => u.UID, r => r.UID, (u, r) => new
                                {
                                    住戶識別碼 = u.PID,
                                    年度 = r.Year,
                                    月份 = r.Month,
                                    度數 = r.Usage,
                                    登記日期 = $"{r.ReportDate:yyyy/MM/dd HH:mm:ss}",
                                }).OrderBy(r => r.住戶識別碼);

            return View("~/Views/DataQuery/QueryResult.cshtml", items);
        }

        public ActionResult InquireEnergyDegree(EnergyQueryViewModel viewModel)
        {
            var result = MessageOutbound.Instance.ReportEnergyDegree(viewModel, 3);
            if (result != null)
            {
                return View("~/Views/Outbound/BuildingInfo/QueryResult.cshtml", result);
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
                if (profile != null)
                {
                    if (!profile.UserFCM.Any(t => t.FCMToken == fcmToken))
                    {
                        profile.UserFCM.Add(new UserFCM
                        {
                            FCMToken = fcmToken
                        });
                        models.SubmitChanges();
                    }
                    return Json(new { result = true });
                }
            }
            return Json(new { result = false });
        }

        public ActionResult PlayAlarm()
        {
            Response.Headers.Add("Feature-Policy", "autoplay 'self'");
            return File("~/media/alarm.wav", "audio/wav");
        }
        public ActionResult UrgencyIndex(InfoQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            if (viewModel.CheckConnection == true)
            {
                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            }

            if(viewModel.ShowEvent.HasValue)
            {
                ViewBag.ResidentOnly = true;
                return View("~/Views/InfoCenter/UrgencyIndex.cshtml");
            }

            if (UrgentEventHandler.Instance.CurrentFire)
                return Json(new { result = true, eventCode = (int)DeviceTransactionViewModel.UrgentEventDefinition.火災 }, JsonRequestBehavior.AllowGet);
            else if (UrgentEventHandler.Instance.CurrentEarthquake)
                return Json(new { result = true, eventCode = (int)DeviceTransactionViewModel.UrgentEventDefinition.地震 }, JsonRequestBehavior.AllowGet);

            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RegisterDevice(InfoQueryViewModel viewModel,String suffix)
        {
            ViewBag.ViewModel = viewModel;

            viewModel.InstanceID = viewModel.InstanceID.GetEfficientString();
            if (viewModel.InstanceID == null)
            {
                return Json(new { result = false, message = "設備識別碼錯誤" }, JsonRequestBehavior.AllowGet);
            }

            viewModel.ResidentID = viewModel.ResidentID.GetEfficientString();
            if (viewModel.ResidentID == null)
            {
                return Json(new { result = false, message = "住戶代號錯誤" }, JsonRequestBehavior.AllowGet);
            }

            var segments = viewModel.ResidentID.Split('-');
            if (segments.Length < 2)
            {
                return Json(new { result = false, message = "住戶代號未指定社區代碼" }, JsonRequestBehavior.AllowGet);
            }

            var community = models.GetTable<Community>().Where(c => c.CommunityNo == segments[0]).FirstOrDefault();
            if (community == null)
            {
                return Json(new { result = false, message = "社區代碼錯誤" }, JsonRequestBehavior.AllowGet);
            }

            var device = models.GetTable<UserProfileExtension>().Where(t => t.InstanceID == viewModel.InstanceID).FirstOrDefault();
            var user = models.GetTable<UserProfile>().Where(t => t.PID == viewModel.ResidentID).FirstOrDefault();

            if (device == null)
            {
                if (user != null)
                {
                    if (user.UserProfileExtension == null)
                    {
                        user.UserProfileExtension = new UserProfileExtension
                        {
                            InstanceID = viewModel.InstanceID,
                            CommunityID = community.CommunityID,
                        };
                    }
                    else
                    {
                        return Json(new { result = false, message = "住戶代號重複" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    user = new UserProfile
                    {
                        PID = viewModel.ResidentID,
                        UserName = viewModel.ResidentID,
                        UserRegister = new UserRegister
                        {

                        },
                        UserProfileExtension = new UserProfileExtension
                        {
                            InstanceID = viewModel.InstanceID,
                            CommunityID = community.CommunityID,
                        },
                    };
                    models.GetTable<UserProfile>().InsertOnSubmit(user);
                }
                device = user.UserProfileExtension;
            }
            else
            {
                user = device.UserProfile;
                user.PID = viewModel.ResidentID;
            }

            models.SubmitChanges();

            if (AppSettings.Default.RegisterResidentToSECOM)
            {
                if (!AppSettings.Default.BuildingID.HasValue || !AppSettings.Default.FloorID.HasValue)
                {
                    MessageOutbound.Instance.ApplyDefaultBuildingInfo();
                }

                if (user.UserRegister != null && user.UserRegister.DeviceUri == null
                    && AppSettings.Default.BuildingID.HasValue
                    && AppSettings.Default.FloorID.HasValue)
                {
                    models.Insert_BA_Device(AppSettings.Default.BuildingID, AppSettings.Default.FloorID, user, user.UserRegister, suffix ?? "LINE");
                }
            }

            if(AppSettings.Default.PushToLineMessageCenter)
            {
                String url = $"{AppSettings.Default.LineMessageCenter}{Request.RawUrl}";
                url.PushToLineMessageCenter(viewModel);
            }

            return Json(new { result = true }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Check(InfoQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            return Json(new { result = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UrgentCall(bool? reset)
        {
            if (reset == true)
            {
                UrgentEventHandler.Instance.Reset();
                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if(Request.InputStream.CanSeek)
                {
                    Request.InputStream.Seek(0, SeekOrigin.Begin);
                }
                using (StreamReader reader = new StreamReader(Request.InputStream, Request.ContentEncoding))
                {
                    UrgentEventHandler.Instance.PushMessage(reader.ReadToEnd());
                }
            }
            return new EmptyResult { };
        }

        public ActionResult CheckVocalCommand(InfoQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            var item = models.GetTable<DeviceCommand>()
                        .Where(d => d.TokenID == viewModel.InstanceID)
                        .OrderBy(d => d.LogID).FirstOrDefault();

            if (item == null)
                return new EmptyResult { };

            models.ExecuteCommand("delete DeviceCommand where LogID = {0} ", item.LogID);

            return Json(new { item.CommandID }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckDefence(InfoQueryViewModel viewModel)
        {
            // 將 viewModel 放入 ViewBag 以便於 View 使用
            ViewBag.ViewModel = viewModel;

            // 輸出目前時間與 viewModel 內容到 Console 以便除錯
            Console.WriteLine($"{DateTime.Now},{viewModel.JsonStringify()}");

            // 若設定為推送到 Line Message Center，則直接推送並回傳結果
            if (AppSettings.Default.PushToLineMessageCenter)
            {
                String url = $"{AppSettings.Default.LineMessageCenter}{Request.RawUrl}";
                String data = url.PushToLineMessageCenter(viewModel, true);
                return Content(data, "application/json");
            }

            // 若有傳入 DefenceStatus 且 UrgentEventHandler 實例存在，則更新裝置狀態
            if (viewModel.DefenceStatus.HasValue && UrgentEventHandler.Instance != null)
            {
                // 取得或建立對應 InstanceID 的 DeviceStatus
                var statusItem = UrgentEventHandler.Instance.DeviceStatusList
                        .Where(d => d != null && d.InstanceID == viewModel.InstanceID)
                        .FirstOrDefault();
                if (statusItem == null)
                {
                    statusItem = new DeviceStatus
                    {
                        InstanceID = viewModel.InstanceID,
                    };
                    UrgentEventHandler.Instance.DeviceStatusList.Add(statusItem);
                    Logger.Debug($"CheckDefence DeviceStatus Item: {viewModel.JsonStringify()}");
                }

                // 設定防禦狀態
                statusItem.DefenceStatus = viewModel.DefenceStatus;

                // 若狀態為清除，則將對應 UserAlarm 的 AlarmID 設為 0
                if (viewModel.DefenceStatus == Naming.DefenceStatus.Clear)
                {
                    models.ExecuteCommand(@"UPDATE  UserAlarm
                                            SET        AlarmID = 0
                                            FROM     UserAlarm INNER JOIN
                                                         UserProfile ON UserAlarm.UID = UserProfile.UID
                                            WHERE   (UserProfile.PID = {0})", viewModel.InstanceID);
                    // 若需刪除 UserAlarm，可啟用下列程式碼
                    //models.ExecuteCommand(@"DELETE FROM UserAlarm
                    //                    FROM     UserProfile INNER JOIN
                    //                                 UserAlarm ON UserProfile.UID = UserAlarm.UID
                    //                    WHERE   (UserProfile.PID = {0})", viewModel.InstanceID);
                }
            }

            bool hasResult = false;
            // 建立回傳的結果物件
            DeviceTransactionViewModel result = new DeviceTransactionViewModel
            {

            };

            // 若為本地訊息中心，檢查主門狀態
            if (AppSettings.Default.LocalMessageCenter)
            {
                if (UrgentEventHandler.Instance.MainDoorStatus.HasValue)
                {
                    result.MainDoor = (DeviceTransactionViewModel.DoorStatus?)UrgentEventHandler.Instance.MainDoorStatus;
                    hasResult = true;
                }
            }

            // 查詢 MessageBoard 是否有對應 InstanceID 的訊息
            var item = models.GetTable<MessageBoard>()
                        .Where(d => d.InstanceID == viewModel.InstanceID)
                        .FirstOrDefault();

            if (item != null)
            {
                hasResult = true;
                result.Defence = (DeviceTransactionViewModel.DefenceStatus?)item.Defence;
                // 取得後刪除該訊息
                models.ExecuteCommand("delete MessageBoard where InstanceID = {0} ", item.InstanceID);
            }

            // 若為本地訊息中心，檢查緊急事件（火災、地震、清除）
            if (AppSettings.Default.LocalMessageCenter)
            {
                if (UrgentEventHandler.Instance.CurrentFire)
                {
                    result.EventCode = DeviceTransactionViewModel.UrgentEventDefinition.火災;
                    if (AppSettings.Default.CheckPublicAlarmSettings)
                    {
                        var items = models.GetTable<UserProfile>()
                            .Where(u => u.PID == viewModel.InstanceID);

                        if (UrgentEventHandler.Instance.AlarmLoop.HasValue)
                        {
                            items = items.Where(u => u.SubscribedAlarm == (int)Naming.AlarmSubscription.公共設施
                                || u.SubscribedAlarm == UrgentEventHandler.Instance.AlarmLoop);
                        }
                        else
                        {
                            items = items.Where(u => u.SubscribedAlarm.HasValue);
                        }
                        hasResult = items.Any();
                    }
                    else
                    {
                        hasResult = true;
                    }
                }
                else if (UrgentEventHandler.Instance.CurrentEarthquake)
                {
                    result.EventCode = DeviceTransactionViewModel.UrgentEventDefinition.地震;
                    if (AppSettings.Default.CheckPublicAlarmSettings)
                    {
                        hasResult = models.GetTable<UserProfile>()
                            .Where(u => u.PID == viewModel.InstanceID)
                            .Where(u => u.SubscribedAlarm.HasValue)
                            .Any();
                    }
                    else
                    {
                        hasResult = true;
                    }
                }
                else if (UrgentEventHandler.Instance.Clear)
                {
                    result.EventCode = DeviceTransactionViewModel.UrgentEventDefinition.Clear;
                    hasResult = true;
                }
            }

            // 若有結果，回傳 JSON 格式內容，否則回傳空結果
            if (hasResult)
            {
                Logger.Debug($"CheckDefence:{viewModel.JsonStringify()}\r\n{result.JsonStringify()}");
                return Content(result.JsonStringify(), "application/json");
            }
            else
            {
                return new EmptyResult { };
            }
        }

        public ActionResult GetDeviceStatus(InfoQueryViewModel viewModel)
        {
            viewModel.ResidentID = viewModel.ResidentID.GetEfficientString();
            if (viewModel.ResidentID != null)
            {
                var statusItem = UrgentEventHandler.Instance.DeviceStatusList
                    .Where(d => d != null && d.InstanceID == viewModel.InstanceID).FirstOrDefault();
                return Content($"{(int?)statusItem?.DefenceStatus ?? -1}");
            }
            else
            {
                return Json(UrgentEventHandler.Instance.DeviceStatusList, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ResetDeviceStatus()
        {
            UrgentEventHandler.Instance.DeviceStatusList.Clear();
            return Json(UrgentEventHandler.Instance.DeviceStatusList, JsonRequestBehavior.AllowGet);
        }


        public ActionResult SendToFCM(UserProfileQueryViewModel viewModel)
        {
            IQueryable<UserProfile> items = models.GetTable<UserProfile>();
            bool hasQuery = false;

            viewModel.PID = viewModel.PID.GetEfficientString();
            if (viewModel.PID != null)
            {
                hasQuery = true;
                items = items.Where(u => u.PID == viewModel.PID);
            }
            viewModel.Phone = viewModel.Phone.GetEfficientString();
            if (viewModel.Phone != null)
            {
                hasQuery = true;
                items = items.Where(u => u.Phone == viewModel.Phone);
            }
            viewModel.RealName = viewModel.RealName.GetEfficientString();
            if (viewModel.RealName != null)
            {
                hasQuery = true;
                items = items.Where(u => u.RealName == viewModel.RealName);
            }
            viewModel.ThemeName = viewModel.ThemeName.GetEfficientString();
            if (viewModel.ThemeName != null)
            {
                hasQuery = true;
                items = items.Where(u => u.ThemeName == viewModel.ThemeName);
            }
            viewModel.UserName = viewModel.UserName.GetEfficientString();
            if (viewModel.UserName != null)
            {
                hasQuery = true;
                items = items.Where(u => u.UserName == viewModel.UserName);
            }

            if (viewModel.UID.HasValue)
            {
                hasQuery = true;
                items = items.Where(u => u.UID == viewModel.UID);
            }

            if (hasQuery)
            {
                viewModel.Caller = viewModel.Caller.GetEfficientString();
                if (viewModel.Caller != null)
                {
                    UserProfile profile = items.FirstOrDefault();
                    if (profile != null)
                    {
                        profile.PushCallAlarm(viewModel.Caller, models);
                    }
                }

                var fcmItems = models.GetTable<UserFCM>()
                    .Join(items, f => f.UID, u => u.UID, (f, u) => f);

                fcmItems.SendToFCM(models);

            }

            return Content("OK!");
        }

        public ActionResult SetDefence(DefenceQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            if(viewModel.KeyID!=null)
            {
                viewModel.UID = viewModel.DecryptKeyValue();
            }

            var item = models.GetTable<UserProfileExtension>().Where(p => p.UID == viewModel.UID).FirstOrDefault();

            if (item == null)
            {
                viewModel.InstanceID = viewModel.InstanceID.GetEfficientString();
                item = models.GetTable<UserProfileExtension>().Where(t => t.InstanceID == viewModel.InstanceID)
                            .FirstOrDefault();
            }

            if (item == null || item.InstanceID == null)
            {
                return Json(new { result = false, message = "設備識別碼錯誤" }, JsonRequestBehavior.AllowGet);
            }

            var messageItem = models.GetTable<MessageBoard>().Where(m => m.InstanceID == item.InstanceID).FirstOrDefault();
            if(messageItem==null)
            {
                messageItem = new MessageBoard
                {
                    InstanceID = item.InstanceID,
                    MessageDate = DateTime.Now,
                };
                models.GetTable<MessageBoard>().InsertOnSubmit(messageItem);
            }

            messageItem.Defence = (int?)viewModel.Defence;

            models.SubmitChanges();

            return Json(new { result = true }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult PushFCMToken(String token)
        {
            token = token.GetEfficientString();
            FCMToken item = null;
            if (token != null)
            {
                item = models.GetTable<FCMToken>().Where(t => t.Token == token).FirstOrDefault();
                if (item == null)
                {
                    item = new FCMToken
                    {
                        Token = token,
                    };
                    models.GetTable<FCMToken>().InsertOnSubmit(item);
                    models.SubmitChanges();
                }
            }

            return Json(new { result = item != null }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult UserGuide(UserGuideQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            if (!viewModel.CurrentIndex.HasValue)
            {
                viewModel.CurrentIndex = 0;
            }

            return View("~/Views/InfoCenter/UserGuide/InfoPage.cshtml");
        }

        public ActionResult Bulletin(UserGuideQueryViewModel viewModel)
        {
            viewModel.ResourceName = "Bulletin";
            return UserGuide(viewModel);
        }


        public ActionResult UserGuidePage(UserGuideQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;


            viewModel.UserGuide = viewModel.UserGuide.GetEfficientString();
            if (viewModel.UserGuide == null)
            {
                return new EmptyResult { };
            }

            String userGuideUrl = $"~/{viewModel.ResourceName}/{viewModel.UserGuide}";
            var items = Directory.EnumerateFiles(Server.MapPath(userGuideUrl));

            var item = items.Skip(Math.Min(items.Count() - 1, viewModel.CurrentIndex ?? 0))
                .FirstOrDefault();

            if (item == null)
            {
                return new EmptyResult { };
            }

            return Redirect(item.Replace(Request.PhysicalApplicationPath, "~/").Replace("\\", "/"));

            //return File(item, "text/html");
        }

        public ActionResult SystemInfo()
        {
            return Content((new 
            {
                AppSettings.Default,
            }).JsonStringify());
        }

        public ActionResult ReloadSettings()
        {
            AppSettings.Reload();
            return Json(AppSettings.Default, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PushToLine(UserProfileQueryViewModel viewModel)
        {
            Request.SaveAs(Path.Combine(Logger.LogDailyPath, $"{DateTime.Now.Ticks}.txt"), true);

            if (AppSettings.Default.PushToLineMessageCenter)
            {
                String url = $"{AppSettings.Default.LineMessageCenter}{Request.RawUrl}";
                url.PushToLineMessageCenter(viewModel);
            }
            else
            {
                var item = models.GetTable<UserProfile>()
                            .Where(u => u.PID == viewModel.PID)
                            .FirstOrDefault();

                if (item == null)
                {
                    return Json(new { result = false, message = "Device not found !" }, JsonRequestBehavior.AllowGet);
                }

                viewModel.Message = viewModel.Message.GetEfficientString();
                if (viewModel.Message != null)
                {
                    item.PushToLine(viewModel.Message, models);
                }
            }

            return Json(new { result = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetQRCode(String content)
        {
            // 建立 QRCodeGenerator 物件
            QRCodeGenerator qrGenerator = new QRCodeGenerator();

            // 建立 QRCodeData 物件，將字串設為內容
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(HttpUtility.UrlEncode(content), QRCodeGenerator.ECCLevel.Q);

            // 建立 QRCode 物件，設定大小及內容
            using (QRCode qrCode = new QRCode(qrCodeData))
            {
                using (Bitmap qrCodeImage = qrCode.GetGraphic(20))
                {
                    Response.Clear();
                    Response.ContentType = "image/png";
                    // 將 QR Code 輸出成檔案
                    qrCodeImage.Save(Response.OutputStream, System.Drawing.Imaging.ImageFormat.Png);
                }
            }
            return new EmptyResult { };
        }
    }
}