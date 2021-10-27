using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogAnalyzer.Helpers
{
    public class LogParserFactory
    {
        private readonly IServiceProvider _sp;
        public LogParserFactory(IServiceProvider sp)
        {
            _sp = sp;
        }

        public DataContext CreateLogParser()
        {
            return _sp.GetRequiredService<DataContext>();
        }
    }
}
