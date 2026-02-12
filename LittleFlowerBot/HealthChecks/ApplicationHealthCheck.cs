using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics;
using System.Reflection;

namespace LittleFlowerBot.HealthChecks;

/// <summary>
/// 應用程式基本健康檢查
/// </summary>
public class ApplicationHealthCheck : IHealthCheck
{
    private readonly ILogger<ApplicationHealthCheck> _logger;
    private readonly DateTime _startTime;

    public ApplicationHealthCheck(ILogger<ApplicationHealthCheck> logger)
    {
        _logger = logger;
        _startTime = DateTime.UtcNow;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var uptime = DateTime.UtcNow - _startTime;
            var currentProcess = Process.GetCurrentProcess();
            var memoryUsed = currentProcess.WorkingSet64 / (1024 * 1024); // 轉換為 MB

            var data = new Dictionary<string, object>
            {
                ["version"] = Assembly.GetExecutingAssembly()
                    .GetName()
                    .Version?
                    .ToString() ?? "unknown",
                ["uptime"] = $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m",
                ["memoryUsedMB"] = memoryUsed,
                ["machineName"] = Environment.MachineName,
                ["osVersion"] = Environment.OSVersion.ToString(),
                ["processorCount"] = Environment.ProcessorCount
            };

            // 如果記憶體使用超過 500MB，標記為 Degraded
            if (memoryUsed > 500)
            {
                return Task.FromResult(
                    HealthCheckResult.Degraded(
                        $"應用程式記憶體使用較高: {memoryUsed} MB",
                        data: data));
            }

            return Task.FromResult(
                HealthCheckResult.Healthy(
                    "應用程式運行正常",
                    data: data));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "應用程式健康檢查失敗");

            return Task.FromResult(
                HealthCheckResult.Unhealthy(
                    "應用程式健康檢查失敗",
                    ex));
        }
    }
}
