using System.Collections.Generic;

namespace Hamburger.Models.Requests.FileStorage
{
    public class DeleteFilesRequest
    {
        public IEnumerable<string> Paths { get; set; }
    }
}
