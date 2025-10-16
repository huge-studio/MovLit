using Microsoft.EntityFrameworkCore;
using Oqtane.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huge.MovLit.Models
{
    public class Story : IAuditable
    {
        public int StoryId { get; set; }
        public int ModuleId { get; set; }
        public int PageId { get; set; }
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string Title { get; set; }
        public string CoverArtUrl { get; set; }
        public string Description { get; set; }
        public string InkJson { get; set; }
        public int UpvoteCount { get; set; }
        public List<string> Tags { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
    }

    public class StoryView : IAuditable
    {
        [Key]
        public int ViewId { get; set; }
        public int StoryId { get; set; }
        public int? UserId { get; set; }
        public int? VisitorId { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
    }

    public class StoryLike : IAuditable
    {
        [Key]
        public int LikeId { get; set; }
        public int StoryId { get; set; }
        public int? UserId { get; set; }
        public int? VisitorId { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
