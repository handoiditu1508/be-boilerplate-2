using Hamburger.Models.LoggingService;
using System.Collections.Generic;

namespace Hamburger.Models.Requests.LoggingService
{
    public class InsertLogsRequest
    {
        public IEnumerable<LoggingModel> Logs { get; set; }
        public string CollectionName { get; set; }
    }
}
