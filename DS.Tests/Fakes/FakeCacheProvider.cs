using System.Collections.Generic;
using DS.Domain.Interface;

namespace DS.Tests.Fakes
{
    public class FakeCacheProvider : ICacheProvider
    {
        private Dictionary<string, object> _cache = new Dictionary<string, object>();
        
        public bool Get<T>(string key, out T value)
        {

            try
            {
                if (_cache[key] == null)
                {
                    value = default(T);
                    return false;
                }

                value = (T)_cache[key];
            }
            catch
            {
                value = default(T);
                return false;
            }

            return true;
        }

        public void Set<T>(string key, T value)
        {
            Set<T>(key, value, 10);
        }

        public void Set<T>(string key, T value, int duration)
        {
            _cache.Add(key,value);
        }

        public void Clear(string key)
        {
            _cache.Clear();
        }

        public IEnumerable<KeyValuePair<string, object>> GetAll()
        {
            foreach (var item in _cache)
            {
                yield return new KeyValuePair<string, object>(item.Key as string, item.Value);
            }
        }
    }
}
