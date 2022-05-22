using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Hamburger.Helpers.Extensions
{
    public static class FormFileExtension
    {
        public static async Task<byte[]> GetBytes(this IFormFile file)
        {
            byte[] bytes = Array.Empty<byte>();

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);

                bytes = memoryStream.ToArray();
            }

            return bytes;
        }
    }
}
