using Microsoft.Extensions.Logging;
using System;

namespace Hamburger.Models.LoggingService
{
    public class LoggingModel
    {
        public string Id { get; set; }
        public DateTime? CreatedDate { get; set; }
        public LogLevel LogLevel { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string Source { get; set; }
        public string Group { get; set; }
        public string Code { get; set; }
    }
}
