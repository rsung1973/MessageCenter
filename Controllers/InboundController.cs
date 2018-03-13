using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebHome.Models.DataEntity;
using WebHome.Models.ViewModel;
using Utility;
using WebHome.DataPort;
using WebHome.Models.Locale;
using Newtonsoft.Json;
using WebHome.Models.Helper;
using WebHome.Properties;
using System.IO;
using WebHome.Helper.Jobs;

namespace WebHome.Controllers
{
    public class InboundController : SampleController<LiveDevice>
    {
        // GET: Inbound
        public ActionResult PushMessage()
        {
            if (Request.TotalBytes <= 0)
                return new EmptyResult();

            dynamic message;

            using (StreamReader reader = new StreamReader(Request.InputStream, Request.ContentEncoding))
            {
                using (JsonTextReader jsonReader = new JsonTextReader(reader))
                {
                    message = JsonSerializer.Create().Deserialize(jsonReader);
                    return Content(JsonConvert.SerializeObject((new TouchLifeDispatcher()).PushMessage(message)), "application/json");
                }
            }
        }

        public ActionResult Echo(EnergyQueryViewModel viewModel)
        {
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }
    }
}