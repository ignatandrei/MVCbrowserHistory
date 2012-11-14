using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using BrowserHistory.Models;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace BrowserHistorySqlRepository
{
    class BrowserHistoryContext : DbContext
    {
        public DbSet<BrowserUserHistoryData> BrowserUserHistoryTable{ get; set; }

        public BrowserHistoryContext()
            : base()
        {

        }
        public BrowserHistoryContext(string con)
            : base(con)
        {

        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Entity<BrowserUserHistoryData>().ToTable("BrowserUserHistoryData");
            modelBuilder.Entity<BrowserUserHistoryData>().Property(item => item.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<BrowserUserHistoryData>().HasKey(item => item.Id);
            base.OnModelCreating(modelBuilder);
        }
    }
}
