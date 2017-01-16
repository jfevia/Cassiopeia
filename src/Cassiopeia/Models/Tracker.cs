using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace Cassiopeia.Models
{
    internal abstract class Tracker : ObservableObject
    {
        private long _downloaded;
        private Uri _uri;
        private int _peers;
        private int _seeders;
        private TrackerStatus _status;

        protected Tracker(Uri uri)
        {
            _uri = uri;
            _status = TrackerStatus.Working;
        }

        public Uri Uri
        {
            get { return _uri; }
            set { Set(nameof(Uri), ref _uri, value); }
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

        public abstract void AnnounceAsync(AnnounceParameters parameters);
        public abstract void ScrapeAsync();
    }
}