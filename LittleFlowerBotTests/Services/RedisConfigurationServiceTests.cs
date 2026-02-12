using LittleFlowerBot.Services;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace LittleFlowerBotTests.Services;

[TestFixture]
public class RedisConfigurationServiceTests
{
    [Test]
    public void ParseHerokuRedisUrl_ValidUrl_ReturnsCorrectConnectionString()
    {
        // Arrange
        var herokuUrl = "redis://:mypassword123@ec2-host.compute.amazonaws.com:12345";

        // Act
        var result = RedisConfigurationService.ParseHerokuRedisUrl(herokuUrl);

        // Assert
        ClassicAssert.AreEqual("ec2-host.compute.amazonaws.com:12345,password=mypassword123", result);
    }

    [Test]
    public void ParseHerokuRedisUrl_NullUrl_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            RedisConfigurationService.ParseHerokuRedisUrl(null));
    }

    [Test]
    public void ParseHerokuRedisUrl_EmptyUrl_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            RedisConfigurationService.ParseHerokuRedisUrl(""));
    }

    [Test]
    public void ParseHerokuRedisUrl_WhitespaceUrl_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            RedisConfigurationService.ParseHerokuRedisUrl("   "));
    }

    [Test]
    public void ParseHerokuRedisUrl_InvalidFormat_NoAtSymbol_ThrowsFormatException()
    {
        // Arrange
        var invalidUrl = "redis://:mypassword123";

        // Act & Assert
        Assert.Throws<FormatException>(() =>
            RedisConfigurationService.ParseHerokuRedisUrl(invalidUrl));
    }

    [Test]
    public void ParseHerokuRedisUrl_InvalidFormat_WrongPrefix_ThrowsFormatException()
    {
        // Arrange
        var invalidUrl = "http://:mypassword123@host:12345";

        // Act & Assert
        Assert.Throws<FormatException>(() =>
            RedisConfigurationService.ParseHerokuRedisUrl(invalidUrl));
    }

    [Test]
    public void ParseHerokuRedisUrl_InvalidFormat_EmptyPassword_ThrowsFormatException()
    {
        // Arrange
        var invalidUrl = "redis://:@host:12345";

        // Act & Assert
        Assert.Throws<FormatException>(() =>
            RedisConfigurationService.ParseHerokuRedisUrl(invalidUrl));
    }

    [Test]
    public void IsValidConnectionString_ValidString_ReturnsTrue()
    {
        // Arrange
        var validConnectionString = "localhost:6379,password=mypassword";

        // Act
        var result = RedisConfigurationService.IsValidConnectionString(validConnectionString);

        // Assert
        ClassicAssert.IsTrue(result);
    }

    [Test]
    public void IsValidConnectionString_NullString_ReturnsFalse()
    {
        // Act
        var result = RedisConfigurationService.IsValidConnectionString(null);

        // Assert
        ClassicAssert.IsFalse(result);
    }

    [Test]
    public void IsValidConnectionString_EmptyString_ReturnsFalse()
    {
        // Act
        var result = RedisConfigurationService.IsValidConnectionString("");

        // Assert
        ClassicAssert.IsFalse(result);
    }

    [Test]
    public void IsValidConnectionString_MissingColon_ReturnsFalse()
    {
        // Arrange
        var invalidConnectionString = "localhost,password=mypassword";

        // Act
        var result = RedisConfigurationService.IsValidConnectionString(invalidConnectionString);

        // Assert
        ClassicAssert.IsFalse(result);
    }

    [Test]
    public void IsValidConnectionString_MissingPassword_ReturnsFalse()
    {
        // Arrange
        var invalidConnectionString = "localhost:6379";

        // Act
        var result = RedisConfigurationService.IsValidConnectionString(invalidConnectionString);

        // Assert
        ClassicAssert.IsFalse(result);
    }

    [Test]
    public void ParseHerokuRedisUrl_UrlWithSpaces_TrimsCorrectly()
    {
        // Arrange
        var herokuUrl = "redis://: mypassword123 @ ec2-host.compute.amazonaws.com:12345 ";

        // Act
        var result = RedisConfigurationService.ParseHerokuRedisUrl(herokuUrl);

        // Assert
        ClassicAssert.AreEqual("ec2-host.compute.amazonaws.com:12345,password=mypassword123", result);
    }
}
