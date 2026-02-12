using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LittleFlowerBot.HealthChecks;

/// <summary>
/// 記憶體健康檢查
/// </summary>
public class MemoryHealthCheck : IHealthCheck
{
    private readonly ILogger<MemoryHealthCheck> _logger;
    private readonly long _thresholdBytes;

    /// <summary>
    /// 建構子
    /// </summary>
    /// <param name="logger">日誌記錄器</param>
    /// <param name="thresholdMB">記憶體閾值（MB），預設 1024MB</param>
    public MemoryHealthCheck(
        ILogger<MemoryHealthCheck> logger,
        long thresholdMB = 1024)
    {
        _logger = logger;
        _thresholdBytes = thresholdMB * 1024 * 1024;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var gcInfo = GC.GetGCMemoryInfo();
            var allocated = GC.GetTotalMemory(forceFullCollection: false);
            var allocatedMB = allocated / (1024 * 1024);

            var data = new Dictionary<string, object>
            {
                ["allocatedMB"] = allocatedMB,
                ["gen0Collections"] = GC.CollectionCount(0),
                ["gen1Collections"] = GC.CollectionCount(1),
                ["gen2Collections"] = GC.CollectionCount(2),
                ["totalAvailableMemoryMB"] = gcInfo.TotalAvailableMemoryBytes / (1024 * 1024),
                ["heapSizeMB"] = gcInfo.HeapSizeBytes / (1024 * 1024)
            };

            if (allocated > _thresholdBytes)
            {
                _logger.LogWarning(
                    "記憶體使用量超過閾值: {AllocatedMB} MB (閾值: {ThresholdMB} MB)",
                    allocatedMB,
                    _thresholdBytes / (1024 * 1024));

                return Task.FromResult(
                    HealthCheckResult.Degraded(
                        $"記憶體使用量較高: {allocatedMB} MB",
                        data: data));
            }

            return Task.FromResult(
                HealthCheckResult.Healthy(
                    $"記憶體使用正常: {allocatedMB} MB",
                    data: data));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "記憶體健康檢查失敗");

            return Task.FromResult(
                HealthCheckResult.Unhealthy(
                    "記憶體健康檢查失敗",
                    ex));
        }
    }
}
