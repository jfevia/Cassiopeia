using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Cassiopeia.Common;

namespace Cassiopeia.Models
{
    internal sealed class HttpTracker : Tracker
    {
        private static readonly Random Random = new Random();
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
        private string _key;
        private string _trackerId;

        public HttpTracker(Uri uri) : base(uri)
        {
            var passwordKey = new byte[8];
            lock (Random) Random.NextBytes(passwordKey);
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
                    var request = (HttpWebRequest)WebRequest.Create(announceUri);
                    request.UserAgent = parameters.UserAgent;
                    request.Proxy = new WebProxy();
                    request.BeginGetResponse(EndAnnouncement, new object[] {request, TrackerId});
                }
                catch (Exception)
                {
                    Status = TrackerStatus.Offline;
                    // TODO: FailureMessage = ("Could not initiate announce request: " + ex.Message);
                    // TODO: RaiseAnnounceComplete(new AnnounceResponseEventArgs(this, state, false));
                }
            });
        }

        private void EndAnnouncement(IAsyncResult asyncResult)
        {
            var state = (object[])asyncResult.AsyncState;
            var request = (WebRequest)state[0];
            var trackerId = (string)state[1];
            var peers = new List<Peer>();

            try
            {
                // TODO: Handle reply
                //BEncodedDictionary dict = DecodeResponse(request, asyncResult);
                //HandleAnnounce(dict, peers);
                Status = TrackerStatus.Working;
            }
            catch (WebException)
            {
                Status = TrackerStatus.Offline;
                // TODO: FailureMessage = "The tracker could not be contacted";
            }
            catch
            {
                Status = TrackerStatus.InvalidResponse;
                // TODO: FailureMessage = "The tracker returned an invalid or incomplete response";
            }
            finally
            {
                // TODO: RaiseAnnounceComplete(new AnnounceResponseEventArgs(this, trackerId, string.IsNullOrEmpty(FailureMessage), peers));
            }
        }

        public override async void ScrapeAsync()
        {
            // TODO: Implement
            throw new NotImplementedException();
        }
    }
}