using System.IO;

namespace RequestLogger.Web
{
    internal class ResponseFilterStream : Stream
    {
        private readonly Stream _stream;
        private readonly MemoryStream _copyStream;

        public ResponseFilterStream(Stream stream)
        {
            _stream = stream;
            _copyStream = new MemoryStream();
        }

        public byte[] ReadStream()
        {
            lock (_stream)
            {
                var pPosition = _copyStream.Position;

                try
                {
                    _copyStream.Position = 0;

                    return _copyStream.ToArray();
                }
                finally
                {
                    _copyStream.Position = pPosition;
                }
            }
        }

        public override void Flush()
        {
            _stream.Flush();
            _copyStream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _stream.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _stream.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _stream.Write(buffer, offset, count);
            _copyStream.Write(buffer, offset, count);
        }

        public override bool CanRead
        {
            get { return _stream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return _stream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return _stream.CanWrite; }
        }

        public override long Length
        {
            get { return _stream.Length; }
        }

        public override long Position
        {
            get { return _stream.Position; }
            set { _stream.Position = value; }
        }
    }
}
