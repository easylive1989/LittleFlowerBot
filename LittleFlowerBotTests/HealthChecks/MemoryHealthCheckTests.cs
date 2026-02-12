using LittleFlowerBot.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace LittleFlowerBotTests.HealthChecks;

[TestFixture]
public class MemoryHealthCheckTests
{
    private ILogger<MemoryHealthCheck> _logger = null!;

    [SetUp]
    public void Setup()
    {
        _logger = Substitute.For<ILogger<MemoryHealthCheck>>();
    }

    [Test]
    public async Task CheckHealthAsync_ReturnsHealthyResult()
    {
        // Arrange
        var healthCheck = new MemoryHealthCheck(_logger, thresholdMB: 10000); // 設定很高的閾值
        var context = new HealthCheckContext();

        // Act
        var result = await healthCheck.CheckHealthAsync(context);

        // Assert
        ClassicAssert.AreEqual(HealthStatus.Healthy, result.Status);
        ClassicAssert.IsNotNull(result.Description);
        ClassicAssert.IsNotNull(result.Data);
        ClassicAssert.IsTrue(result.Data.ContainsKey("allocatedMB"));
    }

    [Test]
    public async Task CheckHealthAsync_IncludesMemoryMetrics()
    {
        // Arrange
        var healthCheck = new MemoryHealthCheck(_logger);
        var context = new HealthCheckContext();

        // Act
        var result = await healthCheck.CheckHealthAsync(context);

        // Assert
        ClassicAssert.IsNotNull(result.Data);
        ClassicAssert.IsTrue(result.Data.ContainsKey("allocatedMB"));
        ClassicAssert.IsTrue(result.Data.ContainsKey("gen0Collections"));
        ClassicAssert.IsTrue(result.Data.ContainsKey("gen1Collections"));
        ClassicAssert.IsTrue(result.Data.ContainsKey("gen2Collections"));
        ClassicAssert.IsTrue(result.Data.ContainsKey("totalAvailableMemoryMB"));
        ClassicAssert.IsTrue(result.Data.ContainsKey("heapSizeMB"));
    }

    [Test]
    public async Task CheckHealthAsync_WithLowThreshold_ReturnsDegraded()
    {
        // Arrange
        var healthCheck = new MemoryHealthCheck(_logger, thresholdMB: 1); // 設定很低的閾值
        var context = new HealthCheckContext();

        // Act
        var result = await healthCheck.CheckHealthAsync(context);

        // Assert
        // 因為應用程式通常會使用超過 1MB，所以應該返回 Degraded
        ClassicAssert.AreEqual(HealthStatus.Degraded, result.Status);
        ClassicAssert.IsNotNull(result.Description);
        ClassicAssert.IsTrue(result.Description!.Contains("記憶體使用量較高"));
    }
}
