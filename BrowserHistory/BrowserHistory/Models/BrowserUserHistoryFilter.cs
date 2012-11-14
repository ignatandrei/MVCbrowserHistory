using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BrowserHistory.Models
{
    public class BrowserUserHistoryFilter:IActionFilter, IResultFilter 
    {
        private const int SaveToRepositoryInterval=3;
        public static T AddOrRetrieveFromApplication<T>(HttpApplicationStateBase app)
            where T:new()
        {
            string key=typeof(T).FullName;

            if (app.AllKeys.Contains(key)) 
            {
                return (T)app[key];
            }

            var result = new T();
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
        public static string key = "BrowserUserHistory";
        BrowserUserHistory history(ControllerContext context)
        {
            var app = context.HttpContext.Application;
            return AddOrRetrieveFromApplication<BrowserUserHistory>(app);
        }
        IBrowserUserHistoryRepository SaveRepository(ControllerContext context)
        {
            var app = context.HttpContext.Application;
            return AddOrRetrieveFromApplication<BrowserUserHistoryRepositoryMemory>(app);
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

            ViewResult vr = filterContext.Result as ViewResult;
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