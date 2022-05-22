using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hamburger.Models.Requests.FileStorage
{
    public class DeleteFileRequest
    {
        public string Path { get; set; }
        public bool? IsFile { get; set; }
    }
}
