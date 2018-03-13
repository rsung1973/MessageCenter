using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Utility;
using WebHome.DataModels;
using WebHome.DataPort;
using WebHome.Helper.Jobs;
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

        public static void SynchronizeUserDevices()
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
                            var users = db.users.GroupBy(u => u.user_Column)
                                .Select(g => g.First()).ToArray();

                            foreach (var item in users)
                            {
                                var profile = mgr.GetTable<UserProfile>().Where(u => u.PID == item.user_Column).FirstOrDefault();
                                if(profile==null)
                                {
                                    profile = new UserProfile
                                    {
                                        PID = item.user_Column,
                                        UserName = item.name
                                    };
                                    mgr.GetTable<UserProfile>().InsertOnSubmit(profile);
                                    mgr.SubmitChanges();
                                }

                                UserRegister userReg = profile.UserRegister;
                                if (userReg == null)
                                {
                                    userReg = new UserRegister
                                    {
                                        UserProfile = profile
                                    };

                                    dynamic queryValues = new DynamicQueryStringParameter();
                                    queryValues.prm_l1_device_type = Settings.Default.PRMType;
                                    queryValues.prm_parent_uri = "NONE";
                                    queryValues.prm_device_name = Settings.Default.LeaderID + Settings.Default.CenterID + String.Format("{0:000000}", profile.UID);
                                    queryValues.prm_used_device_type = Settings.Default.PRMUsed;
                                    queryValues.prm_alarm_point_yn = "Y";
                                    queryValues.prm_building_id = item.build;
                                    queryValues.prm_floor_id = item.floor;
                                    queryValues.prm_device_name_ui = profile.PID;

                                    var result = MessageOutbound.Instance.InsertDevice(queryValues);

                                    if (result != null && String.IsNullOrEmpty((String)result[0].return_message))
                                    {
                                        userReg.DeviceUri = result[0].return_final_uri;
                                        mgr.SubmitChanges();
                                    }
                                    else
                                    {
                                        Logger.Warn("登錄設備失敗=>" + JsonConvert.SerializeObject(result));
                                    }
                                }

                                if (!String.IsNullOrEmpty(userReg.DeviceUri))
                                {
                                    var items = db.GetTable<alarm_zone>().Where(a => a.user == item.user_Column).ToArray();
                                    foreach (var deviceItem in items)
                                    {
                                        var dbItem = deviceTable.Where(d => d.DeviceID == deviceItem.id).FirstOrDefault();
                                        if (dbItem == null)
                                        {
                                            dbItem = new LiveDevice
                                            {
                                                DeviceID = deviceItem.id
                                            };
                                            deviceTable.InsertOnSubmit(dbItem);
                                        }
                                        if(!dbItem.UID.HasValue)
                                        {
                                            dbItem.UserRegister = userReg;
                                            mgr.SubmitChanges();
                                        }
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

        //public static void SynchronizeDevices()
        //{
        //    if (Interlocked.Increment(ref __BusyCount) == 1)
        //    {
        //        try
        //        {
        //            using (dnakeDB db = new dnakeDB("dnake"))
        //            {
        //                using (ModelSource<MessageCenterDataContext> mgr = new ModelSource<MessageCenterDataContext>())
        //                {
        //                    var deviceTable = mgr.GetTable<LiveDevice>();
        //                    var items = db.GetTable<alarm_zone>().ToArray();
        //                    foreach (var item in items)
        //                    {
        //                        if (!deviceTable.Any(d => d.DeviceID == item.id))
        //                        {
        //                            dynamic queryValues = new DynamicQueryStringParameter();
        //                            queryValues.prm_l1_device_type = Settings.Default.PRMType;
        //                            queryValues.prm_parent_uri = "NONE";
        //                            queryValues.prm_device_name = Settings.Default.LeaderID + Settings.Default.CenterID + String.Format("{0:000000}", item.id);
        //                            queryValues.prm_used_device_type = Settings.Default.PRMUsed;
        //                            queryValues.prm_alarm_point_yn = "Y";
        //                            var u = db.GetTable<user>().Where(r => r.user_Column == item.user).FirstOrDefault();
        //                            if (u != null)
        //                            {
        //                                queryValues.prm_building_id = u.build;
        //                                queryValues.prm_floor_id = u.floor;
        //                                var sensor = db.alarm_sensor.Where(s => s.id == item.sensor).FirstOrDefault();
        //                                if (sensor != null)
        //                                {
        //                                    queryValues.prm_device_name_ui = u.name + "_" + item.name + "_" + sensor.name.Replace(" ", "");
        //                                }
        //                                else
        //                                {
        //                                    queryValues.prm_device_name_ui = u.name + "_" + item.name;
        //                                }
        //                            }
        //                            var result = MessageOutbound.Instance.InsertDevice(queryValues);

        //                            if (result != null && String.IsNullOrEmpty((String)result[0].return_message))
        //                            {
        //                                deviceTable.InsertOnSubmit(new LiveDevice
        //                                {
        //                                    DeviceID = item.id,
        //                                    DeviceUri = result[0].return_final_uri
        //                                });
        //                                mgr.SubmitChanges();
        //                            }
        //                            else
        //                            {
        //                                Logger.Warn("登錄設備失敗=>" + JsonConvert.SerializeObject(result));
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Logger.Error(ex);
        //        }
        //        finally
        //        {
        //            Interlocked.Exchange(ref __BusyCount, 0);
        //        }
        //    }

        //}


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
                                    if (Settings.Default.CommunicationMode == 0 || Settings.Default.CommunicationMode == 1)   //中保
                                    {
                                        reportStatus(db, mgr, deviceTable, item, Naming.DeviceLevelDefinition.正常);
                                    }
                                    if (Settings.Default.CommunicationMode == 0 || Settings.Default.CommunicationMode == 2)        //新保
                                    {
                                        TouchLifeDispatcher dispatcher = new TouchLifeDispatcher();
                                        dispatcher.ReportUserStatus(db, mgr, item, Naming.DeviceLevelDefinition.正常);
                                    }
                                }
                                else if (interval > Settings.Default.DeviceHeartBeatPeriodInMinutes * 60 * 2)
                                {
                                    ///sync alive
                                    ///
                                    Logger.Debug("斷線=>current:" + DateTime.Now + " , hearbeat:" + item.heartbeat);
                                    if (Settings.Default.CommunicationMode == 0 || Settings.Default.CommunicationMode == 1)   //中保
                                    {
                                        reportStatus(db, mgr, deviceTable, item, Naming.DeviceLevelDefinition.斷線);
                                    }
                                    if (Settings.Default.CommunicationMode == 0 || Settings.Default.CommunicationMode == 2)        //新保
                                    {
                                        TouchLifeDispatcher dispatcher = new TouchLifeDispatcher();
                                        dispatcher.ReportUserStatus(db, mgr, item, Naming.DeviceLevelDefinition.斷線);
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
                    Interlocked.Exchange(ref __AliveBusyCount, 0);
                }
            }
        }

        private static void reportStatus(dnakeDB db, ModelSource<MessageCenterDataContext> mgr, System.Data.Linq.Table<LiveDevice> deviceTable, device item,Naming.DeviceLevelDefinition status)
        {
            var deviceCheckList = db.GetTable<alarm_zone>().Where(a => a.user == item.user).ToArray();
            //Logger.Debug("user: " + item.user + ",alarm_zone: " + deviceCheckList.Count());
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
                if (status == Naming.DeviceLevelDefinition.正常)
                {
                    if (device.CurrentLevel.HasValue && device.CurrentLevel != (int)Naming.DeviceLevelDefinition.斷線)
                        return;
                }
            
                //Logger.Debug(device.LiveID + ":" + ((Naming.DeviceLevelDefinition)device.CurrentLevel) + " => " + status);

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

        public static void PushMessage(JArray data)
        {

            using (dnakeDB db = new dnakeDB("dnake"))
            {
                MySqlCommand cmd = (MySqlCommand)db.CreateCommand();
                cmd.Connection = (MySqlConnection)db.Connection;
                cmd.CommandText = @"INSERT INTO `text_logger`(`user`, `type`, `text`, `done`, `ts`) 
                                    SELECT `user`,@type,@text,@done,@ts from `users` where `user` = @user";
                cmd.Parameters.Add(new MySqlParameter("@user", MySqlDbType.String));
                cmd.Parameters.Add(new MySqlParameter("@type", MySqlDbType.Int32));
                cmd.Parameters.Add(new MySqlParameter("@text", MySqlDbType.String));
                cmd.Parameters.Add(new MySqlParameter("@done", MySqlDbType.Int32));
                cmd.Parameters.Add(new MySqlParameter("@ts", MySqlDbType.Datetime));

                MySqlCommand cmdAll = (MySqlCommand)db.CreateCommand();
                cmdAll.Connection = (MySqlConnection)db.Connection;
                cmdAll.CommandText = @"INSERT INTO `text_logger`(`user`, `type`, `text`, `done`, `ts`) 
                                    SELECT `user`,@type,@text,@done,@ts from `users` ";
                cmdAll.Parameters.Add(new MySqlParameter("@type", MySqlDbType.Int32));
                cmdAll.Parameters.Add(new MySqlParameter("@text", MySqlDbType.String));
                cmdAll.Parameters.Add(new MySqlParameter("@done", MySqlDbType.Int32));
                cmdAll.Parameters.Add(new MySqlParameter("@ts", MySqlDbType.Datetime));

                foreach (dynamic rq in (JArray)data)
                {
                    try
                    {
                        String userName = (String)rq.C09_msg_ps_01;
                        if (String.IsNullOrEmpty(userName))
                            userName = (String)rq.C06_msg_address;
                        StringBuilder msg = new StringBuilder();
                        msg.Append(rq.C02_msg_subject).Append("\r\n").Append(rq.C03_msg_contents);

                        if (msg.Length > 2)
                        {
                            int index = 0;
                            int total = msg.Length;
                            int read = Math.Min(total, Settings.Default.PushMessageMaxLength);

                            if (String.IsNullOrEmpty(userName))
                            {
                                cmdAll.Parameters["@type"].Value = 0;
                                cmdAll.Parameters["@done"].Value = 0;
                                cmdAll.Parameters["@ts"].Value = DateTime.Now;
                                while (total > 0)
                                {
                                    cmdAll.Parameters["@text"].Value = msg.ToString(index, read);
                                    cmdAll.ExecuteNonQuery();

                                    index += read;
                                    total -= read;
                                    read = Math.Min(total, Settings.Default.PushMessageMaxLength);
                                }
                            }
                            else
                            {
                                cmd.Parameters["@user"].Value = userName;
                                cmd.Parameters["@type"].Value = 0;
                                cmd.Parameters["@done"].Value = 0;
                                cmd.Parameters["@ts"].Value = DateTime.Now;
                                while (total > 0)
                                {
                                    cmd.Parameters["@text"].Value = msg.ToString(index, read);
                                    cmd.ExecuteNonQuery();

                                    index += read;
                                    total -= read;
                                    read = Math.Min(total, Settings.Default.PushMessageMaxLength);
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

        public static void SendToFCM(this ModelSource<MessageCenterDataContext> models,String pid)
        {
            var profile = models.GetTable<UserProfile>().Where(u => u.PID == pid).FirstOrDefault();

            if (profile != null && profile.UserFCM.Count > 0)
            {
                using (WebClient client = new WebClient())
                {
                    dynamic fcmData = new JObject { };
                    fcmData.to = "";
                    fcmData.data = new JObject { };
                    fcmData.data.id = 1;
                    fcmData.data.text = "啟動保全";


                    client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    client.Headers.Add("Authorization", Settings.Default.GoogleFCMAuthorization);

                    foreach (var item in profile.UserFCM)
                    {
                        try
                        {
                            fcmData.to = item.FCMToken;
                            Logger.Info(client.UploadString(Settings.Default.GoogleFCMUrl, JsonConvert.SerializeObject(fcmData)));
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