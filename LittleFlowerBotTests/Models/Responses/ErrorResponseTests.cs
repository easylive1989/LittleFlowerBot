using LittleFlowerBot.Models.Responses;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace LittleFlowerBotTests.Models.Responses;

[TestFixture]
public class ErrorResponseTests
{
    [Test]
    public void Create_WithAllParameters_ReturnsCorrectErrorResponse()
    {
        // Arrange
        var type = "TestError";
        var title = "測試錯誤";
        var status = 400;
        var detail = "這是詳細訊息";
        var traceId = "test-trace-id";

        // Act
        var result = ErrorResponse.Create(type, title, status, detail, traceId);

        // Assert
        ClassicAssert.AreEqual(type, result.Type);
        ClassicAssert.AreEqual(title, result.Title);
        ClassicAssert.AreEqual(status, result.Status);
        ClassicAssert.AreEqual(detail, result.Detail);
        ClassicAssert.AreEqual(traceId, result.TraceId);
        ClassicAssert.IsNotNull(result.Timestamp);
    }

    [Test]
    public void InternalServerError_ReturnsCorrectStatusAndType()
    {
        // Act
        var result = ErrorResponse.InternalServerError("trace-123");

        // Assert
        ClassicAssert.AreEqual("InternalError", result.Type);
        ClassicAssert.AreEqual("內部伺服器錯誤", result.Title);
        ClassicAssert.AreEqual(500, result.Status);
        ClassicAssert.AreEqual("trace-123", result.TraceId);
        ClassicAssert.IsNotNull(result.Detail);
    }

    [Test]
    public void ValidationError_ReturnsCorrectStatusAndType()
    {
        // Arrange
        var detail = "驗證失敗";

        // Act
        var result = ErrorResponse.ValidationError(detail, "trace-456");

        // Assert
        ClassicAssert.AreEqual("ValidationError", result.Type);
        ClassicAssert.AreEqual("驗證錯誤", result.Title);
        ClassicAssert.AreEqual(400, result.Status);
        ClassicAssert.AreEqual(detail, result.Detail);
        ClassicAssert.AreEqual("trace-456", result.TraceId);
    }

    [Test]
    public void NotFound_ReturnsCorrectStatusAndType()
    {
        // Arrange
        var detail = "找不到指定的資源";

        // Act
        var result = ErrorResponse.NotFound(detail, "trace-789");

        // Assert
        ClassicAssert.AreEqual("NotFound", result.Type);
        ClassicAssert.AreEqual("找不到資源", result.Title);
        ClassicAssert.AreEqual(404, result.Status);
        ClassicAssert.AreEqual(detail, result.Detail);
        ClassicAssert.AreEqual("trace-789", result.TraceId);
    }

    [Test]
    public void Unauthorized_ReturnsCorrectStatusAndType()
    {
        // Act
        var result = ErrorResponse.Unauthorized("trace-abc");

        // Assert
        ClassicAssert.AreEqual("Unauthorized", result.Type);
        ClassicAssert.AreEqual("未授權", result.Title);
        ClassicAssert.AreEqual(401, result.Status);
        ClassicAssert.AreEqual("trace-abc", result.TraceId);
        ClassicAssert.IsNotNull(result.Detail);
    }

    [Test]
    public void Create_WithoutOptionalParameters_SetsDefaultValues()
    {
        // Act
        var result = ErrorResponse.Create("TestError", "測試", 400);

        // Assert
        ClassicAssert.IsNull(result.Detail);
        ClassicAssert.IsNull(result.TraceId);
        ClassicAssert.IsNull(result.Extensions);
        ClassicAssert.IsNotNull(result.Timestamp);
    }

    [Test]
    public void Timestamp_IsSetToCurrentTime()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow;

        // Act
        var result = ErrorResponse.InternalServerError();

        // Assert
        var afterCreation = DateTime.UtcNow;
        ClassicAssert.IsTrue(result.Timestamp >= beforeCreation);
        ClassicAssert.IsTrue(result.Timestamp <= afterCreation);
    }

    [Test]
    public void Extensions_CanBeSetAndRetrieved()
    {
        // Arrange
        var errorResponse = ErrorResponse.InternalServerError();
        var extensions = new Dictionary<string, object>
        {
            ["customKey"] = "customValue",
            ["errorCode"] = 12345
        };

        // Act
        errorResponse.Extensions = extensions;

        // Assert
        ClassicAssert.IsNotNull(errorResponse.Extensions);
        ClassicAssert.AreEqual(2, errorResponse.Extensions.Count);
        ClassicAssert.AreEqual("customValue", errorResponse.Extensions["customKey"]);
        ClassicAssert.AreEqual(12345, errorResponse.Extensions["errorCode"]);
    }
}
