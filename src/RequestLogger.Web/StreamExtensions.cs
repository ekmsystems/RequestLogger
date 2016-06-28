using System.IO;

namespace RequestLogger.Web
{
    static class StreamExtensions
    {
        public static void CopyTo(this Stream src, Stream dst, int bufferSize = 1024000)
        {
            int bytesRead;
            var buffer = new byte[bufferSize];

            while ((bytesRead = src.Read(buffer, 0, bufferSize)) > 0)
            {
                dst.Write(buffer, 0, bytesRead);
            }
        }
    }
}
