using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Utility;
using WebHome.DataModels;
using WebHome.DataPort;
using WebHome.Models.DataEntity;
using WebHome.Models.Helper;
using WebHome.Models.Locale;
using WebHome.Properties;

namespace WebHome.Helper
{
    public static partial class BusinessExtensionMethods
    {
        private static int __BusyCount;
        private static int __AliveBusyCount;

        public static void SynchronizeDevices()
        {
            if (Interlocked.Increment(ref __BusyCount) == 1)
            {
                try
                {
                    using (dnakeDB db = new dnakeDB("dnake"))
                    {
                        using (ModelSource<MessageCenterDataContext> mgr = new ModelSource<MessageCenterDataContext>())
                        {
                            var deviceTable = mgr.GetTable<LiveDevice>();
                            var items = db.GetTable<alarm_zone>().ToArray();
                            foreach (var item in items)
                            {
                                if (!deviceTable.Any(d => d.DeviceID == item.id))
                                {
                                    dynamic queryValues = new DynamicQueryStringParameter();
                                    queryValues.prm_l1_device_type = Settings.Default.PRMType;
                                    queryValues.prm_parent_uri = "NONE";
                                    queryValues.prm_device_name = Settings.Default.LeaderID + Settings.Default.CenterID + String.Format("{0:000000}", item.id);
                                    queryValues.prm_used_device_type = Settings.Default.PRMUsed;
                                    queryValues.prm_alarm_point_yn = "Y";
                                    var u = db.GetTable<user>().Where(r => r.user_Column == item.user).FirstOrDefault();
                                    if (u != null)
                                    {
                                        queryValues.prm_building_id = u.build;
                                        queryValues.prm_floor_id = u.floor;
                                        var sensor = db.alarm_sensor.Where(s => s.id == item.sensor).FirstOrDefault();
                                        if (sensor != null)
                                        {
                                            queryValues.prm_device_name_ui = u.name + "_" + item.name + "_" + sensor.name.Replace(" ", "");
                                        }
                                        else
                                        {
                                            queryValues.prm_device_name_ui = u.name + "_" + item.name;
                                        }
                                    }
                                    var result = MessageOutbound.Instance.InsertDevice(queryValues);

                                    if (result != null && String.IsNullOrEmpty((String)result[0].return_message))
                                    {
                                        deviceTable.InsertOnSubmit(new LiveDevice
                                        {
                                            DeviceID = item.id,
                                            DeviceUri = result[0].return_final_uri
                                        });
                                        mgr.SubmitChanges();
                                    }
                                    else
                                    {
                                        Logger.Warn("登錄設備失敗=>" + JsonConvert.SerializeObject(result));
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
                finally
                {
                    Interlocked.Exchange(ref __BusyCount, 0);
                }
            }
            
        }

        public static void CheckDeviceAlive()
        {
            if (Interlocked.Increment(ref __AliveBusyCount) == 1)
            {
                try
                {
                    using (dnakeDB db = new dnakeDB("dnake"))
                    {
                        using (ModelSource<MessageCenterDataContext> mgr = new ModelSource<MessageCenterDataContext>())
                        {
                            var deviceTable = mgr.GetTable<LiveDevice>();
                            var items = db.GetTable<device>().ToArray();
                            foreach (var item in items)
                            {
                                if (!item.heartbeat.HasValue)
                                    continue;

                                int interval = (int)(DateTime.Now - item.heartbeat.Value).TotalSeconds;
                                if (interval > Settings.Default.DeviceHeartBeatPeriodInMinutes * 60 && interval <= Settings.Default.DeviceHeartBeatPeriodInMinutes * 2 * 60)
                                {
                                    ///觀察期,bypass
                                    continue;
                                }
                                else if (interval <= Settings.Default.DeviceHeartBeatPeriodInMinutes * 60)
                                {
                                    ///sync alive
                                    ///
                                    Logger.Debug("正常=>current:" + DateTime.Now + " , hearbeat:" + item.heartbeat);
                                    reportStatus(db, mgr, deviceTable, item, Naming.DeviceLevelDefinition.正常);
                                }
                                else if (interval > Settings.Default.DeviceHeartBeatPeriodInMinutes * 60 * 2)
                                {
                                    ///sync alive
                                    ///
                                    Logger.Debug("斷線=>current:" + DateTime.Now + " , hearbeat:" + item.heartbeat);
                                    reportStatus(db, mgr, deviceTable, item, Naming.DeviceLevelDefinition.斷線);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
                finally
                {
                    Interlocked.Exchange(ref __AliveBusyCount, 0);
                }
            }
        }

        private static void reportStatus(dnakeDB db, ModelSource<MessageCenterDataContext> mgr, System.Data.Linq.Table<LiveDevice> deviceTable, device item,Naming.DeviceLevelDefinition status)
        {
            var deviceCheckList = db.GetTable<alarm_zone>().Where(a => a.user == item.user).ToArray();
            foreach (var deviceToCheck in deviceCheckList)
            {
                ReportDeviceStatus(deviceToCheck, mgr, deviceTable, status);
            }
        }

        public static void ReportDeviceStatus(this alarm_zone deviceToCheck,ModelSource<MessageCenterDataContext> mgr, System.Data.Linq.Table<LiveDevice> deviceTable, Naming.DeviceLevelDefinition status)
        {
            var device = deviceTable.Where(d => d.DeviceID == deviceToCheck.id).FirstOrDefault();
            if (device != null && (!device.CurrentLevel.HasValue || device.CurrentLevel != (int)status))
            {
                Logger.Debug(device.LiveID + ":" + ((Naming.DeviceLevelDefinition)device.CurrentLevel) + " => " + status);

                dynamic result = MessageOutbound.Instance.UpdateDeviceStatus(device, Naming.DeviceStatusCode[(int)status]);
                if (result != null && (String)result[0].result == "OK")
                {
                    var logDate = DateTime.Now;
                    device.CurrentLevel = (int)status;
                    mgr.GetTable<DeviceEventReport>().InsertOnSubmit(new DeviceEventReport
                    {
                        LiveID = device.LiveID,
                        LevelID = (int)status,
                        ReportDate = logDate,
                        DeviceEventLog = new DeviceEventLog
                        {
                            EventCode = Naming.DeviceStatusCode[(int)status],
                            LiveID = device.LiveID,
                            LogDate = logDate,
                            Rx = JsonConvert.SerializeObject(result)
                        }
                    });
                    mgr.SubmitChanges();
                }
                else
                {
                    Logger.Error("狀態更新失敗:" + device.DeviceID + "=>" + result);
                }
            }
        }

        public static LiveDevice ReportDeviceEvent(this alarm_zone deviceToCheck, ModelSource<MessageCenterDataContext> mgr, System.Data.Linq.Table<LiveDevice> deviceTable, Naming.DeviceLevelDefinition status,String eventType)
        {
            var device = deviceTable.Where(d => d.DeviceID == deviceToCheck.id).FirstOrDefault();
            if (device != null)
            {
                dynamic result = MessageOutbound.Instance.ReportDeviceEvent(device, Naming.DeviceStatusCode[(int)status], eventType);
                if (result != null && (String)result[0].result == "OK")
                {
                    var logDate = DateTime.Now;
                    mgr.GetTable<DeviceEventReport>().InsertOnSubmit(new DeviceEventReport
                    {
                        LiveID = device.LiveID,
                        LevelID = (int)status,
                        ReportDate = logDate,
                        DeviceEventLog = new DeviceEventLog
                        {
                            EventCode = Naming.DeviceStatusCode[(int)status],
                            LiveID = device.LiveID,
                            LogDate = logDate,
                            Rx = JsonConvert.SerializeObject(result)
                        }
                    });
                    mgr.SubmitChanges();

                    MessageOutbound.Instance.UpdateDeviceStatus(device, Naming.DeviceStatusCode[(int)status]);

                }
                else
                {
                    Logger.Error("狀態更新失敗:" + device.DeviceID + "=>" + result);
                }
            }
            return device;
        }

        //public static void ReportDeviceStatus(this alarm_zone deviceToCheck, ModelSource<MessageCenterDataContext> mgr, System.Data.Linq.Table<LiveDevice> deviceTable, Naming.DeviceLevelDefinition status, String eventType)
        //{
        //    var device = deviceTable.Where(d => d.DeviceID == deviceToCheck.id).FirstOrDefault();
        //    if (device != null)
        //    {
        //        dynamic result = MessageOutbound.Instance.UpdateDeviceStatus(device, Naming.DeviceStatusCode[(int)status]);
        //        if (result != null && (String)result[0].result == "OK")
        //        {
        //            var logDate = DateTime.Now;
        //            device.CurrentLevel = (int)status;
        //            mgr.GetTable<DeviceEventReport>().InsertOnSubmit(new DeviceEventReport
        //            {
        //                LiveID = device.LiveID,
        //                LevelID = (int)status,
        //                ReportDate = logDate,
        //                DeviceEventLog = new DeviceEventLog
        //                {
        //                    EventCode = Naming.DeviceStatusCode[(int)status],
        //                    LiveID = device.LiveID,
        //                    LogDate = logDate,
        //                    Rx = JsonConvert.SerializeObject(result)
        //                }
        //            });
        //            mgr.SubmitChanges();
        //        }
        //        else
        //        {
        //            Logger.Error("狀態更新失敗:" + device.DeviceID + "=>" + result);
        //        }
        //    }
        //}


    }
}