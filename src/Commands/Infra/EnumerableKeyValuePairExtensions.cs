using System.Collections.Generic;
using System.Linq;

namespace MilkmenUnion.Commands.Infra
{
    public static class EnumerableKeyValuePairExtensions
    {
        public static string ToFormattedString(this IEnumerable<KeyValuePair<string, string>> list)
            => list == null ? string.Empty : string.Join(";", list.Select(x => x.Key + ":" + x.Value));
    }
}