using System;
using System.IO;

namespace Cassiopeia.BitTorrent
{
    public class BitTorrentReader : Stream
    {
        private readonly byte[] _peekedData;
        private readonly Stream _stream;
        private bool _hasDataPeek;

        public BitTorrentReader(Stream stream)
        {
            _stream = stream;
            _peekedData = new byte[1];
        }

        public override bool CanRead => _stream.CanRead;

        public override bool CanSeek => _stream.CanSeek;

        public override bool CanWrite => false;

        public override long Length => _stream.Length;

        public override long Position
        {
            get
            {
                if (_hasDataPeek)
                    return _stream.Position - 1;
                return _stream.Position;
            }
            set
            {
                if (value != Position)
                {
                    _hasDataPeek = false;
                    _stream.Position = value;
                }
            }
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public int PeekByte()
        {
            if (!_hasDataPeek)
                _hasDataPeek = Read(_peekedData, 0, 1) == 1;
            return _hasDataPeek ? _peekedData[0] : -1;
        }

        public override int ReadByte()
        {
            if (!_hasDataPeek) return base.ReadByte();

            _hasDataPeek = false;
            return _peekedData[0];
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var read = 0;
            if (_hasDataPeek && count > 0)
            {
                _hasDataPeek = false;
                buffer[offset] = _peekedData[0];
                offset++;
                count--;
                read++;
            }
            read += _stream.Read(buffer, offset, count);
            return read;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            _hasDataPeek = false;
            return _stream.Seek(_hasDataPeek && origin == SeekOrigin.Current ? offset - 1 : offset, origin);
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}