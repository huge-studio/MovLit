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

        //Browse
        public const string Tag = "Tag";
        public const string Category = "Category";
        public const string Browse = "Browse";
        public const string Mode = "Mode";


        public static List<string> GetAllFilters => new() { Trending, New, Top };

        public static string ToSlug(string filter)
        {
            if (string.Equals(filter, Trending, StringComparison.OrdinalIgnoreCase)) return "Trending";
            if (string.Equals(filter, New, StringComparison.OrdinalIgnoreCase)) return "Fresh";
            if (string.Equals(filter, Top, StringComparison.OrdinalIgnoreCase)) return "Top";
            // default to fresh
            return "Fresh";
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
