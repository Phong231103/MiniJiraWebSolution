using Microsoft.Extensions.Caching.Memory;
using Web.Application.Common.Interfaces;

namespace Web.Infrastructure.Services
{
    public class InMemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;

        public InMemoryCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
        {
            // Không cần async nữa
            if (_cache.TryGetValue(key, out T? value))
            {
                return Task.FromResult(value);   // Hoặc ValueTask để tối ưu hơn
            }

            return Task.FromResult(default(T?));
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
        {
            var options = new MemoryCacheEntryOptions();

            // Nếu có thiết lập thời gian sống (TTL) thì cấu hình, nếu không thì lưu mãi mãi
            if (expiry.HasValue)
            {
                options.AbsoluteExpirationRelativeToNow = expiry.Value;
            }

            _cache.Set(key, value, options);
            await Task.CompletedTask;
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            _cache.Remove(key);
            await Task.CompletedTask;
        }
    }
}
