using System.Collections.Generic;
using System.Linq;

namespace RequestLogger.Formatters
{
    public class DefaultHeaderFormatter : IHeaderFormatter
    {
        public string Format(IDictionary<string, string[]> header)
        {
            var checkedHeader = header ?? new Dictionary<string, string[]>();
            var headerValues = string.Join(", ", checkedHeader.Keys
                .Select(x => string.Format("{0}: [{1}]", x, string.Join(", ", checkedHeader[x])))
                .ToArray());

            return string.Format("{{{0}}}", headerValues);
        }
    }
}
