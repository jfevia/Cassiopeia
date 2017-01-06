using System;
using Cassiopeia.BitTorrent;
using Cassiopeia.Models;

namespace Cassiopeia.Converters
{
    internal static class TorrentConverter
    {
        public static Torrent ConvertFromBEncode(BEncodedValue value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            
            if (!(value is BEncodedDictionary))
                throw new ArgumentException($"Invalid BEncode object. Expected root {nameof(BEncodedDictionary)} value");

            var torrent = new Torrent();
            var dictionary = (BEncodedDictionary) value;

            if (dictionary.ContainsKey("encoding"))
                torrent.Encoding = ((BEncodedString) dictionary["encoding"]).Text;

            if (dictionary.ContainsKey("comment"))
                torrent.Comment = ((BEncodedString) dictionary["comment"]).Text;

            if (dictionary.ContainsKey("created by"))
                torrent.CreatedBy = ((BEncodedString) dictionary["created by"]).Text;

            if (dictionary.ContainsKey("creation date"))
                torrent.CreationDate = ((BEncodedNumber) dictionary["creation date"]).Number;

            if (dictionary.ContainsKey("announce-list"))
            {
                var announceList = (BEncodedList) dictionary["announce-list"];
                foreach (var announceItem in announceList)
                {
                    var announceItemList = (BEncodedList) announceItem;

                    foreach (var announce in announceItemList)
                        torrent.AddTracker(new Tracker(((BEncodedString)announce).Text));
                }
            }

            if (dictionary.ContainsKey("announce"))
                torrent.AddTracker(new Tracker(((BEncodedString)dictionary["announce"]).Text));

            if (dictionary.ContainsKey("info"))
            {
                var infoDictionary = (BEncodedDictionary) dictionary["info"];

                if (infoDictionary.ContainsKey("name"))
                    torrent.Name = ((BEncodedString) infoDictionary["name"]).Text;

                if (infoDictionary.ContainsKey("piece length"))
                    torrent.PieceSize = ((BEncodedNumber) infoDictionary["piece length"]).Number;

                if (infoDictionary.ContainsKey("private"))
                    torrent.IsPrivate = ((BEncodedNumber) infoDictionary["private"]).Number != 0;

                if (infoDictionary.ContainsKey("files"))
                {
                    var fileList = (BEncodedList) infoDictionary["files"];
                    foreach (var fileDetails in fileList)
                    {
                        var file = new FileItem();
                        var fileDetailsDictionary = (BEncodedDictionary) fileDetails;

                        if (fileDetailsDictionary.ContainsKey("length"))
                            file.Size = ((BEncodedNumber) fileDetailsDictionary["length"]).Number;

                        if (fileDetailsDictionary.ContainsKey("md5sum"))
                            file.Md5Sum = ((BEncodedString) fileDetailsDictionary["md5sum"]).Text;

                        if (fileDetailsDictionary.ContainsKey("path"))
                        {
                            var filePaths = (BEncodedList) fileDetailsDictionary["path"];

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