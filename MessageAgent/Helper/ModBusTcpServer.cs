using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using Utility;
using MessageAgent.Properties;
using WebHome.Models.DataEntity;
using WebHome.DataModels;
using WebHome.Helper.Jobs;
using LinqToDB;

namespace MessageAgent.Helper
{
    public class ModbusTcpServer
    {
        private TcpListener tcpListener;
        private bool isRunning = false;
        private const int InitRegisterCount = 16; // 最大寄存器數量
        private readonly List<RegisterData> RegisterItem = new List<RegisterData>(InitRegisterCount); // 16個整數值陣列
        //private DateTime lastUpdateTime = DateTime.MinValue;
        private readonly ReaderWriterLockSlim _registerLock = new ReaderWriterLockSlim();

        public ModbusTcpServer() 
        {
            for(int i = 0; i < InitRegisterCount; i++)
            {
                RegisterItem.Add(new RegisterData { Value = 0, UserName = string.Empty });
            }

            if(Settings.Default.AWTEKMode)
            {
                ((Action)(() =>
                {
                    using (ModelSource<UserProfile> models = new ModelSource<UserProfile>())
                    {
                        var items = models.GetTable<UserProfile>();
                        if (items.Any())
                        {
                            try
                            {
                                _registerLock.EnterWriteLock();
                                if (RegisterItem?.Count == items.Count())
                                {
                                    int i = 0;
                                    foreach (var item in items)
                                    {
                                        RegisterItem[i].Value = (ushort)(item.UserAlarm?.AlarmID ?? 0);
                                        RegisterItem[i].UserName = item.PID;
                                        i++;
                                    }
                                }
                                else
                                {
                                    RegisterItem.Clear();
                                    var users = items.ToList();
                                    RegisterItem.AddRange(users
                                                    .Select(u =>
                                                    new RegisterData
                                                    {
                                                        Value = (ushort)(u.UserAlarm?.AlarmID ?? 0),
                                                        UserName = u.PID
                                                    }));
                                }
                            }
                            finally
                            {
                                _registerLock.ExitWriteLock();
                            }
                        }
                        else
                        {
                            for (int i = 0; i < RegisterItem.Count; i++)
                            {
                                RegisterItem[i].Value = 0;
                            }
                        }
                    }
                })).RecycleJobImmediately(Settings.Default.AlarmReportIntervalInSeconds * 1000);
            }
            else
            {
                ((Action)(() =>
                {
                    FromCMS();
                })).RecycleJobImmediately(Settings.Default.AlarmReportIntervalInSeconds * 1000);

            }
        }

        private void FromCMS()
        {
            //lock (this)
            //{
            //    if (DateTime.Now - lastUpdateTime < TimeSpan.FromSeconds(Settings.Default.AlarmReportIntervalInSeconds))
            //    {
            //        return; // 如果距離上次更新時間小於設定的間隔，則不進行更新
            //    }
            //    lastUpdateTime = DateTime.Now;
            //}

            using (dnakeDB db = new dnakeDB("dnake"))
            {
                var items = db.GetTable<alarm_logger>()
                        .Where(a => a.confirm == 0)
                        .OrderByDescending(a => a.ts)
                        .ToList();

                var users = db.users.ToList();
                
                try
                {
                    _registerLock.EnterWriteLock();
                    
                    if(RegisterItem.Count != users.Count)
                    {
                        RegisterItem.Clear();
                        RegisterItem.AddRange(users
                                        .Select(u =>
                                        new RegisterData
                                        {
                                            Value = u.online == 1 ? (ushort)0 : ushort.MaxValue,
                                            UserName = u.user_Column
                                        }));
                    }
                    else
                    {
                        for (int i = 0; i < RegisterItem.Count; i++)
                        {
                            RegisterItem[i].Value = users[i].online == 1 ? (ushort)0 : ushort.MaxValue;
                            RegisterItem[i].UserName = users[i].user_Column;
                        }
                    }

                    if (items.Any())
                    {
                        foreach (var item in items)
                        {
                            try
                            {
                                var zone = db.alarm_zone.Where(z => z.user == item.user && z.zone == item.zone).FirstOrDefault();
                                if (zone != null)
                                {
                                    var registerItem = RegisterItem.FirstOrDefault(r => r.UserName == item.user);
                                    if (registerItem != null)
                                    {
                                        if (registerItem.Value == ushort.MaxValue)
                                        {
                                            continue; // 如果用戶已經離線，則不更新寄存器值
                                        }
                                        else
                                        {
                                            registerItem.Value |= (ushort)(1 << item.zone);
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
                finally
                {
                    _registerLock.ExitWriteLock();
                }
            }
        }

        public void Start(int port)
        {
            tcpListener = new TcpListener(IPAddress.Any, port);
            isRunning = true;
            tcpListener.Start();

            // 初始化寄存器值
            for (int i = 0; i < RegisterItem.Count; i++)
            {
                RegisterItem[i].Value = (ushort)0;
            }

            Thread listenerThread = new Thread(ListenForClients);
            listenerThread.Start();

            Console.WriteLine($"Modbus TCP服務器已啟動,監聽端口: {port}");
        }

        private void ListenForClients()
        {
            try
            {
                while (isRunning)
                {
                    TcpClient client = tcpListener.AcceptTcpClient();
                    Thread clientThread = new Thread(HandleClientComm);
                    clientThread.Start(client);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void HandleClientComm(object clientObj)
        {
            TcpClient tcpClient = (TcpClient)clientObj;
            NetworkStream stream = tcpClient.GetStream();

            byte[] buffer = new byte[1024];
            int bytesRead;

            try
            {

                // 從客戶端讀取數據
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    // 處理Modbus請求
                    byte[] response = ProcessModbusRequest(buffer, bytesRead);
                    if (response != null)
                    {
                        stream.Write(response, 0, response.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"處理客戶端通信時發生錯誤: {ex.Message}");
            }
            finally
            {
                tcpClient.Close();
            }
        }

        private byte[] ProcessModbusRequest(byte[] request, int length)
        {
            if (length < 12) // Modbus TCP最小長度
            {
                return null;
            }

            ushort transactionId = (ushort)((request[0] << 8) | request[1]);
            byte functionCode = request[7];
            ushort startAddress = (ushort)((request[8] << 8) | request[9]);
            ushort quantity = (ushort)((request[10] << 8) | request[11]);

            byte[] response;
            if (functionCode == 0x03 || functionCode == 0x04) // 讀取保持寄存器或輸入寄存器
            {
                response =  CreateReadResponse(transactionId, functionCode, startAddress, quantity);
            }
            else
            {
                response = CreateExceptionResponse(transactionId, functionCode, 0x01); // 不支持的功能碼
            }
            Logger.Debug($"Modbus TCP請求: 功能碼={functionCode}, 起始地址={startAddress}, 數量={quantity}\r\n{response.ToHexString(" ")}");
            return response;
        }

        private byte[] CreateReadResponse(ushort transactionId, byte functionCode, ushort startAddress, ushort quantity)
        {
            try
            {
                _registerLock.EnterReadLock();
                
                if (startAddress + quantity > RegisterItem.Count)
                {
                    return CreateExceptionResponse(transactionId, functionCode, 0x02);
                }

                byte[] response = new byte[9 + quantity * 2];

                // MBAP頭
                response[0] = (byte)(transactionId >> 8);
                response[1] = (byte)(transactionId & 0xFF);
                response[2] = 0x00; // 協議標識符
                response[3] = 0x00;
                ushort dataLength = (ushort)(3 + quantity * 2);
                response[4] = (byte)(dataLength >> 8); // 長度
                response[5] = (byte)(dataLength & 0xFF);
                response[6] = 0x01; // 單元標識符

                // PDU
                response[7] = functionCode;
                response[8] = (byte)(quantity * 2); // 字節計數

                // 數據
                for (int i = 0; i < quantity; i++)
                {
                    ushort value = RegisterItem[startAddress + i].Value;
                    response[9 + i * 2] = (byte)(value >> 8);
                    response[10 + i * 2] = (byte)(value & 0xFF);
                }

                return response;
            }
            finally
            {
                _registerLock.ExitReadLock();
            }
        }

        private byte[] CreateExceptionResponse(ushort transactionId, byte functionCode, byte exceptionCode)
        {
            byte[] response = new byte[9];

            // MBAP頭
            response[0] = (byte)(transactionId >> 8);
            response[1] = (byte)(transactionId & 0xFF);
            response[2] = 0x00;
            response[3] = 0x00;
            response[4] = 0x00;
            response[5] = 0x03;
            response[6] = 0x01;

            // PDU
            response[7] = (byte)(functionCode | 0x80); // 錯誤功能碼
            response[8] = exceptionCode;

            return response;
        }

        public void Stop()
        {
            isRunning = false;
            tcpListener.Stop();
            Console.WriteLine("Modbus TCP服務器已停止");
        }

        public void DumpRegisters()
        {
            try
            {
                _registerLock.EnterReadLock();
                
                Console.WriteLine($"住戶警報暫存器:共{RegisterItem.Count}戶");
                //if (Settings.Default.AWTEKMode == false)
                //{
                //    FromCMS();
                //}

                for (int i = 0; i < RegisterItem.Count; i++)
                {
                    Console.WriteLine($"[{i}:{RegisterItem[i].UserName}] = {RegisterItem[i].Value:X02}");
                }
            }
            finally
            {
                _registerLock.ExitReadLock();
            }
        }
    }

    class RegisterData
    {
        public ushort Value { get; set; }
        public String UserName { get; set; }
    }
    //class Program
    //{
    //    static void Main(string[] args)
    //    {
    //        ModbusTcpServer server = new ModbusTcpServer();
    //        server.Start(502); // 標準Modbus TCP端口

    //        Console.WriteLine("按任意鍵停止服務器...");
    //        Console.ReadKey();

    //        server.Stop();
    //    }
    //}
}

