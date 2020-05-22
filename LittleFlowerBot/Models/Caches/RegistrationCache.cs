using System.Collections.Generic;

namespace LittleFlowerBot.Models.Caches
{
    public class RegistrationCache
    {
        private readonly Dictionary<string, string> _cache = new Dictionary<string, string>();

        public void Add(string guid, string userId)
        {
            _cache.Add(guid, userId);
        }

        public string GetSenderId(string guid)
        {
            return _cache.ContainsKey(guid) ? _cache[guid] : null;
        }
        
        public void Remove(string guid)
        {
            _cache.Remove(guid);
        }
    }
}