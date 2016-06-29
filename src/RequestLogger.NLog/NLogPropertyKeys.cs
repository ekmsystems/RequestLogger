namespace RequestLogger.NLog
{
    public class NLogPropertyKeys
    {
        public string HttpMethod { get; set; }
        public string Uri { get; set; }
        public string RequestHeaders { get; set; }
        public string RequestBody { get; set; }
        public string StatusCode { get; set; }
        public string ReasonPhrase { get; set; }
        public string ResponseHeaders { get; set; }
        public string ResponseBody { get; set; }

        internal NLogPropertyKeys()
        {
            HttpMethod = "Method";
            Uri = "Uri";
            RequestHeaders = "RequestHeaders";
            RequestBody = "RequestBody";
            StatusCode = "StatusCode";
            ReasonPhrase = "ReasonPhrase";
            ResponseHeaders = "ResponseHeaders";
            ResponseBody = "ResponseBody";
        }
    }
}
