using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebHome.Models.ViewModel
{
    public class DeviceQueryViewModel
    {
        public String DeviceUri { get; set; }
        public int PageSize { get; set; } = 50;
        public int? PageIndex { get; set; }
        public int? LiveID { get; set; }
    }
}