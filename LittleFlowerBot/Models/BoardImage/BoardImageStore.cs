using Microsoft.Extensions.Caching.Memory;

namespace LittleFlowerBot.Models.BoardImage
{
    public class BoardImageStore : IBoardImageStore
    {
        private readonly IMemoryCache _cache;
        private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(5);

        public BoardImageStore(IMemoryCache cache)
        {
            _cache = cache;
        }

        public string Save(byte[] imageData)
        {
            var imageId = Guid.NewGuid().ToString("N");
            _cache.Set($"board-image:{imageId}", imageData, CacheExpiration);
            return imageId;
        }

        public byte[]? Get(string imageId)
        {
            return _cache.TryGetValue($"board-image:{imageId}", out byte[]? imageData) ? imageData : null;
        }
    }
}
