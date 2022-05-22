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
        private readonly string _subUrl;

        public FileStorageService(IHttpHelper httpHelper)
        {
            _httpHelper = httpHelper;
            _httpHelper.SetBaseUrl(AppSettings.FileStorage.BaseUrl);
            _httpHelper.UseApiKeyAuthentication("X-API-Key", AppSettings.FileStorage.ApiKey);
            _subUrl = "/api/FilesStorage";
        }

        public async Task<string> UploadFile(UploadFileRequest request)
        {
            await _httpHelper.Post(_subUrl, request);

            return Path.Combine(AppSettings.FileStorage.BaseUrl, _subUrl, request.DestinationFolder, request.FileContent.FileName).Replace("\\", "/");
        }

        public async Task<IEnumerable<string>> UploadFiles(UploadFilesRequest request)
        {
            await _httpHelper.Post($"{_subUrl}/StoreFiles", request);

            return request.FileContents.Select(f => Path.Combine(AppSettings.FileStorage.BaseUrl, _subUrl, request.DestinationFolder, f.FileName).Replace("\\", "/"));
        }

        public async Task DeleteFile(string path)
        {
            var fullUrl = Path.Combine(_subUrl, path).Replace("\\", "/");

            await _httpHelper.Delete(fullUrl);
        }

        public async Task DeleteFiles(DeleteFilesRequest request)
        {
            await _httpHelper.Post($"{_subUrl}/DeleteFiles", request);
        }
    }
}
