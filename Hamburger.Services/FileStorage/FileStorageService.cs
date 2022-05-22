using Hamburger.Helpers;
using Hamburger.Helpers.Abstractions;
using Hamburger.Helpers.Extensions;
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
        private readonly string _subPath;

        public FileStorageService(IHttpHelper httpHelper)
        {
            _httpHelper = httpHelper;
            _httpHelper.SetBaseUrl(AppSettings.FileStorage.BaseUrl);
            _httpHelper.UseApiKeyAuthentication("X-API-Key", AppSettings.FileStorage.ApiKey);
            _subPath = "/api/FilesStorage";
        }

        public async Task<string> UploadFile(UploadFileRequest request)
        {
            await _httpHelper.Post(_subPath, request);

            return Path.Combine(AppSettings.FileStorage.BaseUrl, _subPath, request.DestinationFolder, request.FileContent.FileName).Replace("\\", "/");
        }

        public async Task<IEnumerable<string>> UploadFiles(UploadFilesRequest request)
        {
            await _httpHelper.Post($"{_subPath}/StoreFiles", request);

            return request.FileContents.Select(f => Path.Combine(AppSettings.FileStorage.BaseUrl, _subPath, request.DestinationFolder, f.FileName).Replace("\\", "/"));
        }

        public async Task DeleteFile(string path)
        {
            var fullUrl = Path.Combine(_subPath, path).Replace("\\", "/");

            await _httpHelper.Delete(fullUrl);
        }

        public async Task DeleteFiles(DeleteFilesRequest request)
        {
            await _httpHelper.Post($"{_subPath}/DeleteFiles", request);
        }

        public async Task<Stream> GetFileStream(string path)
        {
            var fullUrl = Path.Combine(_subPath, path).Replace("\\", "/");

            return await _httpHelper.GetStreamResult(fullUrl);
        }

        public async Task<string> GetFileContent(string path)
        {
            string text;

            using (var stream = await GetFileStream(path))
            {
                text = await stream.GetString();
            }

            return text;
        }
    }
}
