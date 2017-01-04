using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Cassiopeia.Models
{
    internal class Sessions
    {
        public Sessions(string name, IEnumerable<SessionCategory> sessionCategories)
        {
            Name = name;
            SessionCategories = new ObservableCollection<SessionCategory>(sessionCategories);
        }

        public ObservableCollection<SessionCategory> SessionCategories { get; }
        public string Name { get; }
    }
}