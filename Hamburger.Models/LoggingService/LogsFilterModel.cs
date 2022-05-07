using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Hamburger.Models.LoggingService
{
    public class LogsFilterModel
    {
        public string Group { get; set; }
        public IEnumerable<LogLevel> LogLevels { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
