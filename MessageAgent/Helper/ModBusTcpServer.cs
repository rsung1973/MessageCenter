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

namespace MessageAgent.Helper
{
    public class ModbusTcpServer
    {
        private TcpListener tcpListener;
        private bool isRunning = false;
        private ushort[] registers = new ushort[16]; // 16個整數值陣列
        private String[] userNames = new String[16]; // 16個用戶名稱陣列

        public ModbusTcpServer() 
        {
            ((Action)(() => 
            {
                using(ModelSource<UserProfile> models = new ModelSource<UserProfile>())
                {
                    var items = models.GetTable<UserProfile>();
                    if(items.Any())
                    {
                        if(registers?.Length == items.Count())
                        {
                            int i = 0;
                            foreach(var item in items)
                            {
                                registers[i] = (ushort)(item.UserAlarm?.AlarmID ?? 0);
                                userNames[i] = item.PID;
                                i++;
                            }
                        }
                        else
                        {
                            var users = items.ToList();
                            registers = users
                                            .Select(u => (ushort)(u.UserAlarm?.AlarmID ?? 0))
                                            .ToArray();
                            userNames = users.Select(u => u.PID).ToArray();
                        }
                    }
                    else
                    {
                        for(int i = 0; i < registers.Length; i++)
                        {
                            registers[i] = 0;
                        }
                    }
                }
            })).RecycleJobImmediately(Settings.Default.AlarmReportIntervalInSeconds * 1000);
        }

        public void Start(int port)
        {
            tcpListener = new TcpListener(IPAddress.Any, port);
            isRunning = true;
            tcpListener.Start();

            // 初始化寄存器值
            for (int i = 0; i < registers.Length; i++)
            {
                registers[i] = (ushort)0;
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

            byte[] response = null;
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
            if (startAddress + quantity > registers.Length)
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
                ushort value = registers[startAddress + i];
                response[9 + i * 2] = (byte)(value >> 8);
                response[10 + i * 2] = (byte)(value & 0xFF);
            }

            return response;
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
            Console.WriteLine($"住戶警報暫存器:共{registers.Length}戶");
            for (int i = 0; i < registers.Length; i++)
            {
                Console.WriteLine($"[{i}:{userNames[i]}] = {registers[i]:X02}");
            }
        }
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

