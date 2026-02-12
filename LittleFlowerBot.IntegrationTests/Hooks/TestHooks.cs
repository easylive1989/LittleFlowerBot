using BoDi;
using LittleFlowerBot.IntegrationTests.Infrastructure;
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
    /// 注入依賴到情境容器
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
