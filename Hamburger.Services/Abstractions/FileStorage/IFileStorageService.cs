using Hamburger.Models.Requests.FileStorage;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hamburger.Services.Abstractions.FileStorage
{
    public interface IFileStorageService
    {
        Task<string> UploadFile(UploadFileRequest request);
        Task<IEnumerable<string>> UploadFiles(UploadFilesRequest request);
        Task DeleteFile(string path, bool? isFile = null);
        Task DeleteFiles(DeleteFilesRequest request);
    }
}
