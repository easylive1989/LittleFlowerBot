using LittleFlowerBot.DbContexts;
using LittleFlowerBot.HealthChecks;
using LittleFlowerBot.Middlewares;
using LittleFlowerBot.Models.Caches;
using LittleFlowerBot.Models.Game;
using LittleFlowerBot.Models.Game.BoardGame.ChessGames.ChineseChess;
using LittleFlowerBot.Models.Game.BoardGame.KiGames.Gomoku;
using LittleFlowerBot.Models.Game.BoardGame.KiGames.TicTacToe;
using LittleFlowerBot.Models.Game.GuessNumber;
using LittleFlowerBot.Models.HealthCheck;
using LittleFlowerBot.Models.Message;
using LittleFlowerBot.Models.Renderer;
using LittleFlowerBot.Repositories;
using LittleFlowerBot.Services;
using LittleFlowerBot.Services.EventHandler;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 取得環境變數
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
var redisUrl = Environment.GetEnvironmentVariable("HEROKU_REDIS_MAUVE_URL");

Console.WriteLine($"Database url: {databaseUrl}");
Console.WriteLine($"Redis url: {redisUrl}");

// 設定 DbContext
builder.Services.AddDbContext<LittleFlowerBotContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// 設定 Redis 快取
var redisConnectionString = RedisConfigurationService.GetRedisConnectionStringFromEnvironment();

if (!string.IsNullOrEmpty(redisConnectionString))
{
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.InstanceName = "LittleFlowerBot";
        options.Configuration = redisConnectionString;
    });
    Console.WriteLine("Redis 快取已成功配置");
}
else
{
    Console.WriteLine("警告: Redis URL 未設定，將使用記憶體快取作為後備方案");
    // 在開發環境中可以使用記憶體快取作為後備
    builder.Services.AddDistributedMemoryCache();
}

// 設定基本服務
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();
builder.Services.AddControllers();

// 註冊快取服務
builder.Services.AddSingleton<RegistrationCache>();
builder.Services.AddSingleton<IGameBoardCache, GameBoardCache>();

// 註冊工廠服務
builder.Services.AddScoped<IGameFactory, GameFactory>();
builder.Services.AddScoped<IRendererFactory, RendererFactory>();

// 註冊 Repository
builder.Services.AddScoped<IBoardGameResultsRepository, BoardGameResultsRepository>();
builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();

// 註冊遊戲服務
builder.Services.AddScoped<TicTacToeGame>();
builder.Services.AddScoped<GomokuGame>();
builder.Services.AddScoped<ChineseChessGame>();
builder.Services.AddScoped<GuessNumberGame>();

// 註冊事件處理器
builder.Services.AddScoped<ILineEventHandler, GameHandler>();
builder.Services.AddScoped<ILineEventHandler, RecordHandler>();
builder.Services.AddScoped<GameHandler, GameHandler>();
builder.Services.AddScoped<ILineEventHandler, RegistrationHandler>();

// 註冊 Renderer
builder.Services.AddScoped<ConsoleRenderer>();
builder.Services.AddScoped<LineNotifySender>();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<ITextRenderer, ConsoleRenderer>();
    builder.Services.AddScoped<ILineNotifySubscription, ConsoleRenderer>();
    builder.Services.AddScoped<IMessage, ConsoleRenderer>();
}
else
{
    builder.Services.AddScoped<ITextRenderer, LineNotifySender>();
    builder.Services.AddScoped<ILineNotifySubscription, LineNotifySubscription>();
    builder.Services.AddScoped<IMessage, LineMessage>();
}

// 設定 Health Checks
var healthChecksBuilder = builder.Services.AddHealthChecks()
    // 資料庫健康檢查
    .AddNpgSql(
        builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "PostgreSQL",
        failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy,
        tags: new[] { "database", "postgresql" })

    // 應用程式健康檢查
    .AddCheck<ApplicationHealthCheck>(
        "Application",
        tags: new[] { "application", "basic" })

    // 記憶體健康檢查
    .AddCheck<MemoryHealthCheck>(
        "Memory",
        tags: new[] { "memory", "performance" });

// 只在 Redis 配置可用時才加入 Redis 健康檢查
if (!string.IsNullOrEmpty(redisConnectionString))
{
    healthChecksBuilder.AddRedis(
        redisConnectionString,
        name: "Redis",
        failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded,
        tags: new[] { "cache", "redis" });
}

var app = builder.Build();

// 設定 HTTP 請求管道

// 使用全域錯誤處理中介軟體（應該在管道的最前面）
app.UseGlobalExceptionHandler();

// 在開發環境中可以保留開發者例外頁面，但全域處理器會優先處理
if (app.Environment.IsDevelopment())
{
    // app.UseDeveloperExceptionPage(); // 已被全域錯誤處理器取代
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

// 設定 Health Check 端點

// 完整的健康檢查（包含所有檢查項目）
app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = HealthCheckResponseWriter.WriteResponse,
    AllowCachingResponses = false
});

// 就緒檢查（只檢查關鍵服務，用於容器編排）
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("database") || check.Tags.Contains("cache"),
    ResponseWriter = HealthCheckResponseWriter.WriteResponse,
    AllowCachingResponses = false
});

// 存活檢查（最基本的檢查，用於確認應用程式還活著）
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("application"),
    ResponseWriter = HealthCheckResponseWriter.WriteResponse,
    AllowCachingResponses = false
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// 執行資料庫遷移
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<LittleFlowerBotContext>();
    context.Database.Migrate();
}

app.Run();
