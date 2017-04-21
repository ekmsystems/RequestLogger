using System.IO;

namespace RequestLogger.Web
{
    internal static class StreamExtensions
    {
        public static void CopyTo(this Stream src, Stream dst, int bufferSize = 1024000)
        {
            int bytesRead;
            var buffer = new byte[bufferSize];

            while ((bytesRead = src.Read(buffer, 0, bufferSize)) > 0)
                dst.Write(buffer, 0, bytesRead);
        }

        public static byte[] ReadStream(this Stream stream)
        {
            using (var ms = new MemoryStream())
            {
                if (stream.CanRead && stream.CanSeek)
                {
                    stream.Seek(0, SeekOrigin.Begin); 
                    stream.CopyTo(ms);
                    stream.Seek(0, SeekOrigin.Begin);
                }

                return ms.ToArray();
            }
        }
    }
}