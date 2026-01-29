using Oqtane.Modules;
using System;

namespace Huge.MovLit.Services
{
    public class TagFilterService : IService
    {
        public event Action<string?> TagChanged;
        public void SetTag(string? tag)
        {
            TagChanged?.Invoke(tag);
        }
    }
}
