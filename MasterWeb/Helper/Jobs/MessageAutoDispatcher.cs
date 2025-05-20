using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using CommonLib.Helper;
using Utility;
using WebHome.Models.DataEntity;
using WebHome.Properties;
using WebHome.Models.Locale;
using WebHome.DataModels;
using System.IO;
using WebHome.Models.ViewModel;
using LinqToDB;
using LineMessagingAPISDK.Models;
using Newtonsoft.Json.Linq;

namespace WebHome.Helper.Jobs
{
    public class MessageAutoDispatcher : IJob
    {
        public void Dispose()
        {
            
        }

        public void DoJob()
        {
            BusinessExtensionMethods.SynchronizeUserDevices();
        }

        public DateTime GetScheduleToNextTurn(DateTime current)
        {
            return current.AddMinutes(5);
        }
    }

    public class AliveDeviceStatusDispatcher : IJob
    {
        public void Dispose()
        {

        }

        public void DoJob()
        {
            BusinessExtensionMethods.CheckDeviceAlive();
        }

        public DateTime GetScheduleToNextTurn(DateTime current)
        {
            return current.AddMinutes(Settings.Default.DeviceHeartBeatPeriodInMinutes);
        }
    }

    public static class JobLauncher
    {
        public static void StartUp()
        {

            System.Net.ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;

            //ThreadPool.QueueUserWorkItem(stateInfo =>
            //{
            //    Thread.Sleep(10000);
            //    Console.WriteLine("Web starts up...");
            //    using (WebClient client = new WebClient())
            //    {
            //        Logger.Info(client.DownloadString(String.Format("{0}{1}",
            //            Settings.Default.HostUrl,
            //            VirtualPathUtility.ToAbsolute("~/DeviceEvents/Index"))));
            //    }
            //});

            Console.WriteLine("Daemon Job launches ...");

            JobScheduler.StartUp(Settings.Default.JobSchedulerInMilliseconds);

            var jobList = JobScheduler.JobList;

            if (Settings.Default.CommunicationMode == (int)Naming.CommunicationMode.ControlCenter)
            {

            }
            else
            {

                if (Settings.Default.CommunicationMode == (int)Naming.CommunicationMode.All || Settings.Default.CommunicationMode == (int)Naming.CommunicationMode.中保)
                {
                    if (jobList == null || !jobList.Any(j => j.AssemblyQualifiedName == typeof(MessageAutoDispatcher).AssemblyQualifiedName))
                    {
                        JobScheduler.AddJob(new JobItem
                        {
                            AssemblyQualifiedName = typeof(MessageAutoDispatcher).AssemblyQualifiedName,
                            Description = "傳送設備端裝置對應",
                            Schedule = DateTime.Today.Add(new TimeSpan(0, 0, 0))
                        });
                    }

                }

                if (Settings.Default.CommunicationMode != (int)Naming.CommunicationMode.AwtekOnly)
                {

                    if (jobList == null || !jobList.Any(j => j.AssemblyQualifiedName == typeof(AliveDeviceStatusDispatcher).AssemblyQualifiedName))
                    {
                        JobScheduler.AddJob(new JobItem
                        {
                            AssemblyQualifiedName = typeof(AliveDeviceStatusDispatcher).AssemblyQualifiedName,
                            Description = "檢查設備端狀態",
                            Schedule = DateTime.Today.Add(new TimeSpan(0, 10, 0))
                        });
                    }
                }

                if (Settings.Default.CommunicationMode == (int)Naming.CommunicationMode.All || Settings.Default.CommunicationMode == (int)Naming.CommunicationMode.中保)
                {
                    if (jobList == null || !jobList.Any(j => j.AssemblyQualifiedName == typeof(SecurityGuardDispatcher).AssemblyQualifiedName))
                    {
                        JobScheduler.AddJob(new JobItem
                        {
                            AssemblyQualifiedName = typeof(SecurityGuardDispatcher).AssemblyQualifiedName,
                            Description = "檢查設備端保全設定",
                            Schedule = DateTime.Today.Add(new TimeSpan(0, 20, 0))
                        });
                    }

                    if (jobList == null || !jobList.Any(j => j.AssemblyQualifiedName == typeof(DeviceEventDispatcher).AssemblyQualifiedName))
                    {
                        JobScheduler.AddJob(new JobItem
                        {
                            AssemblyQualifiedName = typeof(DeviceEventDispatcher).AssemblyQualifiedName,
                            Description = "檢查設備端警報",
                            Schedule = DateTime.Today.Add(new TimeSpan(0, 30, 0))
                        });
                    }
                }
            }
        }

        public static QueuedProcessHandler DKCMSMessageDispatcher { get; } = new QueuedProcessHandler(Logger.Instance)
        {
            PeriodInSeconds = 5,
            Process = () =>
            { 
                DispatchTextLogs();

                if (AppSettings.Default.DispatchTalkLog)
                {
                    DispatchTalkLogs();
                }
            }
        };

        private static void DispatchTextLogs()
        {
            using (dnakeDB db = new dnakeDB("dnake"))
            {
                var loggers = db.GetTable<text_logger>().Where(a => a.done == 0)
                                    .ToList();

                if (loggers.Any())
                {
                    String url = $"{AppSettings.Default.LineMessageCenter}/MessageCenter/LineEvents/PushMessage";
                    var viewModel = new UserAccountQueryViewModel
                    {
                        Title = "CMS公告訊息",
                    };

                    using (var models = new ModelSource<LiveDevice>())
                    {
                        foreach (var logger in loggers)
                        {
                            String id = $"-{logger.user}";
                            var user = models.GetTable<UserProfile>().Where(p => p.PID.Contains(id)).FirstOrDefault();
                            if (user != null)
                            {
                                String logPath = Path.Combine(Path.Combine(Logger.LogPath, "SMS", $"{user.PID}").CheckStoredPath(), $"{logger.id}.txt");
                                if (!File.Exists(logPath))
                                {
                                    File.WriteAllText(logPath, logger.text);
                                    viewModel.PID = user.PID;
                                    viewModel.Message = logger.text;
                                    url.PushToLineMessageCenter(viewModel);
                                }
                            }
                        }

                    }
                }
            }
        }

        private static void DispatchTalkLogs()
        {
            using (dnakeDB db = new dnakeDB("dnake"))
            {
                var talkLogs = db.GetTable<talk_logger>().ToList();
                if (talkLogs.Any())
                {
                    foreach (var talk in talkLogs)
                    {
                        if (AppSettings.Default.UseCustomBA)
                        {
                            if (AppSettings.Default.CustomBA_DirectToken != null)
                            {
                                String jsonRequest;

                                jsonRequest = (new
                                {
                                    TokenID = AppSettings.Default.CustomBA_DirectToken,
                                    DeviceType = Settings.Default.PRMType,
                                    DeviceUri = talk.to_user,
                                    //SensorID = loop,
                                    UserID = talk.user,
                                    NeedRelease = false,
                                    //EventType = eventType,
                                    EventSubtype = "0",
                                    EventLevel = 0,
                                    EventTimeout = 0,
                                    //DeviceStatus = status,
                                    CardNo = "",
                                    EventDetail = "",
                                    isLog = false,
                                    DeviceTime = talk.ts,
                                }).JsonStringify();

                                try
                                {
                                    using (WebClient client = new WebClient())
                                    {
                                        //Logger.Debug(Settings.Default.GetAuthToken + queryValues.ToQueryString());
                                        client.Headers[HttpRequestHeader.ContentType] = "application/json";
                                        var json = client.UploadString(Settings.Default.MessageCenter_BA_Service_sr_BA_DeviceWebService, jsonRequest);
                                        Logger.Info($"{Settings.Default.MessageCenter_BA_Service_sr_BA_DeviceWebService} => {jsonRequest}");
                                        Logger.Info(json);
                                        //JObject result = JObject.Parse(json);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                            }
                        }

                        //刪除talk_logger資料
                        db.talk_logger.Delete(t => t.id == talk.id);
                    }
                }
            }
        }
    }
}