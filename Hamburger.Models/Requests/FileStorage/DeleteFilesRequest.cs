using System.Collections.Generic;

namespace Hamburger.Models.Requests.FileStorage
{
    public class DeleteFilesRequest
    {
        public IEnumerable<DeleteFileRequest> Files { get; set; }
    }
}
