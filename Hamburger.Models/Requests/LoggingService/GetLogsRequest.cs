namespace Hamburger.Models.Requests.LoggingService
{
    public class GetLogsRequest
    {
        public string CollectionName { get; set; }
        public int Limit { get; set; } = 10;
    }
}
