using System.Collections.Generic;

namespace RequestLogger
{
    public struct ResponseData
    {
        public int StatusCode;
        public string ReasonPhrase;
        public IDictionary<string, string[]> Header;
        public byte[] Content;
    }
}
