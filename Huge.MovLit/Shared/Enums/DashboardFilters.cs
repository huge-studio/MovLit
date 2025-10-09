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
        
        // We can eventually add more filters based on license, remix, etc.
    }
}
