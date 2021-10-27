using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;
using LogAnalyzer.Entities;

namespace LogAnalyzer.Helpers
{
    public class DataContext: DbContext
    {
        public DbSet<LogFile> LogFiles { get; set; }
        public DbSet<LogEntry> LogEntries { get; set; }
        private readonly IConfiguration Configuration;

        public DataContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(Configuration.GetConnectionString("LogAnalyzer"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LogEntry>(entity =>
            {
                entity.ToTable("LogEntries");
            });
            modelBuilder.Entity<LogFile>(entity =>
            {
                entity.ToTable("LogFiles");
            });
        }
    }
}
