using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using WKPdfWrapper.Properties;
using CommonLib.PlugInAdapter;

namespace WKPdfWrapper
{
    public class PdfUtility : IPdfUtility
    {

        public void ConvertHtmlToPDF(string htmlFile, string pdfFile, double timeOutInMinute)
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = Path.Combine(AppDomain.CurrentDomain.RelativeSearchPath, "wkhtmltopdf.exe");
            info.Arguments = " " + Settings.Default.Options + " " + htmlFile + " " + pdfFile;
            //info.CreateNoWindow = true;
            info.UseShellExecute = false;
            info.WindowStyle = ProcessWindowStyle.Hidden;
            info.WorkingDirectory = AppDomain.CurrentDomain.RelativeSearchPath;

            Process proc = new Process();
            proc.EnableRaisingEvents = true;
            //proc.Exited += new EventHandler(proc_Exited);

            //if (null != _eventHandler)
            //{
            //    proc.Exited += new EventHandler(_eventHandler);
            //}
            proc.StartInfo = info;
            proc.Start();
            proc.WaitForExit((int)timeOutInMinute * 60000);

        }
    }
}
