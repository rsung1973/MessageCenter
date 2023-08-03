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
    public class TaiwanTaxiAgentController : SampleController<LiveDevice>
    {
        // GET: Account
        public ActionResult GetAccessToken(TaiwanTaxiAgentViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            TaiwanTaxiAgent agent = TaiwanTaxiAgent.PrepareTaxiAgent(viewModel);
            return Json(new
            {
                agent.AccessToken,
                Signature = agent.GetServiceSignature("Order"),
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetGeoCode(TaiwanTaxiAgentViewModel viewModel,String addr)
        {
            ViewBag.ViewModel = viewModel;
            TaiwanTaxiAgent agent = TaiwanTaxiAgent.PrepareTaxiAgent(viewModel);

            return Json(agent.GetGISGeoCode(addr), JsonRequestBehavior.AllowGet);
        }

        public ActionResult DispatchOrder(TaxiOrderViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            if(!viewModel.Floor.HasValue)
            {
                this.ModelState.AddModelError("Floor", "請選擇樓層!!");
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

            TaiwanTaxiAgent agent = TaiwanTaxiAgent.PrepareTaxiAgent(viewModel);
            agent.LogData(viewModel, "TaxiOrderViewModel.json");

            TaiwanTaxiAgent.OrderObj order = new TaiwanTaxiAgent.OrderObj
            {
                CustPhone = viewModel.CustPhone,
                CustName = viewModel.CustName,
                CustTitle = viewModel.CustTitle,
                SvcType = "0",
                Amount = viewModel.Amount,
                BookTime = DateTime.Now /*(viewModel.BookDate ?? DateTime.Today).AddHours(viewModel.Hour??0).AddMinutes(viewModel.Minute??0)*/,
                Address = new TaiwanTaxiAgent.AddressObj
                {
                    City = viewModel.City.GetEfficientString(),
                    Distrcit = viewModel.District.GetEfficientString(),
                    Road = viewModel.Road.GetEfficientString(),
                    Section = viewModel.Section.GetEfficientString("段"),
                    Lane = viewModel.Lane.GetEfficientString("巷"),
                    Alley = viewModel.Alley.GetEfficientString("弄"),
                    No = $"{(viewModel.No.GetEfficientString() ?? "0")}號",
                    Lat = viewModel.Latitude,
                    Lng = viewModel.Longitude,
                    //Lat = 25.00872M,
                    //Lng = 121.52066M,
                },
                SpecOrder = new TaiwanTaxiAgent.SpecOrdObj { },
                TargetAddr = null,
                Memo = viewModel.Memo.GetEfficientString(),
                PaidType = viewModel.PaidType,
            };

            var result = agent.DispatchOrder(order);
            viewModel.AuthResponse = agent.AuthResponse;
            if (result != null)
            {
                return View("~/Views/TaiwanTaxi/Module/DispatchOrderResult.cshtml", result);
            }
            else
            {
                return Json(new { result = false, message = "系統異常..." }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DispatchCancel(TaxiOrderViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            TaiwanTaxiAgent agent = TaiwanTaxiAgent.PrepareTaxiAgent(viewModel);

            var result = agent.DispatchCancel(viewModel.JobID);
            viewModel.AuthResponse = agent.AuthResponse;

            if (result != null)
            {
                return View("~/Views/TaiwanTaxi/Module/DispatchCancelResult.cshtml", result);
            }
            else
            {
                return Json(new { result = false, message = "系統異常..." }, JsonRequestBehavior.AllowGet);
            }
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

        public ActionResult DispatchQueryIndex(TaxiOrderViewModel viewModel)
        {
            ViewBag.ResidentOnly = true;
            ViewBag.InquiryView = "~/Views/DeviceEvents/Module/TaxiOrderQuery.cshtml";

            TaiwanTaxiAgent agent = TaiwanTaxiAgent.PrepareTaxiAgent(viewModel);
            var lastOrder = agent.LoadData<TaxiOrderViewModel>("TaxiOrderViewModel.json");

            if (lastOrder != null)
            {
                viewModel = lastOrder;
            }

            ViewBag.ViewModel = viewModel;
            var items = agent.DispatchQuery();
            viewModel.AuthResponse = agent.AuthResponse;

            return View("~/Views/DeviceEvents/Index.cshtml", items);
        }

        public ActionResult DispatchQueryAll(TaxiOrderViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            ViewBag.InquiryView = "~/Views/DeviceEvents/Module/TaxiOrderQueryAll.cshtml";

            var items = TaiwanTaxiAgent.GetOrderPathList();

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