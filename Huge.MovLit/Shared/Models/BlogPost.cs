using Oqtane.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Huge.MovLit.Models
{
    // Maps to dbo.Blog
    public class BlogPost : IAuditable
    {
        [Key]
        public int BlogId { get; set; }
        public int ModuleId { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public int? Thumbnail { get; set; } // matches DB int column (eg. -1 when none)
        public string AlternateText { get; set; }
        public int Views { get; set; }
        public bool AllowComments { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
