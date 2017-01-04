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
            return (T) BEncode.Decode(value.Encode());
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
    }
}