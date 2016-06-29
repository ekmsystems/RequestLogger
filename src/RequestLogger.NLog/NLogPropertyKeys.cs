namespace RequestLogger.NLog
{
    public class NLogPropertyKeys
    {
        public string HttpMethod { get; set; }
        public string Uri { get; set; }
        public string RequestHeader { get; set; }
        public string RequestBody { get; set; }
        public string StatusCode { get; set; }
        public string ReasonPhrase { get; set; }
        public string ResponseHeader { get; set; }
        public string ResponseBody { get; set; }

        internal NLogPropertyKeys()
        {
            HttpMethod = "HttpMethod";
            Uri = "Uri";
            RequestHeader = "RequestHeader";
            RequestBody = "RequestBody";
            StatusCode = "StatusCode";
            ReasonPhrase = "ReasonPhrase";
            ResponseHeader = "ResponseHeader";
            ResponseBody = "ResponseBody";
        }
    }
}
