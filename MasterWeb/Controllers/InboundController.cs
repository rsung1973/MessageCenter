﻿using System;
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
using System.Net;
using System.Text;

namespace WebHome.Controllers
{
    public class InboundController : SampleController<LiveDevice>
    {
        // GET: Inbound
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

        public ActionResult Echo(EnergyQueryViewModel viewModel)
        {
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PushCommand(DeviceCommandViewModel viewModel)
        {
            if (!viewModel.CommandID.HasValue)
            {
                return Json(new { statusCode = -1, message = "CommandID not found" }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                models.GetTable<DeviceCommand>().InsertOnSubmit(new DeviceCommand
                {
                    CommandID = viewModel.CommandID.Value,
                    TokenID = viewModel.TokenID
                });
                models.SubmitChanges();

                return Json(new { statusCode = 0, message = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(new { statusCode = -1, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult PushPublicAlarm(int loopNo)
        {
            loopNo.PushPublicAlarm(models);
            return Json(new { result = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PushAlarm(UserProfileQueryViewModel viewModel)
        {
            var profile = models.GetTable<UserProfile>()
                        .Where(u => u.PID == viewModel.PID)
                        .FirstOrDefault();

            if (profile != null)
            {
                if (viewModel.Sensor.HasValue)
                {
                    UserAlarm alarm = profile.UserAlarm;
                    if (alarm == null)
                    {
                        alarm = new UserAlarm
                        {
                            AlarmID = 0,
                        };
                        profile.UserAlarm = alarm;
                    }

                    alarm.AlarmID |= (1 << (int)viewModel.Sensor.Value);
                    models.SubmitChanges();

                    Logger.Debug(alarm.JsonStringify());
                }
            }

            if (Settings.Default.CommunicationMode == (int)Naming.CommunicationMode.All || Settings.Default.CommunicationMode == (int)Naming.CommunicationMode.中保)
            {
                //var names = viewModel.PID?.Split('-');
                //String pid = names != null && names.Length > 1
                //    ? names[1]
                //    : viewModel.PID;

                //var userProfile = models.GetTable<UserProfile>()
                //            .Where(u => u.PID == pid || u.PID == viewModel.PID)
                //            .FirstOrDefault();

                if (profile != null)
                {
                    UserRegister register = profile.UserRegister;
                    if (register != null && register.DeviceUri != null)
                    {
                        LiveDevice device = register.LiveDevice.Where(d => d.DeviceID == (int)LiveDevice.SpecificDeviceType.MyTablet).FirstOrDefault();
                        if (device == null)
                        {
                            device = new LiveDevice
                            {
                                DeviceID = (int)LiveDevice.SpecificDeviceType.MyTablet,
                            };
                            register.LiveDevice.Add(device);
                            models.SubmitChanges();
                        }

                        device.ReportDeviceEvent(models, viewModel.Sensor.AWTEKSensorToSECOM(), "1");
                    }
                }

                if (AppSettings.Default.PushToLineMessageCenter)
                {
                    String url = $"{AppSettings.Default.LineMessageCenter}{Request.RawUrl}";
                    url.PushToLineMessageCenter(viewModel);
                }
            }
            else if (AppSettings.Default.PushToLineMessageCenter)
            {
                String url = $"{AppSettings.Default.LineMessageCenter}{Request.RawUrl}";
                url.PushToLineMessageCenter(viewModel);
            }
            else
            {

                String alarm = "注意！保全警報發生！請立即檢查設備！！";
                if (viewModel.Sensor.HasValue && viewModel.Io.HasValue)
                {
                    alarm = $"注意！保全第{viewModel.Io + 1}迴路{viewModel.Sensor}警報發生！請立即檢查設備！！";
                }

                if (profile == null)
                {
                    return Content("Device not found !");
                }

                profile.PushToLine(alarm, models);
            }

            return Content("OK!");
        }
    }
}