using GalaSoft.MvvmLight;

namespace Cassiopeia.Models
{
    internal class Peer : ObservableObject
    {
        private string _address;
        private string _client;
        private int _country;
        private long _downloaded;
        private double _downloadSpeed;
        private double _progress;
        private long _uploaded;
        private double _uploadSpeed;

        public int Country
        {
            get { return _country; }
            set { Set(nameof(Country), ref _country, value); }
        }

        public string Address
        {
            get { return _address; }
            set { Set(nameof(Address), ref _address, value); }
        }

        public string Client
        {
            get { return _client; }
            set { Set(nameof(Client), ref _client, value); }
        }

        public double Progress
        {
            get { return _progress; }
            set { Set(nameof(Progress), ref _progress, value); }
        }

        public double DownloadSpeed
        {
            get { return _downloadSpeed; }
            set { Set(nameof(DownloadSpeed), ref _downloadSpeed, value); }
        }

        public double UploadSpeed
        {
            get { return _uploadSpeed; }
            set { Set(nameof(UploadSpeed), ref _uploadSpeed, value); }
        }

        public long Downloaded
        {
            get { return _downloaded; }
            set { Set(nameof(Downloaded), ref _downloaded, value); }
        }

        public long Uploaded
        {
            get { return _uploaded; }
            set { Set(nameof(Uploaded), ref _uploaded, value); }
        }
    }
}