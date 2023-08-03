using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //test01(args);
            String data = "F123456789";
            byte[] encContent = AppResource.Instance.Encrypt(Encoding.UTF8.GetBytes(data));
            String ID = Encoding.UTF8.GetString(AppResource.Instance.Decrypt(encContent));
            Console.WriteLine(String.Join(" ", encContent.Select(v => $"{v:X02}")));
            Console.WriteLine(ID);
            Console.ReadKey();
        }

        private static void test01(string[] args)
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    //client.Credentials = new NetworkCredential("admin", "123456");
                    client.Headers["Authorization"] = "Basic YWRtaW46MTIzNDU2";
                    client.Encoding = Encoding.UTF8;
                    String uri = args != null && args.Length > 0 ? args[0] : "http://192.168.68.18";
                    HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(uri);
                    wr.Headers["Authorization"] = "Basic YWRtaW46MTIzNDU2";
                    wr.Method = "GET";

                    var stream = wr.GetResponse().GetResponseStream();
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        Console.WriteLine(reader.ReadToEnd());
                    }

                }
                catch (WebException ex)
                {
                    Console.WriteLine(ex);
                    //Console.WriteLine(ex.Response?.GetString(client.Encoding));
                }
            }
        }
    }
}
