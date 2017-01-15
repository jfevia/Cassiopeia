using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Cassiopeia.BitTorrent
{
    public class BEncodedDictionary : BEncodedValue, IDictionary<BEncodedString, BEncodedValue>
    {
        internal readonly SortedDictionary<BEncodedString, BEncodedValue> Dictionary;

        public BEncodedDictionary(BitTorrentReader reader) : base(reader)
        {
            Dictionary = new SortedDictionary<BEncodedString, BEncodedValue>();
        }

        public void Add(BEncodedString key, BEncodedValue value)
        {
            Dictionary.Add(key, value);

            // Dictionaries have accountable beginning/ending delimiters
            if (Dictionary.Count == 1)
                ByteLength = 2;

            ByteLength += key.ByteLength;
            ByteLength += value.ByteLength;
        }

        public void Add(KeyValuePair<BEncodedString, BEncodedValue> item)
        {
            Dictionary.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            Dictionary.Clear();
            ByteLength = 0;
        }

        public bool Contains(KeyValuePair<BEncodedString, BEncodedValue> item)
        {
            return Dictionary.ContainsKey(item.Key) && Dictionary[item.Key].Equals(item.Value);
        }

        public bool ContainsKey(BEncodedString key)
        {
            return Dictionary.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<BEncodedString, BEncodedValue>[] array, int arrayIndex)
        {
            Dictionary.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return Dictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(BEncodedString key)
        {
            var value = Dictionary[key];
            var result = Dictionary.Remove(key);
            if (result)
            {
                ByteLength -= key.ByteLength;
                ByteLength -= value.ByteLength;
            }

            return result;
        }

        public bool Remove(KeyValuePair<BEncodedString, BEncodedValue> item)
        {
            var value = Dictionary[item.Key];
            var result = Dictionary.Remove(item.Key);
            if (result)
            {
                ByteLength -= item.Key.ByteLength;
                ByteLength -= value.ByteLength;
            }

            return result;
        }

        public bool TryGetValue(BEncodedString key, out BEncodedValue value)
        {
            return Dictionary.TryGetValue(key, out value);
        }

        public BEncodedValue this[BEncodedString key]
        {
            get { return Dictionary[key]; }
            set { Dictionary[key] = value; }
        }

        public ICollection<BEncodedString> Keys
        {
            get { return Dictionary.Keys; }
        }

        public ICollection<BEncodedValue> Values
        {
            get { return Dictionary.Values; }
        }

        public IEnumerator<KeyValuePair<BEncodedString, BEncodedValue>> GetEnumerator()
        {
            return Dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Dictionary.GetEnumerator();
        }

        public override int Encode(byte[] buffer, int offset)
        {
            var written = 0;

            buffer[offset] = DictionaryStartDelimiter;
            written++;

            foreach (var keypair in this)
            {
                written += keypair.Key.Encode(buffer, offset + written);
                written += keypair.Value.Encode(buffer, offset + written);
            }

            buffer[offset + written] = DictionaryEndDelimiter;
            written++;
            return written;
        }

        internal override void Decode()
        {
            BEncodedString oldkey = null;

            if (Reader.ReadByte() != DictionaryStartDelimiter)
                throw new BEncodingException(
                    $"Invalid data. Expected {DictionaryStartDelimiter}, found {Reader.PeekByte()}");

            while (Reader.PeekByte() != -1 && Reader.PeekByte() != DictionaryEndDelimiter)
            {
                var key = (BEncodedString) Decode(Reader);

                if (oldkey != null && oldkey.CompareTo(key) > 0)
                    throw new BEncodingException(
                        $"Invalid dictionary: Attributes are not ordered correctly. Old key: {oldkey}, New key: {key}");

                oldkey = key;
                var value = Decode(Reader);
                Dictionary.Add(key, value);
            }

            if (Reader.ReadByte() != DictionaryEndDelimiter)
                throw new BEncodingException(
                    $"Invalid data. Expected {DictionaryEndDelimiter}, found {Reader.PeekByte()}");
        }

        public override bool Equals(object obj)
        {
            var other = obj as BEncodedDictionary;

            if (Dictionary.Count != other?.Dictionary.Count)
                return false;

            foreach (var keypair in Dictionary)
            {
                if (!other.TryGetValue(keypair.Key, out var val))
                    return false;

                if (!keypair.Value.Equals(val))
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            var result = 0;
            foreach (var keypair in Dictionary)
            {
                result ^= keypair.Key.GetHashCode();
                result ^= keypair.Value.GetHashCode();
            }

            return result;
        }

        public override string ToString()
        {
            return Encoding.UTF8.GetString(Encode());
        }

        protected override int GetLength()
        {
            var length = 0;

            // Dictionaries have accountable beginning/ending delimiters
            if (Dictionary.Count > 1)
                ByteLength = 2;

            foreach (var kvp in Dictionary)
            {
                length += kvp.Key.ByteLength;
                length += kvp.Value.ByteLength;
            }

            return length;
        }
    }
}