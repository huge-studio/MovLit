using System.Collections.Generic;
using System.Diagnostics;

namespace Huge.MovLit.Models
{
    [DebuggerDisplay("\"{Text}\" Tags:[{(Tags?.Count == 1 ? Tags[0] : (Tags?.Count ?? 0).ToString())}] Index:{Index}")]
    public class CustomInkChoice
    {
        public string Text { get; set; }
        public List<string> Tags { get; set; } = new();
        public int Index { get; set; }
        public string PathStringOnChoice { get; set; } 
    }
}
