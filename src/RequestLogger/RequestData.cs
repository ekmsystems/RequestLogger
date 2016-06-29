﻿using System;
using System.Collections.Generic;

namespace RequestLogger
{
    public struct RequestData
    {
        public string HttpMethod;
        public Uri Url;
        public IDictionary<string, string[]> Header;
        public byte[] Content;
    }
}
