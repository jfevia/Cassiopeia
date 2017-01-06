using GalaSoft.MvvmLight;

namespace Cassiopeia.Models
{
    internal class Tracker : ObservableObject
    {
        private long _downloaded;
        private string _name;
        private int _peers;
        private int _seeders;
        private TrackerStatus _status;

        public Tracker(string name)
        {
            _name = name;
            _status = TrackerStatus.Working;
        }

        public string Name
        {
            get { return _name; }
            set { Set(nameof(Name), ref _name, value); }
        }

        public TrackerStatus Status
        {
            get { return _status; }
            set { Set(nameof(Status), ref _status, value); }
        }

        public int Seeders
        {
            get { return _seeders; }
            set { Set(nameof(Seeders), ref _seeders, value); }
        }

        public int Peers
        {
            get { return _peers; }
            set { Set(nameof(Peers), ref _peers, value); }
        }

        public long Downloaded
        {
            get { return _downloaded; }
            set { Set(nameof(Downloaded), ref _downloaded, value); }
        }
    }
}