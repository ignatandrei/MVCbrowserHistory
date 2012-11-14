using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BrowserHistory.Models
{
    public class HistoryViewModel
    {
        public List<BrowserUserHistoryData> UserHis;
        public IBrowserUserHistoryRepository rep;
    }
}