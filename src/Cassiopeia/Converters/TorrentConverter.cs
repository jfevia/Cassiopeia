using System;
using System.Linq;
using Cassiopeia.BitTorrent;
using Cassiopeia.Models;

namespace Cassiopeia.Converters
{
    internal static class TorrentConverter
    {
        public const string EncodingKey = "encoding";
        public const string CommentKey = "comment";
        public const string CommentUtf8Key = "comment.utf-8";
        public const string CreatedByKey = "created by";
        public const string CreationDateKey = "creation date";
        public const string AnnounceListKey = "announce-list";
        public const string AnnounceKey = "announce";
        public const string NameKey = "name";
        public const string PieceLengthKey = "piece length";
        public const string InfoDictionaryKey = "info";
        public const string PrivateKey = "private";
        public const string FileListKey = "files";
        public const string LengthKey = "length";
        public const string Md5ChecksumKey = "md5sum";
        public const string PathKey = "path";
        public const string NodesKey = "nodes";
        public const string HttpSeeds = "httpseeds";
        public const string UrlListKey = "url-list";
        public const string PublisherUrlKey = "publisher-url";
        public const string AzureusPropertiesKey = "azureus_properties";
        public const string PublisherUrlUtf8Key = "publisher-url.utf-8";

        public static Torrent ConvertFromBEncode(BEncodedValue value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (!(value is BEncodedDictionary))
                throw new ArgumentException($"Invalid BEncode object. Expected root {nameof(BEncodedDictionary)} value");

            var torrent = new Torrent();
            var dictionary = (BEncodedDictionary) value;

            foreach (var kvp in dictionary)
                switch (kvp.Key.Text)
                {
                    case EncodingKey:
                        torrent.Encoding = ((BEncodedString) kvp.Value).Text;
                        break;
                    case NodesKey:
                        var nodesList = (BEncodedList) kvp.Value;
                        foreach (var nodeItem in nodesList)
                        {
                            var nodeItemList = (BEncodedList) nodeItem;

                            foreach (var node in nodeItemList)
                                torrent.AddNode(new Node(((BEncodedString) node).Text));
                        }
                        break;
                    case UrlListKey:
                        var encodedString = kvp.Value as BEncodedString;
                        if (encodedString != null)
                        {
                            torrent.AddHttpSeed(new HttpSeed(encodedString.Text));
                        }
                        else if (kvp.Value is BEncodedList)
                        {
                            var urlList = (BEncodedList) kvp.Value;
                            foreach (var urlItem in urlList)
                            {
                                var urlItemList = (BEncodedList) urlItem;

                                foreach (var url in urlItemList)
                                    torrent.AddHttpSeed(new HttpSeed(((BEncodedString) url).Text));
                            }
                        }
                        break;
                    case AzureusPropertiesKey:
                        throw new NotSupportedException("Azureus Properties are not supported in this version.");
                    case HttpSeeds:
                        throw new NotSupportedException(
                            $"The form of web seeding '{HttpSeeds}' is not supported in this version.");
                    case PublisherUrlKey:
                        torrent.PublisherUrl = ((BEncodedString) kvp.Value).Text;
                        break;
                    case PublisherUrlUtf8Key:
                        torrent.PublisherUrlUtf8 = ((BEncodedString) kvp.Value).Text;
                        break;
                    case CommentUtf8Key:
                        torrent.CommentUtf8 = ((BEncodedString) kvp.Value).Text;
                        break;
                    case CommentKey:
                        torrent.Comment = ((BEncodedString) kvp.Value).Text;
                        break;
                    case CreatedByKey:
                        torrent.CreatedBy = ((BEncodedString) kvp.Value).Text;
                        break;
                    case CreationDateKey:
                        torrent.CreationDate = ((BEncodedNumber) kvp.Value).Number;
                        break;
                    case AnnounceListKey:
                        var announceList = (BEncodedList) kvp.Value;
                        foreach (var announceItem in announceList)
                        {
                            var announceItemList = (BEncodedList) announceItem;

                            foreach (var announce in announceItemList)
                                torrent.AddTracker(new Tracker(((BEncodedString) announce).Text));
                        }
                        break;
                    case AnnounceKey:
                        // BEP 12: if the "announce-list" key is present, the client will ignore the "announce" key
                        if (dictionary.Keys.Any(s => s.Text == AnnounceListKey))
                            continue;

                        torrent.AddTracker(new Tracker(((BEncodedString) kvp.Value).Text));
                        break;
                    case InfoDictionaryKey:
                        var infoDictionary = (BEncodedDictionary) kvp.Value;
                        foreach (var infoKvp in infoDictionary)
                            switch (infoKvp.Key.Text)
                            {
                                case NameKey:
                                    torrent.Name = ((BEncodedString) infoKvp.Value).Text;
                                    break;
                                case PieceLengthKey:
                                    torrent.PieceSize = ((BEncodedNumber) infoKvp.Value).Number;
                                    break;
                                case PrivateKey:
                                    torrent.IsPrivate = ((BEncodedNumber) infoKvp.Value).Number != 0;
                                    break;
                                case FileListKey:
                                    var fileList = (BEncodedList) infoKvp.Value;
                                    foreach (var fileDetails in fileList)
                                    {
                                        var file = new FileItem();
                                        var fileDetailsDictionary = (BEncodedDictionary) fileDetails;

                                        foreach (var kvpFileDetails in fileDetailsDictionary)
                                            switch (kvpFileDetails.Key.Text)
                                            {
                                                case LengthKey:
                                                    file.Size = ((BEncodedNumber) kvpFileDetails.Value).Number;
                                                    break;
                                                case Md5ChecksumKey:
                                                    file.Md5 = ((BEncodedString) kvpFileDetails.Value).Text;
                                                    break;
                                                case PathKey:
                                                    var filePaths = (BEncodedList) kvpFileDetails.Value;

                                                    foreach (var path in filePaths)
                                                        file.Path.Add(((BEncodedString) path).Text);
                                                    break;
                                            }

                                        torrent.AddFile(file);
                                    }
                                    break;
                                default:
                                    throw new NotSupportedException(
                                        $"Unknown metadata key '{infoKvp.Key.Text}' found in Info dictionary.");
                            }
                        break;
                    default:
                        throw new NotSupportedException(
                            $"Unknown metadata key '{kvp.Key.Text}' found in root dictionary.");
                }

            return torrent;
        }
    }
}