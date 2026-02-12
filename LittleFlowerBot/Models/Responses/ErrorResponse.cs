namespace LittleFlowerBot.Models.Responses;

/// <summary>
/// 統一的錯誤回應格式
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// 錯誤類型（例如：ValidationError, NotFound, InternalError）
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// 錯誤標題（簡短描述）
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// HTTP 狀態碼
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 詳細的錯誤訊息
    /// </summary>
    public string? Detail { get; set; }

    /// <summary>
    /// 請求的追蹤 ID（用於日誌查詢）
    /// </summary>
    public string? TraceId { get; set; }

    /// <summary>
    /// 錯誤發生的時間戳記
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 額外的錯誤資訊（選用）
    /// </summary>
    public Dictionary<string, object>? Extensions { get; set; }

    /// <summary>
    /// 創建一般錯誤回應
    /// </summary>
    public static ErrorResponse Create(
        string type,
        string title,
        int status,
        string? detail = null,
        string? traceId = null)
    {
        return new ErrorResponse
        {
            Type = type,
            Title = title,
            Status = status,
            Detail = detail,
            TraceId = traceId
        };
    }

    /// <summary>
    /// 創建內部伺服器錯誤回應
    /// </summary>
    public static ErrorResponse InternalServerError(string? traceId = null)
    {
        return Create(
            "InternalError",
            "內部伺服器錯誤",
            500,
            "伺服器處理請求時發生錯誤，請稍後再試",
            traceId);
    }

    /// <summary>
    /// 創建驗證錯誤回應
    /// </summary>
    public static ErrorResponse ValidationError(string detail, string? traceId = null)
    {
        return Create(
            "ValidationError",
            "驗證錯誤",
            400,
            detail,
            traceId);
    }

    /// <summary>
    /// 創建找不到資源錯誤回應
    /// </summary>
    public static ErrorResponse NotFound(string detail, string? traceId = null)
    {
        return Create(
            "NotFound",
            "找不到資源",
            404,
            detail,
            traceId);
    }

    /// <summary>
    /// 創建未授權錯誤回應
    /// </summary>
    public static ErrorResponse Unauthorized(string? traceId = null)
    {
        return Create(
            "Unauthorized",
            "未授權",
            401,
            "您沒有權限存取此資源",
            traceId);
    }
}
