using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Utility;
using WebHome.Helper;
using WebHome.Models.DataEntity;
//using WebHome.Models.Locale;
//using WebHome.Models.ViewModel;

namespace WebHome.Controllers
{
    public class SampleController<TEntity> : Controller
        where TEntity : class, new()
    {
        protected ModelSource<TEntity> models;
        protected bool _dbInstance;

        private String _requestBody;
        public String RequestBody
        {
            get
            {
                if (_requestBody == null)
                {
                    Request.InputStream.Seek(0, SeekOrigin.Begin);
                    using (StreamReader reader = new StreamReader(Request.InputStream, Request.ContentEncoding))
                    {
                        _requestBody = reader.ReadToEnd();
                    }
                }
                return _requestBody;
            }
        }


        protected SampleController() :base()
        {
            models = TempData["__DB_Instance"] as ModelSource<TEntity>;
            if (models == null)
            {
                models = new ModelSource<TEntity>();
                _dbInstance = true;
                TempData["__DB_Instance"] = models;
            }
            
        }

        public ModelSource<TEntity> DataSource
        {
            get
            {
                return models;
            }
        }

        protected override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
            if (_dbInstance)
                models.Dispose();
        }

        protected override void HandleUnknownAction(string actionName)
        {
            HttpContext.Items["Models"] = models;

            if (!String.IsNullOrEmpty(actionName))
            {
                if (actionName.EndsWith(".aspx", StringComparison.InvariantCultureIgnoreCase))
                {
                    this.View(actionName.Substring(0, actionName.Length - 5)).ExecuteResult(this.ControllerContext);
                }
                else
                {
                    var module = ViewEngineCollection.FindPartialView(this.ControllerContext, $"~/Views/{RouteData.Values["controller"]}/Module/{RouteData.Values["action"]}.cshtml");
                    if (module?.View != null)
                    {
                        this.View(module.View).ExecuteResult(this.ControllerContext);
                    }
                    else
                    {
                        this.View(actionName).ExecuteResult(this.ControllerContext);
                    }
                }
            }
        }

        public TModel BuildViewModel<TModel>(TModel model)
            where TModel : class
        {
            this.UpdateModel<TModel>(model);
            return model;
        }

        public TModel FromJsonBody<TModel>()
            where TModel : class
        {
            return JsonConvert.DeserializeObject<TModel>(RequestBody);
        }


    }
}