using Oqtane.Modules;
using System;

namespace Huge.MovLit.Services
{
    // Simple event-based service to coordinate tag filtering without navigation/query churn
    public class TagFilterService : IService
    {
        public event Action<string?> TagChanged;
        public void SetTag(string? tag)
        {
            TagChanged?.Invoke(tag);
        }
    }
}
