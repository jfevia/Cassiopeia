using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Cassiopeia.BitTorrent;
using Cassiopeia.Collections.ObjectModel;
using Cassiopeia.Common;

namespace Cassiopeia.Models
{
    internal sealed class HttpTracker : Tracker
    {
        private const string UniqueIdKey = "key";
        private const string TrackerIdKey = "trackerid";
        private const string TorrentEventKey = "event";
        private const string IpAddressKey = "ip";
        private const string RequiresEncryptionKey = "requirecrypto";
        private const string SupportsEncryptionKey = "supportcrypto";
        private const string MaximumPeerCountKey = "numwant";
        private const string CompactResponseKey = "compact";
        private const string LeftKey = "left";
        private const string DownloadedKey = "downloaded";
        private const string UploadedKey = "uploaded";
        private const string PortKey = "port";
        private const string PeerIdKey = "peer_id";
        private const string InfoHashKey = "info_hash";
        private const string CompleteKey = "complete";
        private const string IncompleteKey = "incomplete";
        private const string MinimumUpdateIntervalKey = "min interval";
        private const string UpdateIntervalKey = "interval";
        private const string PeersKey = "peers";
        private const string FailureReasonKey = "failure reason";
        private const string WarningMessageKey = "warning message";
        private static readonly Random Random = new Random();
        private int _complete;
        private int _incomplete;
        private string _key;
        private TimeSpan _minUpdateInterval;
        private string _trackerId;
        private TimeSpan _updateInterval;
        private ObservableCollection<LoggableException> _exceptions;

        public HttpTracker(Uri uri) : base(uri)
        {
            _exceptions = new ObservableCollection<LoggableException>();

            var passwordKey = new byte[8];
            lock (Random)
            {
                Random.NextBytes(passwordKey);
            }
            Key = passwordKey.ToUrlEncode();
        }

        public string Key
        {
            get { return _key; }
            set { Set(nameof(Key), ref _key, value); }
        }

        public string TrackerId
        {
            get { return _trackerId; }
            set { Set(nameof(TrackerId), ref _trackerId, value); }
        }

        public TimeSpan UpdateInterval
        {
            get { return _updateInterval; }
            set { Set(nameof(UpdateInterval), ref _updateInterval, value); }
        }

        public TimeSpan MinUpdateInterval
        {
            get { return _minUpdateInterval; }
            set { Set(nameof(MinUpdateInterval), ref _minUpdateInterval, value); }
        }

        public int Complete
        {
            get { return _complete; }
            set { Set(nameof(Complete), ref _complete, value); }
        }

        public int Incomplete
        {
            get { return _incomplete; }
            set { Set(nameof(Incomplete), ref _incomplete, value); }
        }

        public ObservableCollection<LoggableException> Exceptions
        {
            get { return _exceptions; }
            set { Set(nameof(Exceptions), ref _exceptions, value); }
        }

        private Uri GetAnnounceUri(AnnounceParameters parameters)
        {
            var queryBuilder = new TrackerQueryBuilder(Uri);
            queryBuilder.AddParameter(InfoHashKey, parameters.InfoHash.ToUrlEncode());
            queryBuilder.AddParameter(PeerIdKey, parameters.PeerId);
            queryBuilder.AddParameter(PortKey, parameters.Port);
            queryBuilder.AddParameter(UploadedKey, parameters.BytesUploaded);
            queryBuilder.AddParameter(DownloadedKey, parameters.BytesDownloaded);
            queryBuilder.AddParameter(LeftKey, parameters.BytesLeft);
            queryBuilder.AddParameter(MaximumPeerCountKey, parameters.MaximumPeerCount);
            queryBuilder.AddParameter(CompactResponseKey, parameters.UseCompactResponse);
            queryBuilder.AddParameter(SupportsEncryptionKey, parameters.SupportsEncryption);
            queryBuilder.AddParameter(RequiresEncryptionKey, parameters.RequiresEncryption);
            queryBuilder.AddParameter(TorrentEventKey, parameters.Event.ToAnnounceParameter());

            queryBuilder.AddParameter(UniqueIdKey, Key);

            if (!string.IsNullOrWhiteSpace(parameters.IpAddress)) queryBuilder.AddParameter(IpAddressKey, parameters.IpAddress);
            if (!string.IsNullOrWhiteSpace(TrackerId)) queryBuilder.AddParameter(TrackerIdKey, TrackerId);

            return queryBuilder.ToUri();
        }

        public override async void AnnounceAsync(AnnounceParameters parameters)
        {
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    var announceUri = GetAnnounceUri(parameters);
                    var request = (HttpWebRequest) WebRequest.Create(announceUri);
                    request.UserAgent = parameters.UserAgent;
                    request.Proxy = new WebProxy();
                    request.BeginGetResponse(EndAnnouncement, new object[] {request, TrackerId});
                }
                catch (Exception ex)
                {
                    Status = TrackerStatus.Offline;

                    var exception = new LoggableException();
                    exception.DateTime = DateTime.Now;
                    exception.Exception = ex;

                    Exceptions.Add(exception);
                }
            });
        }

        private BEncodedDictionary DecodeResponse(WebRequest request, IAsyncResult result)
        {
            var totalRead = 0;
            var buffer = new byte[2048];

            var response = request.EndGetResponse(result);
            using (var dataStream = new MemoryStream(response.ContentLength > 0 ? (int) response.ContentLength : 256))
            {
                var responseStream = response.GetResponseStream();
                if (responseStream == null)
                    throw new NullReferenceException(nameof(responseStream));

                using (var reader = new BinaryReader(responseStream))
                {
                    // If there is a ContentLength, use that to decide how much we read.
                    int bytesRead;
                    if (response.ContentLength > 0)
                        while (totalRead < response.ContentLength)
                        {
                            bytesRead = reader.Read(buffer, 0, buffer.Length);
                            dataStream.Write(buffer, 0, bytesRead);
                            totalRead += bytesRead;
                        }
                    else
                        while ((bytesRead = reader.Read(buffer, 0, buffer.Length)) > 0)
                            dataStream.Write(buffer, 0, bytesRead);
                }
                response.Close();
                dataStream.Seek(0, SeekOrigin.Begin);
                return (BEncodedDictionary) BEncode.Decode(dataStream);
            }
        }

        private void EndAnnouncement(IAsyncResult asyncResult)
        {
            var state = (object[]) asyncResult.AsyncState;
            var request = (WebRequest) state[0];
            var trackerId = (string) state[1];
            var peers = new List<Peer>();

            try
            {
                ConvertAnnounceFromBencode(DecodeResponse(request, asyncResult), peers);
                Status = TrackerStatus.Working;
            }
            catch (WebException webException)
            {
                Status = TrackerStatus.Offline;
                throw new Exception("The tracker could not be contacted", webException);
            }
            catch(Exception ex)
            {
                Status = TrackerStatus.InvalidResponse;
                throw new Exception("The tracker returned an invalid or incomplete response", ex);
            }
        }

        private void ConvertAnnounceFromBencode(BEncodedDictionary dictionary, List<Peer> peers)
        {
            foreach (var kvp in dictionary)
                switch (kvp.Key.Text)
                {
                    case CompleteKey:
                        Complete = Convert.ToInt32(((BEncodedNumber) kvp.Value).Number);
                        break;
                    case IncompleteKey:
                        Incomplete = Convert.ToInt32(((BEncodedNumber)kvp.Value).Number);
                        break;
                    case DownloadedKey:
                        Downloaded = Convert.ToInt32(((BEncodedNumber)kvp.Value).Number);
                        break;
                    case TrackerIdKey:
                        TrackerId = ((BEncodedString)kvp.Value).Text;
                        break;
                    case MinimumUpdateIntervalKey:
                        MinUpdateInterval = TimeSpan.FromSeconds(Convert.ToInt32(((BEncodedNumber)kvp.Value).Number));
                        break;
                    case UpdateIntervalKey:
                        UpdateInterval = TimeSpan.FromSeconds(Convert.ToInt32(((BEncodedNumber)kvp.Value).Number));
                        break;
                    case PeersKey:
                        // (Non)compact response
                        if (kvp.Value is BEncodedList) peers.AddRange(Peer.Decode((BEncodedList) kvp.Value));
                        else if (kvp.Value is BEncodedString) peers.AddRange(Peer.Decode((BEncodedString) kvp.Value));
                        break;
                    case FailureReasonKey:
                        throw new Exception(((BEncodedString) kvp.Value).Text);
                    case WarningMessageKey:
                        throw new Exception(((BEncodedString) kvp.Value).Text);
                    default:
                        throw new NotImplementedException($"Unknown announce tag received '{kvp.Key.Text}'.");
                }
        }

        public override async void ScrapeAsync()
        {
            // TODO: Implement
            throw new NotImplementedException();
        }
    }
}