using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using CommonLib.Helper;
using MySql.Data.MySqlClient;
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
    public class TouchLifeDispatcher
    {
        public static readonly String __TouchLifeQueue = "TouchLife";
        private static int __Counter = 0;
        private static int __BusyCount = 0;

        public void Outbound(dnakeDB db, alarm_zone zone, ModelSource<LiveDevice> mgr, Naming.AlarmMode alarmMode)
        {
            var rq = createOutboundRequest(db, zone, alarmMode);
            if (rq != null)
                processRequest(rq);

        }

        private void processRequest(dynamic rq)
        {
            dynamic item = new JObject();
            item.jsonarray = new JArray(rq);

            String storedPath = Path.Combine(Logger.LogPath, __TouchLifeQueue);
            if (!Directory.Exists(storedPath))
            {
                Directory.CreateDirectory(storedPath);
            }

            if (__Counter < 0)
                __Counter = 0;


            String fileName = Path.Combine(storedPath, String.Format("{0:0000000000}", Interlocked.Increment(ref __Counter)) + ".json");
            File.WriteAllText(fileName, JsonConvert.SerializeObject(item));

            ProcessOutbound();
        }

        private dynamic createOutboundRequest(dnakeDB db, alarm_zone zone, Naming.AlarmMode alarmMode)
        {
            dynamic rq = new JObject();
            rq.comand_id = DateTime.Now.ToString("yyyyMMddHHmmssffff");
            rq.intercom_number = zone.user;
            rq.type = "03";

            var sensor = db.alarm_sensor.Where(s => s.sensor == zone.sensor).FirstOrDefault();
            if (sensor != null)
            {
                switch (sensor.sensor)
                {
                    case 0: //Smoke
                        rq.subtype = "01";
                        break;
                    case 1: //Gas
                        rq.subtype = "06";
                        break;
                    case 2: //PIR
                        rq.subtype = "00";
                        break;
                    case 3: //Door
                    case 4: //Window
                    case 7: //Pull Cord
                    case 8: //Bed Mat
                        rq.subtype = "03";
                        break;
                    case 5: //Panic
                        rq.subtype = "05";
                        break;
                    case 6: //Flood
                        rq.subtype = "04";
                        break;
                }

                rq.subject = null;
                rq.message = (zone.zone + 1).ToString() + "," + sensor.name;
                rq.event_date = DateTime.Now.ToString("yyyy/MM/dd");
                rq.event_time = DateTime.Now.ToString("HH:mm:ss");
                rq.type_mode = alarmMode.ToString();
                rq.status = "";
                rq.verify = "intercom" + rq.comand_id + "inter" + rq.event_date + "com" + rq.type_mode;

                return rq;
            }

            return null;

        }

        public void ReportDeviceStatus(dnakeDB db, alarm_zone deviceToCheck, ModelSource<LiveDevice> mgr, Naming.DeviceLevelDefinition status)
        {
            var rq = createOutboundRequest(db, deviceToCheck, Naming.AlarmMode.alarm);
            if (rq != null)
            {
                if (status == Naming.DeviceLevelDefinition.正常)
                {
                    rq.subtype = "08";
                }
                else if (status == Naming.DeviceLevelDefinition.斷線)
                {
                    rq.subtype = "09";
                }

                processRequest(rq);
            }
        }

        public void ReportUserStatus(dnakeDB db, ModelSource<LiveDevice> mgr, device item, Naming.DeviceLevelDefinition status)
        {
            var deviceCheckList = db.GetTable<alarm_zone>().Where(a => a.user == item.user).ToArray();
            foreach (var deviceToCheck in deviceCheckList)
            {
                ReportDeviceStatus(db, deviceToCheck, mgr, status);
            }
        }



        public void ProcessOutbound()
        {
            if (Interlocked.Increment(ref __BusyCount) == 1)
            {
                ThreadPool.QueueUserWorkItem(t =>
                {
                    try
                    {
                        String storedPath = Path.Combine(Logger.LogPath, __TouchLifeQueue);
                        if (!Directory.Exists(storedPath))
                        {
                            return;
                        }

                        var files = Directory.GetFiles(storedPath);
                        bool run = files != null && files.Length > 0;
                        while (run)
                        {
                            foreach (var item in files)
                            {
                                try
                                {
                                    using (WebClient client = new WebClient())
                                    {
                                        client.Headers["Content-Type"] = "application/json";

                                        var result = client.UploadString(Settings.Default.TouchLifeOutbound, File.ReadAllText(item));
                                        Logger.Debug(item + ", 回傳 => " + result);

                                        dynamic rs = JsonConvert.DeserializeObject(result);
                                        if (rs is JArray && rs[0].status != null && rs[0].status.ToString() == "0")
                                        {
                                            File.WriteAllText(
                                                Path.Combine(Logger.LogDailyPath, Path.GetFileNameWithoutExtension(item) + "(回傳)" + ".json"), result);
                                            String target = Path.Combine(Logger.LogDailyPath, Path.GetFileName(item));
                                            if (File.Exists(target))
                                                File.Delete(target);
                                            File.Move(item, target);
                                        }
                                        else
                                        {
                                            Logger.Warn(item + ", 回傳失敗 => " + result);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }

                            }

                            files = Directory.GetFiles(storedPath);
                            run = files != null && files.Length > 0;
                            if (run)
                                Thread.Sleep(5000);
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
                });
            }
        }

        public object PushMessage(object message)
        {
            dynamic item = message;
            dynamic data = item != null ? item.jsonarray : null;
            if (data==null || !(data is JArray))
            {
                dynamic result = new JObject();
                result.status = 0;
                return new JArray(result);
            }

            using (dnakeDB db = new dnakeDB("dnake"))
            {
                MySqlCommand cmd = (MySqlCommand)db.CreateCommand();
                cmd.Connection = (MySqlConnection)db.Connection;
                cmd.CommandText = "INSERT INTO `text_logger`(`user`, `type`, `text`, `done`, `ts`) VALUES (@user,@type,@text,@done,@ts)";
                cmd.Parameters.Add(new MySqlParameter("@user", MySqlDbType.String));
                cmd.Parameters.Add(new MySqlParameter("@type", MySqlDbType.Int32));
                cmd.Parameters.Add(new MySqlParameter("@text", MySqlDbType.String));
                cmd.Parameters.Add(new MySqlParameter("@done", MySqlDbType.Int32));
                cmd.Parameters.Add(new MySqlParameter("@ts", MySqlDbType.Datetime));

                foreach (dynamic rq in (JArray)data)
                {
                    rq.status = 9999;
                    try
                    {
                        if (rq.verify == ("intercom" + rq.comand_id + "inter" + rq.event_date + "com" + rq.type_mode))
                        {
                            if (rq.type_mode != null && rq.type_mode.ToString().ToUpper() == "CREATE")
                            {
                                String userName = (String)rq.intercom_number;
                                var user = db.users.Where(u => u.user_Column == userName).FirstOrDefault();
                                if (user != null)
                                {
                                    cmd.Parameters["@user"].Value = userName;
                                    cmd.Parameters["@type"].Value = 0;
                                    cmd.Parameters["@text"].Value = (rq.subject ?? "") + (rq.message ?? "");
                                    cmd.Parameters["@done"].Value = 0;
                                    cmd.Parameters["@ts"].Value = DateTime.Now;
                                    cmd.ExecuteNonQuery();
                                    rq.status = 0;
                                }
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        Logger.Error(ex);
                    }

                    rq.type = null;
                    rq.subtype = null;
                    rq.subject = null;
                    rq.message = null;
                    rq.event_time = null;
                }
            }
            return data;
        }
    }
}
