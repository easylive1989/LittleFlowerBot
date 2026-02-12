using LittleFlowerBot.DbContexts;
using LittleFlowerBot.Models.Message;
using LittleFlowerBot.Models.Renderer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace LittleFlowerBot.IntegrationTests.Infrastructure;

/// <summary>
/// 整合測試用的 WebApplicationFactory
/// 使用 Testcontainers 管理 Docker 容器
/// </summary>
public class IntegrationTestWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly PostgreSqlContainer _postgresContainer;
    private readonly RedisContainer _redisContainer;

    public IntegrationTestWebApplicationFactory()
    {
        // 配置 PostgreSQL 容器
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:15-alpine")
            .WithDatabase("littleflowerbot_test")
            .WithUsername("test_user")
            .WithPassword("test_password")
            .WithCleanUp(true)
            .Build();

        // 配置 Redis 容器（設定測試密碼）
        _redisContainer = new RedisBuilder()
            .WithImage("redis:7-alpine")
            .WithCommand("--requirepass", "test_redis_password")
            .WithCleanUp(true)
            .Build();
    }

    /// <summary>
    /// 取得 PostgreSQL 連線字串
    /// </summary>
    public string PostgresConnectionString => _postgresContainer.GetConnectionString();

    /// <summary>
    /// 取得 Redis 連線字串
    /// </summary>
    public string RedisConnectionString => _redisContainer.GetConnectionString();

    /// <summary>
    /// 暴露 DI 容器供測試存取（用於查詢 DB、Cache 等）
    /// </summary>
    public IServiceProvider ServiceProvider => Services;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // 使用測試環境
        builder.UseEnvironment("Testing");

        // 配置測試用的連接字符串 - 必須先設置，在服務註冊之前
        builder.ConfigureAppConfiguration((context, config) =>
        {
            Console.WriteLine($"[測試] 設定測試資料庫連接字符串: {PostgresConnectionString}");
            Console.WriteLine($"[測試] 設定測試 Redis 連接字符串: {RedisConnectionString}");

            // 清除現有的配置並設定測試配置
            var testConfig = new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = PostgresConnectionString
            };

            // 使用最高優先級添加配置
            config.AddInMemoryCollection(testConfig);
        });

        builder.ConfigureTestServices(services =>
        {
            // 移除原有的 DbContext 配置
            services.RemoveAll(typeof(DbContextOptions<LittleFlowerBotContext>));

            // 使用測試資料庫
            services.AddDbContext<LittleFlowerBotContext>(options =>
            {
                options.UseNpgsql(PostgresConnectionString);
            });

            // 註冊測試健康檢查，使用測試連接字符串
            // 使用與生產環境相同的名稱，因為生產環境的健康檢查在測試環境中不會註冊
            services.AddHealthChecks()
                .AddNpgSql(
                    PostgresConnectionString,
                    name: "PostgreSQL",
                    tags: new[] { "database", "postgresql" })
                .AddRedis(
                    $"{RedisConnectionString},password=test_redis_password",
                    name: "Redis",
                    tags: new[] { "cache", "redis" });

            // 註冊測試用 Renderer 替身
            services.AddSingleton<TestTextRenderer>();
            services.RemoveAll<IRendererFactory>();
            services.AddSingleton<IRendererFactory, TestRendererFactory>();
            services.RemoveAll<ITextRenderer>();
            services.AddScoped<ITextRenderer>(sp => sp.GetRequiredService<TestTextRenderer>());
            services.RemoveAll<IMessage>();
            services.AddScoped<IMessage>(sp => sp.GetRequiredService<TestTextRenderer>());
            services.RemoveAll<ILineNotifySubscription>();
            services.AddScoped<ILineNotifySubscription>(sp => sp.GetRequiredService<TestTextRenderer>());

            // 確保資料庫已建立並執行遷移
            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<LittleFlowerBotContext>();
            dbContext.Database.Migrate();
        });
    }

    /// <summary>
    /// 測試開始前啟動容器
    /// </summary>
    public async Task StartContainersAsync()
    {
        await _postgresContainer.StartAsync();
        await _redisContainer.StartAsync();

        // Testcontainers Redis 返回格式為 "localhost:6379"
        // 需要轉換為 Heroku 格式 "redis://:password@host:port"
        var redisHostPort = RedisConnectionString;
        var redisPassword = "test_redis_password"; // 與 Redis 容器配置的密碼一致
        var redisUrl = $"redis://:{redisPassword}@{redisHostPort}";

        // 設置環境變數，讓應用程式使用測試資料庫
        Environment.SetEnvironmentVariable("DATABASE_URL", PostgresConnectionString);
        Environment.SetEnvironmentVariable("HEROKU_REDIS_MAUVE_URL", redisUrl);
    }

    /// <summary>
    /// 測試結束後停止容器
    /// </summary>
    public async Task StopContainersAsync()
    {
        await _postgresContainer.DisposeAsync();
        await _redisContainer.DisposeAsync();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            StopContainersAsync().GetAwaiter().GetResult();
        }
        base.Dispose(disposing);
    }
}
