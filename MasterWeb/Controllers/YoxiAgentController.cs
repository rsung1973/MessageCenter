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
    public class YoxiAgentController : SampleController<LiveDevice>
    {
        public ActionResult CommitBuildingYoxi(YoxiAgentViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

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

            if (!AppSettings.Default.Location.Longitude.HasValue || !AppSettings.Default.Location.Latitude.HasValue)
            {
                return View("~/Views/InfoCenter/Module/LookupLocation.cshtml");
            }

            YoxiAgent agent = YoxiAgent.PrepareTaxiAgent(viewModel);
            agent.UpdateSpotAccount();

            return View("~/Views/Shared/MessageView.cshtml", model: "存檔完成!!");
        }

        // GET: Account
        public ActionResult GetAccessToken(YoxiAgentViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            YoxiAgent agent = YoxiAgent.PrepareTaxiAgent(viewModel);
            return Json(new
            {
                agent.AccessToken,
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetGeoCode(TaiwanTaxiAgentViewModel viewModel,String addr)
        {
            ViewBag.ViewModel = viewModel;
            TaiwanTaxiAgent agent = TaiwanTaxiAgent.PrepareTaxiAgent(viewModel);

            return Json(agent.GetGISGeoCode(addr), JsonRequestBehavior.AllowGet);
        }

        public ActionResult DispatchOrder(YoxiAgentViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            //if(!viewModel.Floor.HasValue)
            //{
            //    this.ModelState.AddModelError("Floor", "請選擇樓層!!");
            //}

            viewModel.ResidentID = viewModel.ResidentID.GetEfficientString();
            if (viewModel.ResidentID == null)
            {
                this.ModelState.AddModelError("ResidentID", "請輸入住戶識別碼!!");
            }

            viewModel.CustName = viewModel.CustName.GetEfficientString();
            if (viewModel.CustName == null)
            {
                this.ModelState.AddModelError("CustName", "請輸入姓名!!");
            }

            viewModel.CustPhone = viewModel.CustPhone.GetEfficientString();
            if (viewModel.CustPhone == null)
            {
                this.ModelState.AddModelError("CustPhone", "請輸入電話!!");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ModelState = this.ModelState;
                return View("~/Views/Shared/ReportInputError.cshtml");
            }

            YoxiAgent agent = YoxiAgent.PrepareTaxiAgent(viewModel);

            var result = agent.DispatchOrder();
            if (result != null)
            {
                return View("~/Views/Yoxi/Module/DispatchOrderResult.cshtml", result);
            }
            else
            {
                return Json(new { result = false, message = "系統異常..." }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DispatchCancel(YoxiAgentViewModel viewModel,String hashId)
        {
            ViewBag.ViewModel = viewModel;
            YoxiAgent agent = YoxiAgent.PrepareTaxiAgent(viewModel);

            var result = agent.DispatchCancel(hashId);
           if (result != null)
            {
                return View("~/Views/Yoxi/Module/DispatchCancelResult.cshtml", result);
            }
            else
            {
                return Json(new { result = false, message = "系統異常..." }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DeleteTrip(YoxiAgentViewModel viewModel, String hashId)
        {
            ViewBag.ViewModel = viewModel;
            YoxiAgent agent = YoxiAgent.PrepareTaxiAgent(viewModel);

            agent.DeleteTrip(hashId);
            return View("~/Views/Yoxi/Module/DispatchResult.cshtml");
        }

        public ActionResult GetDispatchOrder(TaiwanTaxiAgentViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            TaiwanTaxiAgent agent = TaiwanTaxiAgent.PrepareTaxiAgent(viewModel);

            TaiwanTaxiAgent.OrderObj order = new TaiwanTaxiAgent.OrderObj
            {
                CustPhone = "+886971234789",
                CustName = "Xiao Ming Wang",
                CustTitle = "Mr.",
                SvcType = "0",
                Amount = 1,
                BookTime = DateTime.Now,
                Address = new TaiwanTaxiAgent.AddressObj
                {
                    City = "新北市",
                    Distrcit = "三重區",
                    Road = "重新路",
                    Section = "五段",
                    Lane = null,
                    Alley = null,
                    No = "518號",
                    Lat = 24.818411M,
                    Lng = 121.331541M,
                },
                SpecOrder = new TaiwanTaxiAgent.SpecOrdObj { },
                TargetAddr = null,
                Memo = "請在銀行門口等",
                PaidType = 1,
            };

            return Content((new 
            {
                agent.AuthRequest.UserId,
                Order = order,
                agent.AccessToken,
                Signature = agent.GetServiceSignature("Order"),
            }).JsonStringify(), "application/json");
        }

        public ActionResult GetDispatchQuery(TaiwanTaxiAgentViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            TaiwanTaxiAgent agent = TaiwanTaxiAgent.PrepareTaxiAgent(viewModel);
            return Json(new
            {
                agent.AuthRequest.UserId,
                agent.AccessToken,
                Signature = agent.GetServiceSignature("Query"),
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DispatchQueryIndex(YoxiAgentViewModel viewModel)
        {
            ViewBag.ResidentOnly = true;
            ViewBag.InquiryView = "~/Views/DeviceEvents/Module/YoxiOrderQuery.cshtml";

            YoxiAgent agent = YoxiAgent.PrepareTaxiAgent(viewModel);

            ViewBag.ViewModel = viewModel;
            var items = agent.GetTripOrderList();

            return View("~/Views/DeviceEvents/Index.cshtml", items);
        }

        public ActionResult DispatchQueryAll(YoxiAgentViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            ViewBag.InquiryView = "~/Views/DeviceEvents/Module/YoxiOrderQueryAll.cshtml";

            YoxiAgent agent = YoxiAgent.PrepareTaxiAgent(viewModel);
            var items = agent.GetTripOrderList();

            return View("~/Views/DeviceEvents/Index.cshtml", items);
        }

        public ActionResult CommonSettings(TaiwanTaxiAgentViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            ViewBag.ResidentOnly = true;
            ViewBag.InquiryView = "~/Views/DeviceEvents/Module/TaxiOrderQuery.cshtml";

            TaiwanTaxiAgent agent = TaiwanTaxiAgent.PrepareTaxiAgent(viewModel);
            viewModel.AuthResponse = agent.AuthResponse;

            //return View("~/Views/DeviceEvents/Index.cshtml", agent.CommonSettings());
            return Content(JsonConvert.SerializeObject(agent.CommonSettings()), "application/json");
        }

        public ActionResult TestQueryIndex(TaiwanTaxiAgentViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            ViewBag.ResidentOnly = true;
            ViewBag.InquiryView = "~/Views/DeviceEvents/Module/TaxiOrderQuery.cshtml";

            TaiwanTaxiAgent.DispatchQueryResponse jsonData = JsonConvert.DeserializeObject<TaiwanTaxiAgent.DispatchQueryResponse>(System.IO.File.ReadAllText("G:\\temp\\data.json"));

            return View("~/Views/DeviceEvents/Index.cshtml", jsonData);
        }
    }
}