using Hamburger.Models.LoggingService;

namespace Hamburger.Models.Requests.LoggingService
{
    public class GetNearbyLogsRequest : GetLogsRequest
    {
        public NearbyLogsFilterModel FilterModel { get; set; }
    }
}
