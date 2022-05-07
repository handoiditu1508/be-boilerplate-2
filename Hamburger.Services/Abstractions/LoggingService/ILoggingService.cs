using Hamburger.Models.Requests.LoggingService;
using Hamburger.Models.Responses.LoggingService;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hamburger.Services.Abstractions.LoggingService
{
    public interface ILoggingService
    {
        Task<GetLogsResponse> GetOutermostLogs(GetOutermostLogsRequest request);
        Task<GetLogsResponse> GetNearbyLogs(GetNearbyLogsRequest request);
        Task DeleteCollection(string collectionName);
        Task InsertLogs(InsertLogsRequest request);
        Task InsertLog(InsertLogRequest request);
        Task<IEnumerable<string>> GetCollectionNames();
    }
}
