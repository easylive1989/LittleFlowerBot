using LittleFlowerBot.DbContexts;
using LittleFlowerBot.HealthChecks;
using LittleFlowerBot.Middlewares;
using LittleFlowerBot.Models.Caches;
using LittleFlowerBot.Models.Game;
using LittleFlowerBot.Models.Game.BoardGame.ChessGames.ChineseChess;
using LittleFlowerBot.Models.Game.BoardGame.KiGames.Gomoku;
using LittleFlowerBot.Models.Game.BoardGame.KiGames.TicTacToe;
using LittleFlowerBot.Models.Game.Battleship;
using LittleFlowerBot.Models.Game.GuessNumber;
using LittleFlowerBot.Models.HealthCheck;
using LittleFlowerBot.Models.Message;
using LittleFlowerBot.Models.Renderer;
using LittleFlowerBot.Repositories;
using LittleFlowerBot.Services.EventHandler;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// 設定 MongoDB（優先使用環境變數 MONGODB_URI）
var mongoConnectionString = Environment.GetEnvironmentVariable("MONGODB_URI")
    ?? builder.Configuration.GetConnectionString("MongoDB")
    ?? "mongodb://localhost:27017";
var mongoDatabaseName = builder.Configuration.GetValue<string>("MongoDB:DatabaseName") ?? "LittleFlowerBot";

builder.Services.AddSingleton<IMongoClient>(new MongoClient(mongoConnectionString));
builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IMongoClient>().GetDatabase(mongoDatabaseName));
builder.Services.AddSingleton<MongoDbContext>();

// 設定基本服務
builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddRazorPages();

// 註冊快取服務
builder.Services.AddScoped<IGameBoardCache, GameBoardCache>();

// 註冊工廠服務
builder.Services.AddScoped<IGameFactory, GameFactory>();
builder.Services.AddScoped<IRendererFactory, RendererFactory>();

// 註冊 Repository
builder.Services.AddScoped<IBoardGameResultsRepository, BoardGameResultsRepository>();

// 註冊隨機數產生器
builder.Services.AddScoped<IRandomGenerator, RandomGenerator>();

// 註冊遊戲服務
builder.Services.AddScoped<TicTacToeGame>();
builder.Services.AddScoped<GomokuGame>();
builder.Services.AddScoped<ChineseChessGame>();
builder.Services.AddScoped<GuessNumberGame>();
builder.Services.AddScoped<BattleshipGame>();

// 註冊事件處理器
builder.Services.AddScoped<ILineEventHandler, GameHandler>();
builder.Services.AddScoped<ILineEventHandler, RecordHandler>();
builder.Services.AddScoped<GameHandler, GameHandler>();

// 註冊 Renderer
builder.Services.AddScoped<ConsoleRenderer>();
builder.Services.AddScoped<BufferedReplyRenderer>();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<ITextRenderer, ConsoleRenderer>();
    builder.Services.AddScoped<IMessage, ConsoleRenderer>();
}
else
{
    builder.Services.AddScoped<ITextRenderer, BufferedReplyRenderer>();
    builder.Services.AddScoped<IMessage, LineMessage>();
}

// 設定 Health Checks
var healthChecksBuilder = builder.Services.AddHealthChecks()
    // 應用程式健康檢查
    .AddCheck<ApplicationHealthCheck>(
        "Application",
        tags: new[] { "application", "basic" })

    // 記憶體健康檢查
    .AddCheck<MemoryHealthCheck>(
        "Memory",
        tags: new[] { "memory", "performance" });

// 在非測試環境中才加入資料庫健康檢查
// 測試環境會在測試工廠中註冊自己的健康檢查
if (!builder.Environment.IsEnvironment("Testing"))
{
    healthChecksBuilder.AddCheck(
        "MongoDB",
        new MongoDbHealthCheck(mongoConnectionString),
        failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy,
        tags: new[] { "database", "mongodb" });
}

var app = builder.Build();

// 設定 HTTP 請求管道

// 使用全域錯誤處理中介軟體（應該在管道的最前面）
app.UseGlobalExceptionHandler();

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

app.Run();

// 讓整合測試可以引用此專案
public partial class Program { }
