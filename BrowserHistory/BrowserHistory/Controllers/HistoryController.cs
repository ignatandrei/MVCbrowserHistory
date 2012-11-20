using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BrowserHistory.Models;
using BrowserHistorySqlRepository;
using StructureMap;

namespace BrowserHistory.Controllers
{
    public class HistoryController : Controller
    {
        //
        // GET: /History/

        public ActionResult Index(string id)
        {
        
            bool IsAdministrator = true;
            string UserName = "";
            //administrator can see all data...
            if ((!IsAdministrator) && this.User.Identity.IsAuthenticated)
            {
                UserName = this.User.Identity.Name;
            }

            //for administrators, it will be easier to retrieve data for a specific user, if put in URL
            if (string.IsNullOrEmpty(UserName))
            {
                UserName = id;
            }
            var his = new HistoryViewModel();
            his.UserHis = BrowserUserHistoryFilter.AddOrRetrieveFromApplication<BrowserUserHistory>(this.HttpContext.Application).FilterByUser(id).Where(item => !string.IsNullOrEmpty(item.PageName)).ToList();
            his.rep = BrowserUserHistoryFilter.AddOrRetrieveFromApplication<IBrowserUserHistoryRepository>(this.HttpContext.Application).FilterByUser(id);
            his.rep.Save(his.UserHis);
            return View(his);
        }
        public JsonResult SetRepository(int id)
        {
            try
            {
                string Message = "";
                switch (id)
                {
                    default:
                    case 1:
                        ObjectFactory.EjectAllInstancesOf<IBrowserUserHistoryRepository>();
                        ObjectFactory.Initialize(initializationExpression => { });
                        ObjectFactory.Configure(ce => ce.For<IBrowserUserHistoryRepository>().Use<BrowserUserHistoryRepositoryMemory>());
                        Message = "now use memory";
                        break;
                    case 2:
                        ObjectFactory.EjectAllInstancesOf<IBrowserUserHistoryRepository>();
                        ObjectFactory.Initialize(initializationExpression => { });
                        ObjectFactory.Configure(ce => ce.For<IBrowserUserHistoryRepository>().Use<BrowserUserHistoryRepositorySqlServer>());
                        Message = "now use sql server";
                        break;

                }

                BrowserUserHistoryFilter.ClearData(this.ControllerContext);
                return Json(new { ok = true, message = Message });
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, message = ex.Message });
            }
        }
    }
}
