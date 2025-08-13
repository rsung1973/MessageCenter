using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonLib.Helper;
using MessageAgent.Helper;
using MessageAgent.Helper.Jobs;
using MessageAgent.Properties;
using Utility;

namespace MessageAgent
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //System.Diagnostics.Debugger.Launch();
            if(Settings.Default.UseJobLauncher)
            {
                JobLauncher.StartUp();
            }
            Logger.Info("程式已啟動...");
            Console.WriteLine("程式已啟動...");

            ModbusTcpServer server = new ModbusTcpServer();
            server.Start(Settings.Default.ModbusServerPort); // 標準Modbus TCP端口

            //Application.Run();
            while (true)
            {
                Console.WriteLine("請輸入=>r:立即執行; d:顯示內容; q:結束");
                var cmd = Console.ReadKey();
                Console.WriteLine();
                if (cmd.Key == ConsoleKey.R)
                {
                    if (Settings.Default.UseJobLauncher)
                    {
                        JobScheduler.LaunchImmediately();
                    }
                }
                else if (cmd.Key == ConsoleKey.D)
                {
                    server.DumpRegisters();
                }
                else if (cmd.Key == ConsoleKey.Q)
                {
                    break;
                }
                //Thread.Yield();
            }

            //        Console.WriteLine("按任意鍵停止服務器...");
            //        Console.ReadKey();

            server.Stop();
        }
    }
}
