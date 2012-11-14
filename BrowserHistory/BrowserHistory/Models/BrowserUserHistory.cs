using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BrowserHistory.Models
{
    public class BrowserUserHistoryData
    {
        
        public BrowserUserHistoryData()
        {

        }
        public BrowserUserHistoryData(string Url, string UserName)
        {
            this.Url = Url;
            this.UserName = UserName;
            this.Date = DateTime.Now;
        }
        public BrowserUserHistoryData(string Url)
        {
            HttpContext con = System.Web.HttpContext.Current;
            //TODO: for non web applications, verify con for null...
            string username = (con.User.Identity.IsAuthenticated ? con.User.Identity.Name : con.Session.SessionID);
            this.Url = Url;
            this.UserName = username;
            this.Date = DateTime.Now;
        }

        #region database saving stuff
        
        /// <summary>
        /// to be used just for databse related stuff
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// to be used just for databse related stuff
        /// </summary>
        public bool IsNew
        {
            get
            {
                return Id == 0;
            }
        }
        #endregion
        public string Url { get; set; }
        public DateTime Date{ get; set; }
        public string UserName { get; set; }
        public string PageName;

        public string UniqueKey
        {
            get
            {
                return Url + "_" + Date.ToString("yyyyMMHHmmssfff") + "_" + UserName;
            }
        }
    }

    public class BrowserUserHistory:List<BrowserUserHistoryData> 
    {
        
        public BrowserUserHistory()
            :base()
        {
            
        }
        public BrowserUserHistory(IEnumerable<BrowserUserHistoryData> data)
            : base(data)
        {

        }
        
        public void Add(string Url, string UserName)
        {
            this.Add(new BrowserUserHistoryData(Url, UserName));
        }
        public void Add(string Url)
        {
            this.Add(new BrowserUserHistoryData(Url));
        }

        public void SetName(string Url, string Name)
        {
            this.ForEach(
                item =>
                {
                    if (item.Url == Url)
                        item.PageName = Name;
                });
        }


        
    }
}