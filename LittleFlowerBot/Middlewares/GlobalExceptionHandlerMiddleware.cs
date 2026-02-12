using System.Net;
using System.Text.Json;
using LittleFlowerBot.Models.GameExceptions;
using LittleFlowerBot.Models.Responses;

namespace LittleFlowerBot.Middlewares;

/// <summary>
/// 全域例外處理中介軟體
/// </summary>
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = context.TraceIdentifier;

        // 記錄例外
        _logger.LogError(
            exception,
            "發生未處理的例外。TraceId: {TraceId}, Path: {Path}",
            traceId,
            context.Request.Path);

        // 根據例外類型創建適當的錯誤回應
        var errorResponse = CreateErrorResponse(exception, traceId);

        // 設定回應
        context.Response.StatusCode = errorResponse.Status;
        context.Response.ContentType = "application/json";

        // 序列化並寫入回應
        var json = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = _environment.IsDevelopment()
        });

        await context.Response.WriteAsync(json);
    }

    private ErrorResponse CreateErrorResponse(Exception exception, string traceId)
    {
        return exception switch
        {
            // 遊戲相關例外
            NotYourTurnException ex => ErrorResponse.Create(
                "GameError",
                "不是你的回合",
                400,
                ex.Message,
                traceId),

            PlayerExistException ex => ErrorResponse.Create(
                "GameError",
                "玩家已存在",
                400,
                ex.Message,
                traceId),

            NotYourChessException ex => ErrorResponse.Create(
                "GameError",
                "不是你的棋子",
                400,
                ex.Message,
                traceId),

            MoveInvalidException ex => ErrorResponse.Create(
                "GameError",
                "移動無效",
                400,
                ex.Message,
                traceId),

            CoordinateValidException ex => ErrorResponse.Create(
                "GameError",
                "座標無效",
                400,
                ex.Message,
                traceId),

            // 標準 .NET 例外（注意：更具體的例外要放在前面）
            ArgumentNullException ex => ErrorResponse.ValidationError(
                $"必要參數 '{ex.ParamName}' 不能為 null",
                traceId),

            ArgumentException ex => ErrorResponse.ValidationError(
                ex.Message,
                traceId),

            UnauthorizedAccessException => ErrorResponse.Unauthorized(traceId),

            KeyNotFoundException ex => ErrorResponse.NotFound(
                ex.Message,
                traceId),

            // 預設：內部伺服器錯誤
            _ => CreateInternalServerError(exception, traceId)
        };
    }

    private ErrorResponse CreateInternalServerError(Exception exception, string traceId)
    {
        var errorResponse = ErrorResponse.InternalServerError(traceId);

        // 只在開發環境中包含例外詳細資訊
        if (_environment.IsDevelopment())
        {
            errorResponse.Extensions = new Dictionary<string, object>
            {
                ["exceptionType"] = exception.GetType().Name,
                ["exceptionMessage"] = exception.Message,
                ["stackTrace"] = exception.StackTrace ?? string.Empty
            };
        }

        return errorResponse;
    }
}

/// <summary>
/// 全域例外處理中介軟體的擴充方法
/// </summary>
public static class GlobalExceptionHandlerMiddlewareExtensions
{
    /// <summary>
    /// 使用全域例外處理中介軟體
    /// </summary>
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }
}
