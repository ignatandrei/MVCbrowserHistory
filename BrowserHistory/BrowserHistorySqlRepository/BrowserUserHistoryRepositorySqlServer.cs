using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrowserHistory.Models;

namespace BrowserHistorySqlRepository
{
    public class BrowserUserHistoryRepositorySqlServer : IBrowserUserHistoryRepository
    {
        string UserName;
        public void Save(IEnumerable<BrowserUserHistoryData> history)
        {
            using (var bhc = new BrowserHistoryContext())
            {
                foreach (var item in history.Where(item => item.IsNew))
                    bhc.BrowserUserHistoryTable.Add(item);

                bhc.SaveChanges();

            }
        }

        public IEnumerable<BrowserUserHistoryData> Retrieve(DateTime date)
        {
            var datePrev=date.Date;
            var dateNext=datePrev.AddDays(1);
            using (var bhc = new BrowserHistoryContext())
            {
                IQueryable < BrowserUserHistoryData > ret = bhc.BrowserUserHistoryTable.Where(item => item.Date >= datePrev && item.Date < dateNext);
                if (UserName != null)
                    ret = ret.Where(item => item.UserName == UserName);

                return ret.ToArray();
            }
        }

        public IEnumerable<KeyValuePair<string, int>> MostUsed(int Count, DateTime? date)
        {

            
            using (var bhc = new BrowserHistoryContext())
            {
                IQueryable<BrowserUserHistoryData> data=bhc.BrowserUserHistoryTable;
                if(date != null)
                {
                    var datePrev = date.Value.Date;
                    var dateNext = datePrev.AddDays(1);
                    data=data.Where(item => item.Date >= datePrev && item.Date < dateNext);
                }
                if (UserName != null)
                    data = data.Where(item => item.UserName == UserName);

                var items = data.GroupBy(item=>item.Url).OrderByDescending(g=>g.Count()).Take(Count).ToArray();
                return items.Select(i => new KeyValuePair<string, int>(i.Key, i.Count()));
            }
            
        }

        public IBrowserUserHistoryRepository FilterByUser(string UserName)
        {
            var data = new BrowserUserHistoryRepositorySqlServer();
            data.UserName = UserName;
            return data;
            
        }
    }
}
