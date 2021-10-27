using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LogAnalyzer.Entities;
using LogAnalyzer.Services;

namespace LogAnalyzer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LogEntriesController : ControllerBase
    {
        private readonly ILogEntryService _logEntryService;

        public LogEntriesController(ILogEntryService logEntryService)
        {
            _logEntryService = logEntryService;
        } 

        [HttpGet]
        public ActionResult<LogEntry> GetLogEntries()
        {
            var logEntries = _logEntryService.GetLogEntries();
            return Ok(logEntries);
        }
    }
}
