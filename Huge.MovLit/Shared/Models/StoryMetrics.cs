using System.Collections.Generic;

namespace Huge.MovLit.Models
{
    public class StoryMetrics
    {
        public int StoryId { get; set; }
        public int ViewCount { get; set; }
        public int LikeCount { get; set; }

        public List<int?> VisitorViewIds { get; set; } = new();
        public List<int?> VisitorLikeIds { get; set; } = new();

        public List<int?> UserViewIds { get; set; } = new();
        public List<int?> UserLikeIds { get; set; } = new();
    }
}
