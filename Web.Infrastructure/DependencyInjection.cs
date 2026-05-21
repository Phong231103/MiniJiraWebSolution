namespace Microsoft.Extensions.DependencyInjection;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using Web.Application.Common.Interfaces;
using Web.Domain.Repository;
using Web.Infrastructure.Data;
using Web.Infrastructure.Services;
using Web.Infrastructure.Shared;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        var cacheProvider = configuration["CacheSettings:Provider"];

        if (cacheProvider?.ToLower() == "redis")
        {
            var redisConnection = configuration["CacheSettings:RedisConnectionString"];

            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var config = ConfigurationOptions.Parse(redisConnection!);
                config.AbortOnConnectFail = false;
                return ConnectionMultiplexer.Connect(config);
            });

            services.AddScoped<ICacheService, RedisCacheService>();
        }
        else
        {
            services.AddMemoryCache();
            services.AddScoped<ICacheService, InMemoryCacheService>();
        }

        // Đăng kí JwtSettings từ appsettings.json
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        services.AddScoped<IPasswordHasher, PasswordHasher>();

        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        services.AddScoped<IEmailService, EmailService>();

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        return services;
    }
}
