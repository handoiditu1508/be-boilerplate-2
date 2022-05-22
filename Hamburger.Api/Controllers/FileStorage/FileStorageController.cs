using Hamburger.Helpers.Extensions;
using Hamburger.Models.FileStorage;
using Hamburger.Models.Requests.FileStorage;
using Hamburger.Services.Abstractions.FileStorage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hamburger.Api.Controllers.FileStorage
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileStorageController : CustomControllerBase
    {
        private readonly IFileStorageService _fileStorageService;

        public FileStorageController(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }

        [HttpPost(nameof(Upload))]
        public async Task<ActionResult> Upload([FromForm] UploadFileRequest request, IFormFile file)
        {
            try
            {
                var fileBytes = await file.GetBytes();
                request.FileContent = new FileContent
                {
                    Data = fileBytes,
                    FileName = file.FileName
                };
                var path = await _fileStorageService.UploadFile(request);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.ToSimpleError());
            }
        }

        [HttpPost(nameof(UploadMany))]
        public async Task<ActionResult<IEnumerable<string>>> UploadMany([FromForm] UploadFilesRequest request, IEnumerable<IFormFile> files)
        {
            try
            {
                var readBytesTasks = files.Select(async file =>
                {
                    var fileBytes = await file.GetBytes();
                    return new FileContent
                    {
                        Data = fileBytes,
                        FileName = file.FileName
                    };
                });
                var fileContents = await Task.WhenAll(readBytesTasks);
                request.FileContents = fileContents;
                var paths = await _fileStorageService.UploadFiles(request);
                return Ok(paths);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.ToSimpleError());
            }
        }

        [HttpDelete]
        [Route("{**path}")]
        public async Task<ActionResult> DeleteFile(string path)
        {
            try
            {
                await _fileStorageService.DeleteFile(path);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.ToSimpleError());
            }
        }

        [HttpGet]
        [Route("{**path}")]
        public async Task<ActionResult<string>> GetFileContent(string path)
        {
            try
            {
                var content = await _fileStorageService.GetFileContent(path);
                return Ok(content);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.ToSimpleError());
            }
        }
    }
}
