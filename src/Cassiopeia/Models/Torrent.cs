using System.Collections.Generic;

namespace Cassiopeia.Models
{
    internal class Torrent
    {
        public Torrent()
        {
            Files = new List<FileItem>();
            Trackers = new List<string>();
        }

        public string Name { get; set; }
        public long PieceSize { get; set; }
        public List<FileItem> Files { get; set; }
        public List<string> Trackers { get; set; }
        public string Comment { get; set; }
        public string CreatedBy { get; set; }
        public long CreationDate { get; set; }
        public string Encoding { get; set; }
        public bool IsPrivate { get; set; }
    }
}