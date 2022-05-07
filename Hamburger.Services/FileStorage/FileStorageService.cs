using Hamburger.Helpers;
using Hamburger.Helpers.Abstractions;
using Hamburger.Models.Requests.FileStorage;
using Hamburger.Services.Abstractions.FileStorage;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Hamburger.Services.FileStorage
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IHttpHelper _httpHelper;

        public FileStorageService(IHttpHelper httpHelper)
        {
            _httpHelper = httpHelper;
            _httpHelper.SetBaseUrl(AppSettings.FileStorage.BaseUrl);
            _httpHelper.UseApiKeyAuthentication("X-API-Key", AppSettings.FileStorage.ApiKey);
        }

        public async Task<string> UploadFile(UploadFileRequest request)
        {
            await _httpHelper.Post("/StoreFile", request);

            return Path.Combine(AppSettings.FileStorage.BaseUrl, request.DestinationFolder, request.FileContent.FileName);
        }

        public async Task<IEnumerable<string>> UploadFiles(UploadFilesRequest request)
        {
            await _httpHelper.Post("/StoreFiles", request);

            return request.FileContents.Select(f => Path.Combine(AppSettings.FileStorage.BaseUrl, request.DestinationFolder, f.FileName));
        }

        public async Task DeleteFile(string path)
        {
            await _httpHelper.Delete(path);
        }

        public async Task DeleteFiles(DeleteFilesRequest request)
        {
            await _httpHelper.Post("/DeleteFiles", request);
        }
    }
}
