using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    public sealed class AppResource
    {
        private static AppResource _instance = new AppResource();

        class __KeyResource
        {
            public byte[] Key { get; set; }
            public byte[] IV { get; set; }
            public CipherMode Mode { get; set; }
        }

        public Aes CurrentDES
        {
            get;
            private set;
        }

        private AppResource()
        {
            CurrentDES = Aes.Create();
            InitializeKey();
        }

        public void InitializeKey(bool reset = false)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data");
            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string keyFile = Path.Combine(path, "key.json");

            void saveKeyResource()
            {
                __KeyResource kr = new __KeyResource
                {
                    Key = CurrentDES.Key,
                    IV = CurrentDES.IV,
                    Mode = CurrentDES.Mode,
                };
                File.WriteAllText(keyFile, JsonConvert.SerializeObject(kr));
            }

            if (reset)
            {
                CurrentDES.Dispose();
                CurrentDES = Aes.Create();
                saveKeyResource();
                return;
            }

            if (File.Exists(keyFile))
            {
                try
                {
                    var kr = JsonConvert.DeserializeObject<__KeyResource>(File.ReadAllText(keyFile));
                    CurrentDES.Key = kr.Key;
                    CurrentDES.IV = kr.IV;
                    CurrentDES.Mode = kr.Mode;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    saveKeyResource();
                }
            }
            else
            {
                saveKeyResource();
            }
        }

        public static AppResource Instance
        {
            get
            {
                return _instance;
            }
        }

        public byte[] Encrypt(byte[] data)
        {
            ICryptoTransform xfrm = CurrentDES.CreateEncryptor();
            byte[] outBlock = xfrm.TransformFinalBlock(data, 0, data.Length);
            return outBlock;
        }

        public byte[] Decrypt(byte[] data)
        {
            ICryptoTransform xfrm = CurrentDES.CreateDecryptor();
            byte[] outBlock = xfrm.TransformFinalBlock(data, 0, data.Length);
            return outBlock;
        }
    }
}
