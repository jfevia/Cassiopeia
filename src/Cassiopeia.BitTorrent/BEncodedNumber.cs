using System;

namespace Cassiopeia.BitTorrent
{
    public class BEncodedNumber : BEncodedValue, IComparable<BEncodedNumber>
    {
        private long _number;

        public BEncodedNumber(BitTorrentReader reader)
            : this(0, reader)
        {
        }

        public BEncodedNumber(long value, BitTorrentReader reader) : base(reader)
        {
            Number = value;
        }

        public long Number
        {
            get { return _number; }
            internal set
            {
                _number = value;
                ByteLength = GetLength();
            }
        }

        protected override int GetLength()
        {
            var number = Number;

            // Numbers have accountable beginning/ending delimiters
            var length = 2;

            if (number == 0)
                ByteLength = length + 1;

            if (number < 0)
            {
                number = -number;
                length++;
            }

            for (var i = number; i != 0; i /= 10)
                length++;

            return length;
        }

        public int CompareTo(BEncodedNumber other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return Number.CompareTo(other.Number);
        }

        public override int Encode(byte[] buffer, int offset)
        {
            var number = Number;

            var written = offset;
            buffer[written++] = NumberStartDelimiter;

            if (number < 0)
            {
                buffer[written++] = NumberNegativeSign;
                number = -number;
            }
            // Reverse the number '12345' to get '54321'
            long reversed = 0;
            for (var i = number; i != 0; i /= 10)
                reversed = reversed * 10 + i % 10;

            // Write each digit of the reversed number to the array. We write '1' first, then '2', etc
            for (var i = reversed; i != 0; i /= 10)
                buffer[written++] = (byte) (i % 10 + '0');

            if (number == 0)
                buffer[written++] = (byte) '0';

            // If the original number ends in one or more zeros, they are lost
            // when we reverse the number. We add them back in here.
            for (var i = number; i % 10 == 0 && number != 0; i /= 10)
                buffer[written++] = (byte) '0';

            buffer[written++] = NumberEndDelimiter;
            return written - offset;
        }

        internal override void Decode()
        {
            var sign = 1;

            if (Reader.ReadByte() != NumberStartDelimiter)
                throw new BEncodingException(
                    $"Invalid data. Expected {DictionaryStartDelimiter}, found {Reader.PeekByte()}");

            if (Reader.PeekByte() == NumberNegativeSign)
            {
                sign = -1;
                Reader.ReadByte();
            }

            int character;
            while ((character = Reader.PeekByte()) != -1 && character != NumberEndDelimiter)
            {
                if (character < '0' || character > '9')
                    throw new BEncodingException(
                        $"Invalid data. Expected digit (0-9) byte representation ({(byte)'0'}-{(byte)'9'}), found {Reader.PeekByte()}");
                Number = Number * 10 + (character - '0');
                Reader.ReadByte();
            }

            if (Reader.ReadByte() != NumberEndDelimiter)
                throw new BEncodingException(
                    $"Invalid data. Expected {DictionaryEndDelimiter}, found {Reader.PeekByte()}");

            Number *= sign;
        }


        public int CompareTo(object other)
        {
            if (other is BEncodedNumber || other is long || other is int)
                return CompareTo((BEncodedNumber) other);

            return -1;
        }


        public int CompareTo(long other)
        {
            return Number.CompareTo(other);
        }

        public override bool Equals(object obj)
        {
            var obj2 = obj as BEncodedNumber;

            return Number == obj2?.Number;
        }

        public override int GetHashCode()
        {
            return Number.GetHashCode();
        }

        public override string ToString()
        {
            return Number.ToString();
        }
    }
}