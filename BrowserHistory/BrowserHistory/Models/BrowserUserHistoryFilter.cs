using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StructureMap;

namespace BrowserHistory.Models
{
    public class BrowserUserHistoryFilter:IActionFilter, IResultFilter 
    {
        private const int SaveToRepositoryInterval=3;
        static void ClearFromApplication<T>(HttpApplicationStateBase app)
        {
            var type = typeof(T);
            string key = type.FullName;
            app.Remove(key);
        }
        public static T AddOrRetrieveFromApplication<T>(HttpApplicationStateBase app)
            //where T:new()
        {
            
            var type = typeof(T);
            string key=type.FullName;

            if (app.AllKeys.Contains(key)) 
            {
                var ret = (T)app[key];
                if (ret != null)
                    return ret;
            }
            T result;
            if (typeof(T).IsInterface)
            {
                result =(T) ObjectFactory.GetInstance(type) ;
            }
            else
            {
                result = (T)Activator.CreateInstance(type);
            }
            //var result = new T();
            try
            {
                app.Lock();
                if(!app.AllKeys.Contains(key))
                    app[key] = result;
            }
            finally
            {
                app.UnLock();
            }
            
            return result;
        
        }

        public static void ClearData(ControllerContext context)
        {
            var app = context.HttpContext.Application;
            ClearFromApplication<BrowserUserHistory>(app);
            ClearFromApplication<IBrowserUserHistoryRepository>(app);
        }
        BrowserUserHistory history(ControllerContext context)
        {
            var app = context.HttpContext.Application;
            return AddOrRetrieveFromApplication<BrowserUserHistory>(app);
        }
        IBrowserUserHistoryRepository SaveRepository(ControllerContext context)
        {
            var app = context.HttpContext.Application;
            //TODO: configure here the repository
            return AddOrRetrieveFromApplication<IBrowserUserHistoryRepository>(app);
        }
        string Url(ControllerContext context)
        {
            return context.HttpContext.Request.Url.ToString();
        }
        
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.IsChildAction)
                return;
            if (filterContext.HttpContext.Request.IsAjaxRequest())
                return;

            history(filterContext).Add(this.Url(filterContext));

            
        }
        
        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
            if (filterContext.IsChildAction)
                return;
            if (filterContext.HttpContext.Request.IsAjaxRequest())
                return;

            var vr = filterContext.Result as ViewResult;
            if (vr == null)
                return;
            var his=history(filterContext);
            his.SetName(this.Url(filterContext), vr.ViewName);
            if (his.Count % SaveToRepositoryInterval == 0)
            {
                SaveRepository(filterContext).Save(his);
            }
        }

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
           
        }
    }
}