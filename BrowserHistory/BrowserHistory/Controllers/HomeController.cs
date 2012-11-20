using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BrowserHistory.Models;
using StructureMap;
using BrowserHistorySqlRepository;

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

        
    }
}
