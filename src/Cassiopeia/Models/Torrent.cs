using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;

namespace Cassiopeia.Models
{
    internal class Torrent : ObservableObject
    {
        private readonly ObservableCollection<FileItem> _files;
        private readonly ObservableCollection<HttpSeed> _httpSeeds;
        private readonly ObservableCollection<Node> _nodes;
        private readonly ObservableCollection<Peer> _peers;
        private readonly ObservableCollection<Seed> _seeds;
        private readonly ObservableCollection<Tracker> _trackers;
        private DateTime _addedDate;
        private int _additionalSlotsLowUploadSpeedPercent;
        private string _category;
        private string _comment;
        private string _commentUtf8;
        private string _completedDownloadFolder;
        private string _createdBy;
        private long _creationDate;
        private long _downloaded;
        private string _downloadFolder;
        private long _downloadSize;
        private double _downloadSpeed;
        private bool _enabledDht;
        private string _encoding;
        private TimeSpan _eta;
        private byte[] _infoHash;
        private InitialQueuePosition _initialPosition;
        private bool _initialSeeding;
        private bool _isPrivate;
        private bool _localPeerDiscovery;
        private int _maximumConnectedPeers;
        private int _maximumDownloadRate;
        private int _maximumUploadRate;
        private int _maximumUploadSlots;
        private bool _moveCompletedDownload;
        private string _name;
        private long _originalSize;
        private bool _peerExchange;
        private long _pieceSize;
        private double _progress;
        private string _publisherUrl;
        private string _publisherUrlUtf8;
        private double _ratio;
        private long _size;
        private bool _skipHashCheck;
        private bool _startTorrent;
        private TorrentStatus _status;
        private long _uploaded;
        private double _uploadSpeed;
        private bool _useAdditionalSlotsOnLowUploadSpeed;
        private byte[] _pieces;
        private int _pieceCount;

        public Torrent()
        {
            _files = new ObservableCollection<FileItem>();
            _trackers = new ObservableCollection<Tracker>();
            _peers = new ObservableCollection<Peer>();
            _seeds = new ObservableCollection<Seed>();
            _httpSeeds = new ObservableCollection<HttpSeed>();
            _nodes = new ObservableCollection<Node>();
            _startTorrent = true;
            _enabledDht = true;
            _peerExchange = true;
            _localPeerDiscovery = true;
            _status = TorrentStatus.Paused;
            _addedDate = DateTime.Now;
            _infoHash = new byte[0];
        }

        public long Size
        {
            get { return _size; }
            set { Set(nameof(Size), ref _size, value); }
        }

        public byte[] InfoHash
        {
            get { return _infoHash; }
            set { Set(nameof(InfoHash), ref _infoHash, value); }
        }

        public TimeSpan Eta
        {
            get { return _eta; }
            set { Set(nameof(Eta), ref _eta, value); }
        }

        public TorrentStatus Status
        {
            get { return _status; }
            set { Set(nameof(Status), ref _status, value); }
        }

        public DateTime AddedDate
        {
            get { return _addedDate; }
            set { Set(nameof(AddedDate), ref _addedDate, value); }
        }

        public string Name
        {
            get { return _name; }
            set { Set(nameof(Name), ref _name, value); }
        }

        public ReadOnlyCollection<Peer> Peers
        {
            get { return new ReadOnlyCollection<Peer>(_peers); }
        }

        public int PeerCount
        {
            get { return _peers.Count; }
        }

        public int SeedCount
        {
            get { return _seeds.Count; }
        }

        public int TrackerCount
        {
            get { return _trackers.Count; }
        }

        public int FileCount
        {
            get { return _files.Count; }
        }

        public ReadOnlyCollection<Seed> Seeds
        {
            get { return new ReadOnlyCollection<Seed>(_seeds); }
        }

        public long Uploaded
        {
            get { return _uploaded; }
            set { Set(nameof(Uploaded), ref _uploaded, value); }
        }

        public long Downloaded
        {
            get { return _downloaded; }
            set { Set(nameof(Downloaded), ref _downloaded, value); }
        }

        public double Progress
        {
            get { return _progress; }
            set { Set(nameof(Progress), ref _progress, value); }
        }

        public double Ratio
        {
            get { return _ratio; }
            set { Set(nameof(Ratio), ref _ratio, value); }
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

        public long PieceSize
        {
            get { return _pieceSize; }
            set { Set(nameof(PieceSize), ref _pieceSize, value); }
        }

        public ReadOnlyObservableCollection<FileItem> Files
        {
            get { return new ReadOnlyObservableCollection<FileItem>(_files); }
        }

        public ReadOnlyObservableCollection<Tracker> Trackers
        {
            get { return new ReadOnlyObservableCollection<Tracker>(_trackers); }
        }

        public ReadOnlyObservableCollection<HttpSeed> HttpSeeds
        {
            get { return new ReadOnlyObservableCollection<HttpSeed>(_httpSeeds); }
        }

        public ReadOnlyObservableCollection<Node> Nodes
        {
            get { return new ReadOnlyObservableCollection<Node>(_nodes); }
        }

        public string Comment
        {
            get { return _comment; }
            set { Set(nameof(Comment), ref _comment, value); }
        }

        public string CommentUtf8
        {
            get { return _commentUtf8; }
            set { Set(nameof(CommentUtf8), ref _commentUtf8, value); }
        }

        public string PublisherUrl
        {
            get { return _publisherUrl; }
            set { Set(nameof(PublisherUrl), ref _publisherUrl, value); }
        }

        public string PublisherUrlUtf8
        {
            get { return _publisherUrlUtf8; }
            set { Set(nameof(PublisherUrlUtf8), ref _publisherUrlUtf8, value); }
        }

        public string CreatedBy
        {
            get { return _createdBy; }
            set { Set(nameof(CreatedBy), ref _createdBy, value); }
        }

        public long CreationDate
        {
            get { return _creationDate; }
            set { Set(nameof(CreationDate), ref _creationDate, value); }
        }

        public string Encoding
        {
            get { return _encoding; }
            set { Set(nameof(Encoding), ref _encoding, value); }
        }

        public bool IsPrivate
        {
            get { return _isPrivate; }
            set { Set(nameof(IsPrivate), ref _isPrivate, value); }
        }

        public byte[] Pieces
        {
            get { return _pieces; }
            set { Set(nameof(Pieces), ref _pieces, value); }
        }

        public int PieceCount
        {
            get { return _pieceCount; }
            set { Set(nameof(PieceCount), ref _pieceCount, value); }
        }

        public long DownloadSize
        {
            get { return _downloadSize; }
            set { Set(nameof(DownloadSize), ref _downloadSize, value); }
        }

        public long OriginalSize
        {
            get { return _originalSize; }
            set { Set(nameof(OriginalSize), ref _originalSize, value); }
        }

        public bool SkipHashCheck
        {
            get { return _skipHashCheck; }
            set { Set(nameof(SkipHashCheck), ref _skipHashCheck, value); }
        }

        public bool InitialSeeding
        {
            get { return _initialSeeding; }
            set { Set(nameof(InitialSeeding), ref _initialSeeding, value); }
        }

        public bool StartTorrent
        {
            get { return _startTorrent; }
            set { Set(nameof(StartTorrent), ref _startTorrent, value); }
        }

        public bool PeerExchange
        {
            get { return _peerExchange; }
            set { Set(nameof(PeerExchange), ref _peerExchange, value); }
        }

        public bool EnabledDht
        {
            get { return _enabledDht; }
            set { Set(nameof(EnabledDht), ref _enabledDht, value); }
        }

        public bool LocalPeerDiscovery
        {
            get { return _localPeerDiscovery; }
            set { Set(nameof(LocalPeerDiscovery), ref _localPeerDiscovery, value); }
        }

        public InitialQueuePosition InitialPosition
        {
            get { return _initialPosition; }
            set { Set(nameof(InitialPosition), ref _initialPosition, value); }
        }

        public string Category
        {
            get { return _category; }
            set { Set(nameof(Category), ref _category, value); }
        }

        public string DownloadFolder
        {
            get { return _downloadFolder; }
            set { Set(nameof(DownloadFolder), ref _downloadFolder, value); }
        }

        public bool MoveCompletedDownload
        {
            get { return _moveCompletedDownload; }
            set { Set(nameof(MoveCompletedDownload), ref _moveCompletedDownload, value); }
        }

        public string CompletedDownloadFolder
        {
            get { return _completedDownloadFolder; }
            set { Set(nameof(CompletedDownloadFolder), ref _completedDownloadFolder, value); }
        }

        public int MaximumDownloadRate
        {
            get { return _maximumDownloadRate; }
            set { Set(nameof(MaximumDownloadRate), ref _maximumDownloadRate, value); }
        }

        public int MaximumUploadRate
        {
            get { return _maximumUploadRate; }
            set { Set(nameof(MaximumUploadRate), ref _maximumUploadRate, value); }
        }

        public int MaximumConnectedPeers
        {
            get { return _maximumConnectedPeers; }
            set { Set(nameof(MaximumConnectedPeers), ref _maximumConnectedPeers, value); }
        }

        public int MaximumUploadSlots
        {
            get { return _maximumUploadSlots; }
            set { Set(nameof(MaximumUploadSlots), ref _maximumUploadSlots, value); }
        }

        public bool UseAdditionalSlotsOnLowUploadSpeed
        {
            get { return _useAdditionalSlotsOnLowUploadSpeed; }
            set { Set(nameof(UseAdditionalSlotsOnLowUploadSpeed), ref _useAdditionalSlotsOnLowUploadSpeed, value); }
        }

        public int AdditionalSlotsLowUploadSpeedPercent
        {
            get { return _additionalSlotsLowUploadSpeedPercent; }
            set { Set(nameof(AdditionalSlotsLowUploadSpeedPercent), ref _additionalSlotsLowUploadSpeedPercent, value); }
        }

        public void AddTracker(Tracker tracker)
        {
            if (!_trackers.Contains(tracker))
                _trackers.Add(tracker);

            RaisePropertyChanged(() => nameof(Trackers));
        }

        public void AddNode(Node node)
        {
            if (!_nodes.Contains(node))
                _nodes.Add(node);

            RaisePropertyChanged(() => nameof(Nodes));
        }

        public void AddHttpSeed(HttpSeed httpSeed)
        {
            if (!_httpSeeds.Contains(httpSeed))
                _httpSeeds.Add(httpSeed);

            RaisePropertyChanged(() => nameof(HttpSeeds));
        }

        public void AddFile(FileItem file)
        {
            if (!_files.Contains(file))
                _files.Add(file);

            RaisePropertyChanged(() => nameof(Files));
        }

        public void AddPeer(Peer peer)
        {
            if (!_peers.Contains(peer))
                _peers.Add(peer);

            RaisePropertyChanged(() => nameof(Peers));
        }

        public void Announce(AnnounceParameters announceparameters)
        {
            foreach (var tracker in _trackers)
                tracker.AnnounceAsync(announceparameters);
        }
    }
}