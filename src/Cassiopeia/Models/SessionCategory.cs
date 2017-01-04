using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Cassiopeia.Models
{
    internal class SessionCategory
    {
        public SessionCategory(string name, IEnumerable<Torrent> torrents)
        {
            Name = name;
            Torrents = new ObservableCollection<Torrent>(torrents);
        }

        public ObservableCollection<Torrent> Torrents { get; }
        public string Name { get; }
    }
}