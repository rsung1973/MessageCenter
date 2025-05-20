using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

using Newtonsoft.Json;
using Utility;
using WebHome.DataModels;
using WebHome.DataPort;
using WebHome.Helper;
using WebHome.Helper.Jobs;
using WebHome.Models.DataEntity;
using WebHome.Models.Helper;
using WebHome.Models.Locale;
using WebHome.Models.ViewModel;
using WebHome.Properties;

namespace WebHome.Controllers
{
    public class ClientPanelController : SampleController<LiveDevice>
    {
        public ClientPanelController() : base()
        {

        }
    }
}