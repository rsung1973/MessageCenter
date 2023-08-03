using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using CommonLib.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Utility;
using WebHome.DataModels;
using WebHome.DataPort;
using WebHome.Models.DataEntity;
using WebHome.Models.Helper;
using WebHome.Models.Locale;
using WebHome.Properties;

namespace WebHome.Helper.Jobs
{
    public class SecurityGuardDispatcher : IJob
    {
        private static int __BusyCount;
        public static int __CheckID = -1;

        public void Dispose()
        {

        }

        public void DoJob()
        {
            if (Interlocked.Increment(ref __BusyCount) == 1)
            {
                try
                {
                    checkSecurityGuard();
                }
                catch(Exception ex)
                {
                    Logger.Error(ex);
                }
                finally
                {
                    Interlocked.Exchange(ref __BusyCount, 0);
                }
            }
        }

        public DateTime GetScheduleToNextTurn(DateTime current)
        {
            return current.AddSeconds(30);
        }

        private void checkSecurityGuard()
        {
            using (dnakeDB db = new dnakeDB("dnake"))
            {
                using (ModelSource<LiveDevice> mgr = new ModelSource<LiveDevice>())
                {
                    var deviceTable = mgr.GetTable<LiveDevice>();
                    alarm_defence[] items;
                    if(__CheckID>=0)
                    {
                        items = db.GetTable<alarm_defence>().Where(a=>a.id>__CheckID)
                            .ToArray();
                    }
                    else
                    {
                        items = db.GetTable<alarm_defence>().Where(a => a.ts >= DateTime.Now.AddSeconds(-60))
                            .ToArray();
                    }

                    if (items.Length > 0)
                    {

                        __CheckID = items[items.Length - 1].id;
                        Interlocked.Exchange(ref __BusyCount, 0);

                        foreach (var item in items)
                        {
                            try
                            {
                                if (item.defence > 0)
                                {
                                    ///設防
                                    ///
                                    var zones = item.io.Split(',').ToArray();
                                    for (int idx = 0; idx < zones.Length; idx++)
                                    {
                                        if (zones[idx] == "1")
                                        {
                                            var zone = db.alarm_zone.Where(z => z.user == item.user && z.zone == idx).FirstOrDefault();
                                            if (zone != null)
                                            {
                                                zone.ReportDeviceStatus(mgr, deviceTable, Naming.DeviceLevelDefinition.保全設定);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    ///解除
                                    ///
                                    var zones = db.alarm_zone.Where(z => z.user == item.user).ToArray();
                                    foreach (var zone in zones)
                                    {
                                        zone.ReportDeviceStatus(mgr, deviceTable, Naming.DeviceLevelDefinition.保全解除);
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                Logger.Error(ex);
                            }
                        }
                    }
                }
            }
        }

    }
}