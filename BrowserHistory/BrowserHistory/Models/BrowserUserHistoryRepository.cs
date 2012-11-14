using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BrowserHistory.Models
{
    public interface IBrowserUserHistoryRepository
    {
        void Save(BrowserUserHistory history);
        IEnumerable<BrowserUserHistoryData> Retrieve(DateTime date);
        IEnumerable<KeyValuePair<string,int>> MostUsed(int Count, DateTime? date);
    }

    public class BrowserUserHistoryRepositoryMemory : IBrowserUserHistoryRepository
    {
        private  BrowserUserHistory historyMemory;
        public BrowserUserHistoryRepositoryMemory()
        {
            historyMemory = new BrowserUserHistory();
        }

        public void Save(BrowserUserHistory history)
        {
            //adding just new ones
            this.historyMemory.AddRange(history.Where(item => item.IsNew));

            #region marking saved
            
            //for database stuff, just saving in a table with identity will have this id modified
            //here will do by iterating
            int i = 1;
            foreach (var item in history)
            {
                item.Id = i++;
            }
            #endregion
        }

        public IEnumerable<BrowserUserHistoryData> Retrieve(DateTime date)
        {
            return historyMemory.Where(item => item.Date.Subtract(date).Days == 0).ToArray();
        }

        public IEnumerable<KeyValuePair<string,int>> MostUsed(int Count, DateTime? date)
        {
            var data= historyMemory.Where(item =>(date==null || item.Date.Subtract(date.Value).Days == 0)).GroupBy(item=>item.Url).OrderByDescending(g=>g.Count()).Take(Count);
            return data.Select(i =>new KeyValuePair<string,int>( i.Key,i.Count()));
        }
    }
}