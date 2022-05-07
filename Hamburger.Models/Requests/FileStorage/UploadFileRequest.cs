using Hamburger.Models.FileStorage;

namespace Hamburger.Models.Requests.FileStorage
{
    public class UploadFileRequest
    {
        public string DestinationFolder { get; set; }

        public FileContent FileContent { get; set; }
    }
}
