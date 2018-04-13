using System;
using System.Web.Caching;

namespace DS.Extensions
{
    public static class CacheExtensions
    {
        public static T GetOrStore<T>(this Cache cache, string key, Func<T> generator)
        {
            var result = cache[key];
            if (result == null)
            {
                result = generator();
                if (result != null) cache.Add(key, result, null, DateTime.UtcNow.AddYears(20), TimeSpan.Zero, CacheItemPriority.NotRemovable, null);
            }
            return (T)result;
        }

        public static T ResetCache<T>(this Cache cache, string key, Func<T> generator)
        {
            cache.Remove(key);
            
            return cache.GetOrStore(key, generator);
        }
    }
}
