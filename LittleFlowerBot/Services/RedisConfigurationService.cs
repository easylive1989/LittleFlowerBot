namespace LittleFlowerBot.Services;

/// <summary>
/// Redis 配置服務，負責處理 Redis 連線字串的解析和配置
/// </summary>
public class RedisConfigurationService
{
    /// <summary>
    /// 解析 Heroku Redis URL 格式並轉換為 StackExchange.Redis 連線字串格式
    /// </summary>
    /// <param name="herokuRedisUrl">Heroku Redis URL (格式: redis://:password@host:port)</param>
    /// <returns>StackExchange.Redis 格式的連線字串 (格式: host:port,password=password)</returns>
    /// <exception cref="ArgumentException">當 URL 為空或格式不正確時拋出</exception>
    public static string ParseHerokuRedisUrl(string? herokuRedisUrl)
    {
        if (string.IsNullOrWhiteSpace(herokuRedisUrl))
        {
            throw new ArgumentException("Redis URL 不能為空或 null", nameof(herokuRedisUrl));
        }

        try
        {
            // Heroku Redis URL 格式: redis://:password@host:port
            var urlParts = herokuRedisUrl.Split('@');

            if (urlParts.Length != 2)
            {
                throw new FormatException("Redis URL 格式不正確，應為 redis://:password@host:port");
            }

            // 提取密碼部分
            var authPart = urlParts[0];
            var passwordPrefix = "redis://:";

            if (!authPart.StartsWith(passwordPrefix))
            {
                throw new FormatException("Redis URL 必須以 'redis://:' 開頭");
            }

            var password = authPart[passwordPrefix.Length..].Trim();

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new FormatException("Redis 密碼不能為空");
            }

            // 提取主機和端口部分
            var hostPort = urlParts[1].Trim();

            if (string.IsNullOrWhiteSpace(hostPort))
            {
                throw new FormatException("Redis 主機和端口不能為空");
            }

            // 返回 StackExchange.Redis 格式的連線字串
            return $"{hostPort},password={password}";
        }
        catch (Exception ex) when (ex is not ArgumentException and not FormatException)
        {
            throw new FormatException($"解析 Redis URL 時發生錯誤: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// 從環境變數中取得並解析 Redis URL
    /// </summary>
    /// <param name="environmentVariableName">環境變數名稱</param>
    /// <returns>解析後的 Redis 連線字串，如果環境變數不存在則返回 null</returns>
    public static string? GetRedisConnectionStringFromEnvironment(string environmentVariableName = "HEROKU_REDIS_MAUVE_URL")
    {
        var redisUrl = Environment.GetEnvironmentVariable(environmentVariableName);

        if (string.IsNullOrWhiteSpace(redisUrl))
        {
            return null;
        }

        return ParseHerokuRedisUrl(redisUrl);
    }

    /// <summary>
    /// 驗證 Redis 連線字串格式是否正確
    /// </summary>
    /// <param name="connectionString">Redis 連線字串</param>
    /// <returns>是否為有效的連線字串</returns>
    public static bool IsValidConnectionString(string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return false;
        }

        // 基本驗證：應該包含主機和密碼
        return connectionString.Contains(':') && connectionString.Contains("password=");
    }
}
