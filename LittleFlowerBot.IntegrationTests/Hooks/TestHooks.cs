using BoDi;
using LittleFlowerBot.DbContexts;
using LittleFlowerBot.IntegrationTests.Infrastructure;
using LittleFlowerBot.Models.Caches;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using TechTalk.SpecFlow;

namespace LittleFlowerBot.IntegrationTests.Hooks;

/// <summary>
/// SpecFlow 測試 Hooks
/// 管理測試生命週期和依賴注入
/// </summary>
[Binding]
public class TestHooks
{
    private static IntegrationTestWebApplicationFactory? _factory;
    private static HttpClient? _httpClient;

    /// <summary>
    /// 在所有測試執行前執行一次
    /// 啟動 Docker 容器和測試伺服器
    /// </summary>
    [BeforeTestRun]
    public static async Task BeforeTestRun()
    {
        Console.WriteLine("🐳 正在啟動 Docker 容器...");

        _factory = new IntegrationTestWebApplicationFactory();
        await _factory.StartContainersAsync();

        _httpClient = _factory.CreateClient();

        Console.WriteLine("✅ PostgreSQL 容器已啟動");
        Console.WriteLine($"   連線字串: {_factory.PostgresConnectionString}");
        Console.WriteLine("✅ Redis 容器已啟動");
        Console.WriteLine($"   連線字串: {_factory.RedisConnectionString}");
        Console.WriteLine("✅ 測試伺服器已啟動");
    }

    /// <summary>
    /// 在每個情境執行前執行
    /// 注入依賴到情境容器，並清理測試狀態
    /// </summary>
    [BeforeScenario]
    public void BeforeScenario(IObjectContainer objectContainer)
    {
        if (_factory == null || _httpClient == null)
        {
            throw new InvalidOperationException("Test factory not initialized");
        }

        // 將依賴注入到情境容器
        objectContainer.RegisterInstanceAs(_factory);
        objectContainer.RegisterInstanceAs(_httpClient);

        // 清除測試訊息記錄
        TestTextRenderer.Clear();

        // 清理遊戲快取（本地記憶體 + Redis），避免場景間遊戲狀態汙染
        // 先清理本地記憶體快取
        var gameBoardCache = _factory.ServiceProvider.GetRequiredService<IGameBoardCache>();
        foreach (var gameId in gameBoardCache.GetGameIdList())
        {
            gameBoardCache.Remove(gameId).GetAwaiter().GetResult();
        }
        // 再清理 Redis（FLUSHDB 確保不留殘餘）
        var redisConnectionString = $"{_factory.RedisConnectionString},password=test_redis_password,allowAdmin=true";
        using var redis = ConnectionMultiplexer.Connect(redisConnectionString);
        redis.GetServer(redis.GetEndPoints()[0]).FlushDatabase();

        // 清理資料庫測試資料，避免場景間資料汙染
        using var scope = _factory.ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LittleFlowerBotContext>();
        dbContext.BoardGameGameResults.RemoveRange(dbContext.BoardGameGameResults);
        dbContext.SaveChanges();
    }

    /// <summary>
    /// 在所有測試執行後執行一次
    /// 停止 Docker 容器和測試伺服器
    /// </summary>
    [AfterTestRun]
    public static async Task AfterTestRun()
    {
        Console.WriteLine("🧹 正在清理測試環境...");

        _httpClient?.Dispose();

        if (_factory != null)
        {
            await _factory.StopContainersAsync();
            _factory.Dispose();
        }

        Console.WriteLine("✅ 測試環境已清理完成");
    }
}
