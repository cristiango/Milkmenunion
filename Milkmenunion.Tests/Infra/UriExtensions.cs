using System;
using System.Linq;

namespace Milkmenunion.Tests.Infra
{
    public static class UriExtensions
    {
        public static Uri CreateRelative(this Uri uri, string relativeUri)
            => new Uri(uri, relativeUri);

        public static Uri CreateRelative(this Uri uri, Uri relativeUri)
            => new Uri(uri, relativeUri);

        public static Uri Append(this Uri uri, params string[] paths)
        {
            return new Uri(paths.Aggregate(uri.AbsoluteUri, (current, path) =>
                $"{current.TrimEnd('/')}/{path.TrimStart('/')}"));
        }
    }
}
