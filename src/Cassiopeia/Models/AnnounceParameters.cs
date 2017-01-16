using GalaSoft.MvvmLight;

namespace Cassiopeia.Models
{
    internal sealed class AnnounceParameters : ObservableObject
    {
        private long _bytesDownloaded;
        private long _bytesLeft;
        private long _bytesUploaded;
        private TorrentEvent _event;
        private byte[] _infoHash;
        private string _ipAddress;
        private string _peerId;
        private int _port;
        private bool _requiresEncryption;
        private bool _supportsEncryption;
        private bool _useCompactResponse;
        private int _maximumPeerCount;
        private string _userAgent;

        public string UserAgent
        {
            get { return _userAgent; }
            set { Set(nameof(UserAgent), ref _userAgent, value); }
        }

        public long BytesDownloaded
        {
            get { return _bytesDownloaded; }
            set { Set(nameof(BytesDownloaded), ref _bytesDownloaded, value); }
        }

        public long BytesLeft
        {
            get { return _bytesLeft; }
            set { Set(nameof(BytesLeft), ref _bytesLeft, value); }
        }

        public int MaximumPeerCount
        {
            get { return _maximumPeerCount; }
            set { Set(nameof(MaximumPeerCount), ref _maximumPeerCount, value); }
        }

        public long BytesUploaded
        {
            get { return _bytesUploaded; }
            set { Set(nameof(BytesUploaded), ref _bytesUploaded, value); }
        }

        public TorrentEvent Event
        {
            get { return _event; }
            set { Set(nameof(Event), ref _event, value); }
        }

        public byte[] InfoHash
        {
            get { return _infoHash; }
            set { Set(nameof(InfoHash), ref _infoHash, value); }
        }

        public string IpAddress
        {
            get { return _ipAddress; }
            set { Set(nameof(IpAddress), ref _ipAddress, value); }
        }

        public string PeerId
        {
            get { return _peerId; }
            set { Set(nameof(PeerId), ref _peerId, value); }
        }

        public int Port
        {
            get { return _port; }
            set { Set(nameof(Port), ref _port, value); }
        }

        public bool RequiresEncryption
        {
            get { return _requiresEncryption; }
            set { Set(nameof(RequiresEncryption), ref _requiresEncryption, value); }
        }

        public bool SupportsEncryption
        {
            get { return _supportsEncryption; }
            set { Set(nameof(SupportsEncryption), ref _supportsEncryption, value); }
        }

        public bool UseCompactResponse
        {
            get { return _useCompactResponse; }
            set { Set(nameof(UseCompactResponse), ref _useCompactResponse, value); }
        }
    }
}