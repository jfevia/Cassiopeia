using System;
using System.IO;

namespace Cassiopeia.BitTorrent
{
    public abstract class BEncodedValue : IBEncodedValue
    {
        protected BEncodedValue(BitTorrentReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            Reader = reader;
        }

        protected const byte DictionaryStartDelimiter = (byte)'d';
        protected const byte DictionaryEndDelimiter = (byte)'e';
        protected const byte ListStartDelimiter = (byte)'l';
        protected const byte ListEndDelimiter = (byte)'e';
        protected const byte NumberStartDelimiter = (byte)'i';
        protected const byte NumberEndDelimiter = (byte)'e';
        protected const byte ByteArrayDivider = (byte)':';
        protected const byte NumberNegativeSign = (byte) '-';

        protected BitTorrentReader Reader { get; }
        public int ByteLength { get; internal set; }
        protected abstract int GetLength();

        public byte[] Encode()
        {
            var buffer = new byte[ByteLength];
            if (Encode(buffer, 0) != buffer.Length)
                throw new BEncodingException("Error encoding data");

            return buffer;
        }

        public abstract int Encode(byte[] buffer, int offset);

        internal abstract void Decode();

        public T Clone<T>(T value)
            where T : BEncodedValue
        {
            return (T) Decode(value.Encode());
        }

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

            return Decode(new BitTorrentReader(stream));
        }

        public static BEncodedValue Decode(BitTorrentReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            BEncodedValue data;
            switch (reader.PeekByte())
            {
                case NumberStartDelimiter:
                    data = new BEncodedNumber(reader);
                    break;

                case DictionaryStartDelimiter:
                    data = new BEncodedDictionary(reader);
                    break;

                case ListStartDelimiter:
                    data = new BEncodedList(reader);
                    break;

                case 48: // 0-9
                case 49:
                case 50:
                case 51:
                case 52:
                case 53:
                case 54:
                case 55:
                case 56:
                case 57:
                    data = new BEncodedString(reader);
                    break;

                default:
                    throw new BEncodingException("Invalid decode value");
            }

            data.Decode();
            return data;
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