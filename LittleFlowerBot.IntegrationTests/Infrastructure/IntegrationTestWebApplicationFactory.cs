using LittleFlowerBot.DbContexts;
using LittleFlowerBot.HealthChecks;
using LittleFlowerBot.Models.Game.GuessNumber;
using LittleFlowerBot.Models.Message;
using LittleFlowerBot.Models.Renderer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;
using Testcontainers.MongoDb;

namespace LittleFlowerBot.IntegrationTests.Infrastructure;

/// <summary>
/// 整合測試用的 WebApplicationFactory
/// 使用 Testcontainers 管理 Docker 容器
/// </summary>
public class IntegrationTestWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly MongoDbContainer _mongoContainer;

    public IntegrationTestWebApplicationFactory()
    {
        _mongoContainer = new MongoDbBuilder("mongo:7")
            .WithCleanUp(true)
            .Build();
    }

    /// <summary>
    /// 取得 MongoDB 連線字串
    /// </summary>
    public string MongoConnectionString => _mongoContainer.GetConnectionString();

    /// <summary>
    /// 暴露 DI 容器供測試存取
    /// </summary>
    public IServiceProvider ServiceProvider => Services;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            Console.WriteLine($"[測試] 設定測試 MongoDB 連接字符串: {MongoConnectionString}");

            var testConfig = new Dictionary<string, string?>
            {
                ["ConnectionStrings:MongoDB"] = MongoConnectionString,
                ["MongoDB:DatabaseName"] = "LittleFlowerBot_Test"
            };

            config.AddInMemoryCollection(testConfig);
        });

        builder.ConfigureTestServices(services =>
        {
            // 移除原有的 MongoDB 配置
            services.RemoveAll<IMongoClient>();
            services.RemoveAll<IMongoDatabase>();
            services.RemoveAll<MongoDbContext>();

            // 使用測試 MongoDB
            services.AddSingleton<IMongoClient>(new MongoClient(MongoConnectionString));
            services.AddSingleton(sp =>
                sp.GetRequiredService<IMongoClient>().GetDatabase("LittleFlowerBot_Test"));
            services.AddSingleton<MongoDbContext>();

            // 註冊測試健康檢查
            services.AddHealthChecks()
                .AddCheck(
                    "MongoDB",
                    new MongoDbHealthCheck(MongoConnectionString),
                    tags: new[] { "database", "mongodb" });

            // 註冊測試用假隨機數產生器
            services.RemoveAll<IRandomGenerator>();
            services.AddSingleton<FakeRandomGenerator>();
            services.AddScoped<IRandomGenerator>(sp => sp.GetRequiredService<FakeRandomGenerator>());

            // 註冊測試用好友檢查服務（預設回傳 true）
            services.RemoveAll<ILineUserService>();
            services.AddSingleton<FakeLineUserService>();
            services.AddScoped<ILineUserService>(sp => sp.GetRequiredService<FakeLineUserService>());

            // 註冊測試用 Renderer 替身
            services.AddSingleton<TestTextRenderer>();
            services.RemoveAll<IRendererFactory>();
            services.AddSingleton<IRendererFactory, TestRendererFactory>();
            services.RemoveAll<ITextRenderer>();
            services.AddScoped<ITextRenderer>(sp => sp.GetRequiredService<TestTextRenderer>());
            services.RemoveAll<IMessage>();
            services.AddScoped<IMessage>(sp => sp.GetRequiredService<TestTextRenderer>());
        });
    }

    /// <summary>
    /// 測試開始前啟動容器
    /// </summary>
    public async Task StartContainersAsync()
    {
        await _mongoContainer.StartAsync();
        Console.WriteLine($"✅ MongoDB 容器已啟動: {MongoConnectionString}");
    }

    /// <summary>
    /// 測試結束後停止容器
    /// </summary>
    public async Task StopContainersAsync()
    {
        await _mongoContainer.DisposeAsync();
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
