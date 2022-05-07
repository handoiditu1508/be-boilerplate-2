using Hamburger.Models.LoggingService;

namespace Hamburger.Models.Requests.LoggingService
{
    public class InsertLogRequest
    {
        public LoggingModel Log { get; set; }
        public string CollectionName { get; set; }
    }
}
