using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BrowserHistory.Models;

namespace BrowserHistory.Controllers
{
    
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";
            
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult History()
        {
            var his = new HistoryViewModel();
            his.UserHis = BrowserUserHistoryFilter.AddOrRetrieveFromApplication<BrowserUserHistory>(this.HttpContext.Application);
            his.rep = BrowserUserHistoryFilter.AddOrRetrieveFromApplication<BrowserUserHistoryRepositoryMemory>(this.HttpContext.Application);
            return View(his);
        }
    }
}
