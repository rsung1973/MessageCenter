using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebHome.Models.ViewModel
{
    public class DeviceQueryViewModel  : QueryViewModel
    {
        public String DeviceUri { get; set; }
        public int? LiveID { get; set; }
    }

    public class QueryViewModel
    {
        public int PageSize { get; set; } = 50;
        public int? PageIndex { get; set; }
    }

    public class InfoQueryViewModel : QueryViewModel
    {
        public String ResidentID { get; set; }
    }

    public class EnergyQueryViewModel : InfoQueryViewModel
    {
        public int EnergyType { get; set; }
        public int? Degree { get; set; } = 0;
        public int? Year { get; set; } = DateTime.Today.Year;
        public int? Month { get; set; } = DateTime.Today.AddMonths(-1).Month;
    }

}