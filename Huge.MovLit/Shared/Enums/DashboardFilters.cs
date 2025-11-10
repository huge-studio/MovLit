using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huge.MovLit.Enums
{
    public class DashboardFilters
    {
        public const string Trending = "Trending Stories";
        public const string New = "Fresh Stories";
        public const string Top = "Top Stories";

        public static List<string> GetAllFilters => new() { Trending, New, Top };

        // Slug mapping helpers (used for query parameters)
        // trending => Trending Stories, fresh => Fresh Stories, top => Top Stories
        public static string ToSlug(string filter)
        {
            if (string.Equals(filter, Trending, StringComparison.OrdinalIgnoreCase)) return "trending";
            if (string.Equals(filter, New, StringComparison.OrdinalIgnoreCase)) return "fresh";
            if (string.Equals(filter, Top, StringComparison.OrdinalIgnoreCase)) return "top";
            // default to fresh
            return "fresh";
        }

        public static string FromSlug(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug)) return New;
            slug = slug.ToLowerInvariant();
            return slug switch
            {
                "trending" => Trending,
                "fresh" => New,
                "top" => Top,
                _ => New
            };
        }
    }
}
