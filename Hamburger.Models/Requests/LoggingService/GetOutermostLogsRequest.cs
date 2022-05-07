using Hamburger.Models.LoggingService;

namespace Hamburger.Models.Requests.LoggingService
{
    public class GetOutermostLogsRequest : GetLogsRequest
    {
        public OutermostLogsFilterModel FilterModel { get; set; }
    }
}
