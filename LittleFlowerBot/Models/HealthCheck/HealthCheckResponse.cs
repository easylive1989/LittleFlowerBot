using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LittleFlowerBot.Models.HealthCheck;

/// <summary>
/// 健康檢查回應格式
/// </summary>
public class HealthCheckResponse
{
    /// <summary>
    /// 整體健康狀態
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// 總檢查時間（毫秒）
    /// </summary>
    public long TotalDuration { get; set; }

    /// <summary>
    /// 個別檢查項目
    /// </summary>
    public Dictionary<string, HealthCheckEntry> Checks { get; set; } = new();

    /// <summary>
    /// 檢查時間戳記
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 單一健康檢查項目
/// </summary>
public class HealthCheckEntry
{
    /// <summary>
    /// 狀態（Healthy, Degraded, Unhealthy）
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 檢查時間（毫秒）
    /// </summary>
    public long Duration { get; set; }

    /// <summary>
    /// 錯誤訊息（如果失敗）
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// 額外資料
    /// </summary>
    public Dictionary<string, object>? Data { get; set; }
}

/// <summary>
/// Health Check 回應寫入器
/// </summary>
public static class HealthCheckResponseWriter
{
    /// <summary>
    /// 將 HealthReport 轉換為 JSON 回應
    /// </summary>
    public static async Task WriteResponse(HttpContext context, HealthReport report)
    {
        var response = new HealthCheckResponse
        {
            Status = report.Status.ToString(),
            TotalDuration = report.TotalDuration.Milliseconds,
            Checks = report.Entries.ToDictionary(
                entry => entry.Key,
                entry => new HealthCheckEntry
                {
                    Status = entry.Value.Status.ToString(),
                    Description = entry.Value.Description,
                    Duration = entry.Value.Duration.Milliseconds,
                    Error = entry.Value.Exception?.Message,
                    Data = entry.Value.Data.Count > 0 ?
                        entry.Value.Data.ToDictionary(d => d.Key, d => d.Value) :
                        null
                })
        };

        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(response);
    }
}
