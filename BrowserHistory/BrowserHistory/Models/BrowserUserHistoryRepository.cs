using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BrowserHistory.Models
{
    public interface IBrowserUserHistoryRepository
    {
        void Save(IEnumerable<BrowserUserHistoryData> history);
        IEnumerable<BrowserUserHistoryData> Retrieve(DateTime date);
        IEnumerable<KeyValuePair<string,int>> MostUsed(int Count, DateTime? date);
        IBrowserUserHistoryRepository FilterByUser(string UserName);
    }

    public class BrowserUserHistoryRepositoryMemory : IBrowserUserHistoryRepository
    {
        private  BrowserUserHistory historyMemory;
        private BrowserUserHistoryRepositoryMemory(IEnumerable<BrowserUserHistoryData> history)
            : this()
        {
            historyMemory.AddRange(history);
        }
        public BrowserUserHistoryRepositoryMemory()
        {
            historyMemory = new BrowserUserHistory();
        }

        public void Save(IEnumerable<BrowserUserHistoryData> history)
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
        public IBrowserUserHistoryRepository FilterByUser(string UserName)
        {
            if (string.IsNullOrEmpty(UserName))
                return this;

            var data = historyMemory.Where(item => string.Compare(UserName, item.UserName, StringComparison.CurrentCultureIgnoreCase) == 0).ToArray();
            return new BrowserUserHistoryRepositoryMemory(data);
        }
    }
}