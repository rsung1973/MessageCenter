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
}