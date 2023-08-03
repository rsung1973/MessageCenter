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
    public class DeviceEventDispatcher : IJob
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
                    checkDeviceEvent();
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
            return current.AddSeconds(Settings.Default.CheckEventIntervalInSeconds);
        }

        private void checkDeviceEvent()
        {
            using (dnakeDB db = new dnakeDB("dnake"))
            {
                using (ModelSource<LiveDevice> mgr = new ModelSource<LiveDevice>())
                {
                    var deviceTable = mgr.GetTable<LiveDevice>();
                    alarm_logger[] items;
                    if(__CheckID>=0)
                    {
                        items = db.GetTable<alarm_logger>()
                            .Where(a => /*a.data == 1 &&*/ a.id > __CheckID
                                || a.confirmTs >= DateTime.Now.AddSeconds(-60 * Settings.Default.AlertBeforeMinutes))
                            .ToArray();
                    }
                    else
                    {
                        items = db.GetTable<alarm_logger>()
                            .Where(a => /*a.data == 1 &&*/ a.ts >= DateTime.Now.AddSeconds(-60 * Settings.Default.AlertBeforeMinutes)
                                || a.confirmTs >= DateTime.Now.AddSeconds(-60 * Settings.Default.AlertBeforeMinutes))
                            .ToArray();
                    }

                    Logger.Debug("__CheckID:" + __CheckID + ",ItemsCount:" + items.Length);

                    if (items.Length > 0)
                    {
                        for (int i = items.Length - 1; i >= 0; i--)
                        {
                            if (items[i].confirm == 0)
                            {
                                __CheckID = items[i].id;
                                break;
                            }
                        }
                        //__CheckID = items[items.Length - 1].id;
                        Interlocked.Exchange(ref __BusyCount, 0);

                        foreach (var item in items)
                        {
                            try
                            {
                                var zone = db.alarm_zone.Where(z => z.user == item.user && z.zone == item.zone).FirstOrDefault();
                                if(zone!=null)
                                {
                                    if (Settings.Default.CommunicationMode == (int)Naming.CommunicationMode.All || Settings.Default.CommunicationMode == (int)Naming.CommunicationMode.中保)    //中保
                                    {
                                        if (item.confirm == 0)     //sensor alarm
                                        {
                                            if (zone.zone < 5)
                                            {
                                                dispatchByZone(db, mgr, deviceTable, zone);
                                            }
                                            else
                                            {
                                                dispatchBySensor(db, mgr, deviceTable, zone);
                                            }
                                        }
                                        else    //sensor release
                                        {
                                            zone.ReportDeviceEvent(mgr, deviceTable, Naming.DeviceLevelDefinition.正常, "1");
                                        }
                                    }
                                    if (Settings.Default.CommunicationMode == (int)Naming.CommunicationMode.All || Settings.Default.CommunicationMode == (int)Naming.CommunicationMode.新保)        //新保
                                    {
                                        TouchLifeDispatcher tl = new TouchLifeDispatcher();
                                        if (item.data == 1 || item.data==0)
                                        {
                                            tl.Outbound(db, zone, mgr, (Naming.AlarmMode)item.data);
                                        }
                                    }
                                    if (Settings.Default.CommunicationMode == (int)Naming.CommunicationMode.All || Settings.Default.CommunicationMode == (int)Naming.CommunicationMode.中保 || Settings.Default.CommunicationMode == (int)Naming.CommunicationMode.AwtekOnly)
                                    {
                                        if (item.confirm == 0)
                                        {
                                            mgr.SendToFCM(item.user);

                                        }
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

        private void dispatchBySensor(dnakeDB db, ModelSource<LiveDevice> mgr, System.Data.Linq.Table<LiveDevice> deviceTable, alarm_zone zone)
        {
            var sensor = db.alarm_sensor.Where(s => s.sensor == zone.sensor).FirstOrDefault();
            if (sensor != null)
            {
                switch (sensor.sensor)
                {
                    case 0: //Smoke
                        zone.ReportDeviceEvent(mgr, deviceTable, Naming.DeviceLevelDefinition.火災, "1");
                        break;
                    case 1: //Gas
                        zone.ReportDeviceEvent(mgr, deviceTable, Naming.DeviceLevelDefinition.瓦斯異常, "1");
                        break;
                    case 2: //PIR
                        zone.ReportDeviceEvent(mgr, deviceTable, Naming.DeviceLevelDefinition.緊急, "1");
                        break;
                    case 3: //Door
                        zone.ReportDeviceEvent(mgr, deviceTable, Naming.DeviceLevelDefinition.迴路一異常, "1");
                        break;
                    case 4: //Window
                        zone.ReportDeviceEvent(mgr, deviceTable, Naming.DeviceLevelDefinition.迴路二異常, "1");
                        break;
                    case 5: //Panic
                        zone.ReportDeviceEvent(mgr, deviceTable, Naming.DeviceLevelDefinition.迴路三異常, "1");
                        break;
                    case 6: //Flood
                        zone.ReportDeviceEvent(mgr, deviceTable, Naming.DeviceLevelDefinition.迴路四異常, "1");
                        break;
                    case 7: //Pull Cord
                        zone.ReportDeviceEvent(mgr, deviceTable, Naming.DeviceLevelDefinition.迴路五異常, "1");
                        break;
                    case 8: //Bed Mat
                        zone.ReportDeviceEvent(mgr, deviceTable, Naming.DeviceLevelDefinition.反脅迫警告, "1");
                        break;

                }
            }
        }
        private void dispatchByZone(dnakeDB db, ModelSource<LiveDevice> mgr, System.Data.Linq.Table<LiveDevice> deviceTable, alarm_zone zone)
        {
            switch (zone.zone)
            {
                case 0: //Smoke
                    zone.ReportDeviceEvent(mgr, deviceTable, Naming.DeviceLevelDefinition.迴路一異常, "1");
                    //zone.ReportDeviceEvent(mgr, deviceTable, Naming.DeviceLevelDefinition.反脅迫警告, "1");
                    break;
                case 1: //Gas
                    zone.ReportDeviceEvent(mgr, deviceTable, Naming.DeviceLevelDefinition.迴路二異常, "1");
                    //zone.ReportDeviceEvent(mgr, deviceTable, Naming.DeviceLevelDefinition.緊急, "1");
                    break;
                case 2: //PIR
                    zone.ReportDeviceEvent(mgr, deviceTable, Naming.DeviceLevelDefinition.迴路三異常, "1");
                    //zone.ReportDeviceEvent(mgr, deviceTable, Naming.DeviceLevelDefinition.瓦斯異常, "1");
                    break;
                case 3: //Door
                    zone.ReportDeviceEvent(mgr, deviceTable, Naming.DeviceLevelDefinition.迴路四異常, "1");
                    //zone.ReportDeviceEvent(mgr, deviceTable, Naming.DeviceLevelDefinition.緊急, "1");
                    break;
                case 4: //Door
                    zone.ReportDeviceEvent(mgr, deviceTable, Naming.DeviceLevelDefinition.迴路五異常, "1");
                    //zone.ReportDeviceEvent(mgr, deviceTable, Naming.DeviceLevelDefinition.緊急, "1");
                    break;
            }
        }

    }
}