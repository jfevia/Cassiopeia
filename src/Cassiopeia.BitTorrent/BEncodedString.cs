using Cassiopeia.Common;
using System;
using System.Linq;
using System.Text;

namespace Cassiopeia.BitTorrent
{
    public class BEncodedString : BEncodedValue, IComparable<BEncodedString>
    {
        private byte[] _textBytes;

        public BEncodedString(BitTorrentReader reader)
            : this(new byte[0], reader)
        {
        }

        public BEncodedString(char[] value, BitTorrentReader reader)
            : this(Encoding.UTF8.GetBytes(value), reader)
        {
        }

        public BEncodedString(string value, BitTorrentReader reader)
            : this(Encoding.UTF8.GetBytes(value), reader)
        {
        }

        public BEncodedString(byte[] value, BitTorrentReader reader) : base(reader)
        {
            TextBytes = value;
        }

        public string Text => Encoding.UTF8.GetString(TextBytes);

        public byte[] TextBytes
        {
            get { return _textBytes; }
            internal set
            {
                _textBytes = value;
                ByteLength = GetLength();
            }
        }

        public string Hex => BitConverter.ToString(TextBytes);

        public int CompareTo(BEncodedString other)
        {
            if (other == null)
                return 1;

            int difference;
            var length = TextBytes.Length > other.TextBytes.Length ? other.TextBytes.Length : TextBytes.Length;

            for (var i = 0; i < length; i++)
                if ((difference = TextBytes[i].CompareTo(other.TextBytes[i])) != 0)
                    return difference;

            if (TextBytes.Length == other.TextBytes.Length)
                return 0;

            return TextBytes.Length > other.TextBytes.Length ? 1 : -1;
        }

        protected override int GetLength()
        {
            return GetPrefixLength() + TextBytes.Length;
        }

        private int GetPrefixLength()
        {
            // The length is equal to the length-prefix + ':' + length of data
            var prefix = 1; // Account for ':'

            if (TextBytes.Length == 0)
                prefix++;
            else
                // Count the number of characters needed for the length prefix
                for (var i = TextBytes.Length; i != 0; i /= 10)
                    prefix++;

            return prefix;
        }

        public override int Encode(byte[] buffer, int offset)
        {
            var written = offset;
            written += Append(buffer, written, TextBytes.Length.ToString());
            written += Append(buffer, written, ByteArrayDivider);
            written += Append(buffer, written, TextBytes);
            return written - offset;
        }

        protected int Append(byte[] buffer, int offset, byte value)
        {
            buffer[offset] = value;
            return 1;
        }

        protected int Append(byte[] buffer, int offset, byte[] value)
        {
            return Append(buffer, offset, value, 0, value.Length);
        }

        protected int Append(byte[] dest, int destOffset, byte[] src, int srcOffset, int count)
        {
            Buffer.BlockCopy(src, srcOffset, dest, destOffset, count);
            return count;
        }

        protected int Write(byte[] buffer, int offset, byte value)
        {
            buffer[offset] = value;
            return 1;
        }

        protected int Append(byte[] buffer, int offset, string value)
        {
            for (var i = 0; i < value.Length; i++)
                Write(buffer, offset + i, (byte) value[i]);
            return value.Length;
        }

        internal override void Decode()
        {
            var length = string.Empty;

            while (Reader.PeekByte() != -1 && Reader.PeekByte() != ByteArrayDivider)
                length += (char) Reader.ReadByte();

            if (Reader.ReadByte() != ByteArrayDivider)
                throw new BEncodingException($"Invalid data. Expected {ByteArrayDivider}, found {Reader.PeekByte()}");

            if (!int.TryParse(length, out int characterCount))
                throw new BEncodingException(
                    $"Invalid data. Length was '{length}' instead of a number");

            TextBytes = new byte[characterCount];
            if (Reader.Read(TextBytes, 0, characterCount) != characterCount)
                throw new BEncodingException("Could not decode string");
        }

        public int CompareTo(object other)
        {
            return CompareTo(other as BEncodedString);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            BEncodedString other;
            if (obj is string)
                other = new BEncodedString((string) obj, Reader);
            else if (obj is BEncodedString)
                other = (BEncodedString) obj;
            else
                return false;

            return TextBytes.ByteMatch(other.TextBytes);
        }

        public override int GetHashCode()
        {
            return TextBytes.Aggregate(0, (current, t) => current + t);
        }

        public override string ToString()
        {
            return Encoding.UTF8.GetString(TextBytes);
        }
    }
}