using System.IO;
using System.Threading.Tasks;

namespace Hamburger.Helpers.Extensions
{
    public static class StreamExtension
    {
        public static async Task<string> GetString(this Stream stream)
        {
            if (stream.CanSeek)
                stream.Position = 0;

            string text;
            using (var reader = new StreamReader(stream))
            {
                text = await reader.ReadToEndAsync();
            }
            return text;
        }
    }
}
