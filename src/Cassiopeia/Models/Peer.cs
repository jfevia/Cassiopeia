using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Cassiopeia.BitTorrent;
using GalaSoft.MvvmLight;

namespace Cassiopeia.Models
{
    internal class Peer : ObservableObject
    {
        private const string PeerIdKey = "peer id";
        private const string AltPeerIdKey = "peer_id";
        private const string IpAddressKey = "ip";
        private const string PortKey = "port";
        private Uri _address;
        private string _client;
        private int _country;
        private long _downloaded;
        private double _downloadSpeed;
        private Encryption _encryption;
        private string _peerId;
        private double _progress;
        private long _uploaded;
        private double _uploadSpeed;

        public Encryption Encryption
        {
            get { return _encryption; }
            set { Set(nameof(Encryption), ref _encryption, value); }
        }

        public string PeerId
        {
            get { return _peerId; }
            set { Set(nameof(PeerId), ref _peerId, value); }
        }

        public int Country
        {
            get { return _country; }
            set { Set(nameof(Country), ref _country, value); }
        }

        public Uri Address
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

        public static IEnumerable<Peer> Decode(BEncodedList list)
        {
            foreach (var value in list)
                if (value is BEncodedDictionary)
                    yield return Decode((BEncodedDictionary) value);
                else if (value is BEncodedString)
                    foreach (var peer in Decode((BEncodedString) value))
                        yield return peer;
        }

        private static Peer Decode(BEncodedDictionary dict)
        {
            string peerId = null;
            string ipAddress = null;
            string port = null;

            foreach (var kvp in dict)
                switch (kvp.Key.Text)
                {
                    case PeerIdKey:
                        peerId = ((BEncodedString) kvp.Value).Text;
                        break;
                    case AltPeerIdKey:
                        // Certain trackers return "peer_id" instead of "peer id"
                        peerId = ((BEncodedString) kvp.Value).Text;
                        break;
                    case IpAddressKey:
                        ipAddress = ((BEncodedString) kvp.Value).Text;
                        break;
                    case PortKey:
                        port = ((BEncodedString) kvp.Value).Text;
                        break;
                    default:
                        throw new NotImplementedException($"Key not implemented {kvp.Key.Text}");
                }

            if (string.IsNullOrWhiteSpace(ipAddress))
                throw new ArgumentNullException(nameof(ipAddress));

            if (string.IsNullOrWhiteSpace(port))
                throw new ArgumentNullException(nameof(port));

            var peer = new Peer();
            peer.PeerId = peerId;
            peer.Address = new Uri($"tcp://{ipAddress}:{port}");
            peer.Encryption = Encryption.All;

            return peer;
        }

        public static IEnumerable<Peer> Decode(BEncodedString peers)
        {
            // "Compact Response" peers are encoded in network byte order. 
            // IP's are the first four bytes
            // Ports are the following 2 bytes
            var byteOrderedData = peers.TextBytes;
            var i = 0;
            var stringBuilder = new StringBuilder(27);
            while (i + 5 < byteOrderedData.Length)
            {
                stringBuilder.Remove(0, stringBuilder.Length);

                stringBuilder.Append("tcp://");
                stringBuilder.Append(byteOrderedData[i++]);
                stringBuilder.Append('.');
                stringBuilder.Append(byteOrderedData[i++]);
                stringBuilder.Append('.');
                stringBuilder.Append(byteOrderedData[i++]);
                stringBuilder.Append('.');
                stringBuilder.Append(byteOrderedData[i++]);

                var port = (ushort) IPAddress.NetworkToHostOrder(BitConverter.ToInt16(byteOrderedData, i));
                stringBuilder.Append(':');
                stringBuilder.Append(port);

                var peer = new Peer();
                peer.PeerId = null;
                peer.Address = new Uri(stringBuilder.ToString());
                peer.Encryption = Encryption.All;

                yield return peer;
            }
        }
    }
}