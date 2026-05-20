namespace Microsoft.Extensions.DependencyInjection;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using Web.Application.Common.Interfaces;
using Web.Infrastructure.Data;
using Web.Infrastructure.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        var cacheProvider = configuration["CacheSettings:Provider"];

        if (cacheProvider?.ToLower() == "redis")
        {
            // Đăng ký Redis
            var redisConnection = configuration["CacheSettings:RedisConnectionString"];

            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var config = ConfigurationOptions.Parse(redisConnection!);
                config.AbortOnConnectFail = false; // Không crash app nếu không kết nối được Redis
                return ConnectionMultiplexer.Connect(config);
            });

            services.AddScoped<ICacheService, RedisCacheService>();
        }
        else
        {
            // Đăng ký In-Memory Cache (Dùng cho Dev local không có Docker)
            services.AddMemoryCache();
            services.AddScoped<ICacheService, InMemoryCacheService>();
        }

        // 1. Bind cấu hình JwtSettings từ appsettings.json
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

        // 2. Đăng ký IJwtTokenGenerator
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        return services;
    }
}
