using Hamburger.Models.FileStorage;
using System.Collections.Generic;

namespace Hamburger.Models.Requests.FileStorage
{
    public class UploadFilesRequest
    {
        public string DestinationFolder { get; set; }

        public IEnumerable<FileContent> FileContents { get; set; }
    }
}
