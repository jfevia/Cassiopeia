using System;
using System.IO;

namespace Cassiopeia.BitTorrent
{
    public class BEncode
    {
        public static BEncodedValue Decode(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            using (var stream = new BitTorrentReader(new MemoryStream(data)))
            {
                return Decode(stream);
            }
        }

        public static BEncodedValue Decode(byte[] buffer, int offset, int length)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            if (offset < 0 || length < 0)
                throw new IndexOutOfRangeException("Neither offset or length can be less than zero");

            if (offset > buffer.Length - length)
                throw new ArgumentOutOfRangeException(nameof(length));

            using (var reader = new BitTorrentReader(new MemoryStream(buffer, offset, length)))
            {
                return Decode(reader);
            }
        }

        public static BEncodedValue Decode(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            return BEncodedValue.Decode(new BitTorrentReader(stream));
        }

        public static T Decode<T>(byte[] data) where T : BEncodedValue
        {
            return (T) Decode(data);
        }

        public static T Decode<T>(byte[] buffer, int offset, int length) where T : BEncodedValue
        {
            return (T) Decode(buffer, offset, length);
        }

        public static T Decode<T>(Stream stream) where T : BEncodedValue
        {
            return (T) Decode(stream);
        }

        public static T Decode<T>(BitTorrentReader reader) where T : BEncodedValue
        {
            return (T) Decode(reader);
        }
    }
}