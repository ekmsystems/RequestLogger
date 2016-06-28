using System.Collections.Generic;

namespace RequestLogger
{
    public struct RequestData
    {
        public string HttpMethod;
        public string Uri;
        public IDictionary<string, string[]> Headers;
        public byte[] Content;
    }
}
