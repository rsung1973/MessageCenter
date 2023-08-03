using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;

namespace WebHome.Helper
{
    public static partial class ExtensionMethods
    {
        public static String DecodeValue(this String base64Value)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(base64Value));
        }

        public static JArray DecodeValue(this JArray result)
        {
            foreach (JToken o in result)
            {
                foreach (JProperty p in o.Children())
                {
                    p.Value = p.Value.ToString().DecodeValue();
                }
            }
            return result;
        }

        public static ClosedXML.Excel.XLWorkbook ConvertToExcel(this DataSet ds)
        {
            ClosedXML.Excel.XLWorkbook xls = new ClosedXML.Excel.XLWorkbook();
            xls.Worksheets.Add(ds);
            return xls;
        }
    }
}