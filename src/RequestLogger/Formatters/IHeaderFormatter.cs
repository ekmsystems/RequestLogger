using System.Collections.Generic;

namespace RequestLogger.Formatters
{
    public interface IHeaderFormatter
    {
        string Format(IDictionary<string, string[]> header);
    }
}
