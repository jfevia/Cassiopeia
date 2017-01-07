using System;
using Cassiopeia.BitTorrent;
using Cassiopeia.Models;

namespace Cassiopeia.Converters
{
    internal static class TorrentConverter
    {
        public const string EncodingKey = "encoding";
        public const string CommentKey = "comment";
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
        public const string Md5SumKey = "md5sum";
        public const string PathKey = "path";

        public static Torrent ConvertFromBEncode(BEncodedValue value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (!(value is BEncodedDictionary))
                throw new ArgumentException($"Invalid BEncode object. Expected root {nameof(BEncodedDictionary)} value");

            var torrent = new Torrent();
            var dictionary = (BEncodedDictionary) value;

            if (dictionary.ContainsKey(EncodingKey))
                torrent.Encoding = ((BEncodedString) dictionary[EncodingKey]).Text;

            if (dictionary.ContainsKey(CommentKey))
                torrent.Comment = ((BEncodedString) dictionary[CommentKey]).Text;

            if (dictionary.ContainsKey(CreatedByKey))
                torrent.CreatedBy = ((BEncodedString) dictionary[CreatedByKey]).Text;

            if (dictionary.ContainsKey(CreationDateKey))
                torrent.CreationDate = ((BEncodedNumber) dictionary[CreationDateKey]).Number;

            if (dictionary.ContainsKey(AnnounceListKey))
            {
                var announceList = (BEncodedList) dictionary[AnnounceListKey];
                foreach (var announceItem in announceList)
                {
                    var announceItemList = (BEncodedList) announceItem;

                    foreach (var announce in announceItemList)
                        torrent.AddTracker(new Tracker(((BEncodedString) announce).Text));
                }
            }

            if (dictionary.ContainsKey(AnnounceKey))
                torrent.AddTracker(new Tracker(((BEncodedString) dictionary[AnnounceKey]).Text));

            if (dictionary.ContainsKey(InfoDictionaryKey))
            {
                var infoDictionary = (BEncodedDictionary) dictionary[InfoDictionaryKey];

                if (infoDictionary.ContainsKey(NameKey))
                    torrent.Name = ((BEncodedString) infoDictionary[NameKey]).Text;

                if (infoDictionary.ContainsKey(PieceLengthKey))
                    torrent.PieceSize = ((BEncodedNumber) infoDictionary[PieceLengthKey]).Number;

                if (infoDictionary.ContainsKey(PrivateKey))
                    torrent.IsPrivate = ((BEncodedNumber) infoDictionary[PrivateKey]).Number != 0;

                if (infoDictionary.ContainsKey(FileListKey))
                {
                    var fileList = (BEncodedList) infoDictionary[FileListKey];
                    foreach (var fileDetails in fileList)
                    {
                        var file = new FileItem();
                        var fileDetailsDictionary = (BEncodedDictionary) fileDetails;

                        if (fileDetailsDictionary.ContainsKey(LengthKey))
                            file.Size = ((BEncodedNumber) fileDetailsDictionary[LengthKey]).Number;

                        if (fileDetailsDictionary.ContainsKey(Md5SumKey))
                            file.Md5Sum = ((BEncodedString) fileDetailsDictionary[Md5SumKey]).Text;

                        if (fileDetailsDictionary.ContainsKey(PathKey))
                        {
                            var filePaths = (BEncodedList) fileDetailsDictionary[PathKey];

                            foreach (var path in filePaths)
                                file.Path.Add(((BEncodedString) path).Text);
                        }

                        torrent.AddFile(file);
                    }
                }
            }

            return torrent;
        }
    }
}