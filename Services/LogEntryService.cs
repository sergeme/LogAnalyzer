using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using LogAnalyzer.Entities;
using LogAnalyzer.Helpers;

namespace LogAnalyzer.Services
{
    public interface ILogEntryService
    {
        IEnumerable<LogEntry> GetLogEntries();

    }
    public class LogEntryService : ILogEntryService
    {
        private readonly DataContext _context;

        public LogEntryService(DataContext context)
        {
            _context = context;
        }

        //return specific amount of records with offset
        public IEnumerable<LogEntry> GetLogEntries()
        {
            List<LogEntry> logEntries = _context.LogEntries
                .Select(l => new LogEntry
                {
                    Id = l.Id,
                    ClientIP = l.ClientIP.Trim(),
                    ClientFQDN = l.ClientFQDN.Trim(),
                    Visits = l.Visits
                })
                .OrderBy(l => l.Id).ToList();
            return logEntries;
        }
    }
}
