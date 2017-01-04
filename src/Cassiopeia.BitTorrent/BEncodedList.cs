using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cassiopeia.BitTorrent
{
    public class BEncodedList : BEncodedValue, IList<BEncodedValue>
    {
        protected List<BEncodedValue> List;

        public BEncodedList(BitTorrentReader reader)
            : this(new List<BEncodedValue>(), reader)
        {
        }

        public BEncodedList(int capacity, BitTorrentReader reader)
            : this(new List<BEncodedValue>(capacity), reader)
        {
        }

        public BEncodedList(IEnumerable<BEncodedValue> list, BitTorrentReader reader) :base(reader)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            List = new List<BEncodedValue>(list);
        }

        private BEncodedList(List<BEncodedValue> list, BitTorrentReader reader) : base(reader)
        {
            List = list;
        }

        public void Add(BEncodedValue item)
        {
            List.Add(item);
        }

        public void Clear()
        {
            List.Clear();
        }

        public bool Contains(BEncodedValue item)
        {
            return List.Contains(item);
        }

        public void CopyTo(BEncodedValue[] array, int arrayIndex)
        {
            List.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return List.Count; }
        }

        public int IndexOf(BEncodedValue item)
        {
            return List.IndexOf(item);
        }

        public void Insert(int index, BEncodedValue item)
        {
            List.Insert(index, item);
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(BEncodedValue item)
        {
            return List.Remove(item);
        }

        public void RemoveAt(int index)
        {
            List.RemoveAt(index);
        }

        public BEncodedValue this[int index]
        {
            get { return List[index]; }
            set { List[index] = value; }
        }

        public IEnumerator<BEncodedValue> GetEnumerator()
        {
            return List.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override int Encode(byte[] buffer, int offset)
        {
            var written = 0;
            buffer[offset] = ListStartDelimiter;
            written++;
            written = List.Aggregate(written, (current, t) => current + t.Encode(buffer, offset + current));
            buffer[offset + written] = ListEndDelimiter;
            written++;
            return written;
        }

        internal override void Decode()
        {
            if (Reader.ReadByte() != ListStartDelimiter)
                throw new BEncodingException($"Invalid data. Expected {ListStartDelimiter}, found {Reader.PeekByte()}");

            while (Reader.PeekByte() != -1 && Reader.PeekByte() != ListEndDelimiter)
                List.Add(Decode(Reader));

            if (Reader.ReadByte() != ListEndDelimiter)
                throw new BEncodingException($"Invalid data. Expected {ListEndDelimiter}, found {Reader.PeekByte()}");
        }

        protected override int GetLength()
        {
            var length = 0;

            // Lists have accountable beginning/ending delimiters
            if (List.Count > 0)
            length += 2;

            length += List.Sum(t => t.ByteLength);

            return length;
        }

        public override bool Equals(object obj)
        {
            var other = obj as BEncodedList;

            if (other == null)
                return false;

            return !List.Where((t, i) => !t.Equals(other.List[i])).Any();
        }

        public override int GetHashCode()
        {
            return List.Aggregate(0, (current, t) => current ^ t.GetHashCode());
        }

        public override string ToString()
        {
            return Encoding.UTF8.GetString(Encode());
        }

        public void AddRange(IEnumerable<BEncodedValue> collection)
        {
            List.AddRange(collection);
        }
    }
}