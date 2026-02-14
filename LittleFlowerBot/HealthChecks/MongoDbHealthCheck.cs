using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LittleFlowerBot.HealthChecks;

public class MongoDbHealthCheck : IHealthCheck
{
    private readonly string _connectionString;

    public MongoDbHealthCheck(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase("admin");
            await database.RunCommandAsync<BsonDocument>(
                new BsonDocument("ping", 1), cancellationToken: cancellationToken);

            return HealthCheckResult.Healthy("MongoDB 連線正常");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("MongoDB 連線失敗", ex);
        }
    }
}
