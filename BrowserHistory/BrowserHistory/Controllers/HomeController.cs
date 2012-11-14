﻿using System;
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
            ViewBag.Message = "Welcome to Browser History! Please press about and history links";
            
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult History(string id)
        {
            bool IsAdministrator = true;
            string UserName = "";
            //administrator can see all data...
            if ((!IsAdministrator )&& this.User.Identity.IsAuthenticated )
            {
                UserName = this.User.Identity.Name;
            }
            
            //for administrators, it will be easier to retrieve data for a specific user, if put in URL
            if (string.IsNullOrEmpty(UserName))
            {
                UserName = id;
            }
            var his = new HistoryViewModel();
            his.UserHis = BrowserUserHistoryFilter.AddOrRetrieveFromApplication<BrowserUserHistory>(this.HttpContext.Application).FilterByUser(id).Where(item=>!string.IsNullOrEmpty(item.PageName)).ToList();
            his.rep = BrowserUserHistoryFilter.AddOrRetrieveFromApplication<IBrowserUserHistoryRepository>(this.HttpContext.Application).FilterByUser(id);
            his.rep.Save(his.UserHis);
            return View(his);
        }
    }
}
