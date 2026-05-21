using StackExchange.Redis;
using System.Text.Json;
using Web.Application.Common.Interfaces;

namespace Web.Infrastructure.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase _db;

        public RedisCacheService(IConnectionMultiplexer connectionMultiplexer)
        {
            // Lấy database mặc định của Redis (thường là db0)
            _db = connectionMultiplexer.GetDatabase();
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            // Lấy dữ liệu thô từ Redis dưới dạng RedisValue (string hoặc byte[])
            var data = await _db.StringGetAsync(key);

            if (data.IsNullOrEmpty)
            {
                return default;
            }

            // Deserialize (chuyển đổi) từ JSON string về lại object T
            return JsonSerializer.Deserialize<T>(data!);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
        {
            // Serialize (chuyển đổi) object T thành JSON string để lưu vào Redis
            var jsonValue = JsonSerializer.Serialize(value);

            // Lưu vào Redis. Nếu có expiry thì set thời gian hết hạn
            await _db.StringSetAsync(key, jsonValue, expiry);
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            await _db.KeyDeleteAsync(key);
        }
    }
}
