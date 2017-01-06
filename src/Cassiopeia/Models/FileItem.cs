using System.Collections.Generic;

namespace Cassiopeia.Models
{
    internal class FileItem
    {
        public FileItem()
        {
            Path = new List<string>();
        }

        public long Size { get; set; }
        public List<string> Path { get; set; }
        public string Md5Sum { get; set; }
    }
}