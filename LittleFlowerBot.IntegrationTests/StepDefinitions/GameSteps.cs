using System.Net;
using System.Text;
using FluentAssertions;
using LittleFlowerBot.DbContexts;
using LittleFlowerBot.IntegrationTests.Infrastructure;
using LittleFlowerBot.Models.Game;
using LittleFlowerBot.Models.GameResult;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using TechTalk.SpecFlow;

namespace LittleFlowerBot.IntegrationTests.StepDefinitions;

[Binding]
public class GameSteps
{
    private readonly HttpClient _httpClient;
    private readonly IntegrationTestWebApplicationFactory _factory;
    private readonly ScenarioContext _scenarioContext;
    private HttpResponseMessage? _lastResponse;

    public GameSteps(
        HttpClient httpClient,
        IntegrationTestWebApplicationFactory factory,
        ScenarioContext scenarioContext)
    {
        _httpClient = httpClient;
        _factory = factory;
        _scenarioContext = scenarioContext;
    }

    [When(@"使用者 ""(.*)"" 在群組 ""(.*)"" 發送訊息 ""(.*)""")]
    public async Task When使用者在群組發送訊息(string userId, string groupId, string text)
    {
        var webhookJson = BuildWebhookJson(userId, groupId, text);
        var content = new StringContent(webhookJson, Encoding.UTF8, "application/json");
        _lastResponse = await _httpClient.PostAsync("/api/LineChat/Callback", content);

        // 將 response 存入 ScenarioContext 供共用步驟使用
        _scenarioContext["LastHttpResponse"] = _lastResponse;
    }

    [Then(@"系統應該回覆包含 ""(.*)"" 的訊息")]
    public void Then系統應該回覆包含的訊息(string expectedText)
    {
        var messages = TestTextRenderer.Messages;
        messages.Should().Contain(msg => msg.Contains(expectedText),
            $"預期訊息中應包含 \"{expectedText}\"，但實際訊息為：[{string.Join(", ", messages)}]");
    }

    [Then(@"資料庫中應該有 (\d+) 筆井字遊戲結果")]
    public void Then資料庫中應該有N筆井字遊戲結果(int expectedCount)
    {
        var context = _factory.ServiceProvider.GetRequiredService<MongoDbContext>();
        var results = context.BoardGameResults
            .Find(r => r.GameType == GameType.TicTacToe)
            .ToList();

        results.Should().HaveCount(expectedCount,
            $"預期有 {expectedCount} 筆井字遊戲結果");
    }

    [Then(@"資料庫中應該有 (\d+) 筆平局結果")]
    public void Then資料庫中應該有N筆平局結果(int expectedCount)
    {
        var context = _factory.ServiceProvider.GetRequiredService<MongoDbContext>();
        var results = context.BoardGameResults
            .Find(r => r.GameType == GameType.TicTacToe && r.Result == Models.GameResult.GameResult.Draw)
            .ToList();

        results.Should().HaveCount(expectedCount,
            $"預期有 {expectedCount} 筆平局結果");
    }

    [Then(@"可以持續猜數字直到猜對為止")]
    public async Task Then可以持續猜數字直到猜對為止()
    {
        const int maxAttempts = 100;
        var low = 0;
        var high = 100;

        for (var i = 0; i < maxAttempts; i++)
        {
            if (TestTextRenderer.Messages.Any(msg => msg.Contains("猜對了")))
            {
                return;
            }

            var rangeMessage = TestTextRenderer.Messages
                .LastOrDefault(msg => msg.StartsWith("猜") && msg.Contains(" - "));

            if (rangeMessage != null)
            {
                ParseRange(rangeMessage, out low, out high);
            }

            var guess = (low + high) / 2;

            if (guess <= low) guess = low + 1;
            if (guess >= high) guess = high - 1;

            if (high - low <= 2)
            {
                guess = low + 1;
            }

            var webhookJson = BuildWebhookJson("userA", "group2", guess.ToString());
            var content = new StringContent(webhookJson, Encoding.UTF8, "application/json");
            _lastResponse = await _httpClient.PostAsync("/api/LineChat/Callback", content);
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        TestTextRenderer.Messages.Should().Contain(msg => msg.Contains("猜對了"),
            $"在 {maxAttempts} 次嘗試後應該已經猜對，最後的訊息：[{string.Join(", ", TestTextRenderer.Messages)}]");
    }

    private static void ParseRange(string rangeMessage, out int low, out int high)
    {
        var parts = rangeMessage.Replace("猜", "").Split(" - ");
        if (parts.Length == 2 &&
            int.TryParse(parts[0].Trim(), out var parsedLow) &&
            int.TryParse(parts[1].Trim(), out var parsedHigh))
        {
            low = parsedLow;
            high = parsedHigh;
        }
        else
        {
            low = 0;
            high = 100;
        }
    }

    private static string BuildWebhookJson(string userId, string groupId, string text)
    {
        return $$"""
        {
            "events": [{
                "type": "message",
                "replyToken": "test-reply-token",
                "source": {
                    "type": "group",
                    "groupId": "{{groupId}}",
                    "userId": "{{userId}}"
                },
                "message": {
                    "type": "text",
                    "id": "1",
                    "text": "{{text}}"
                },
                "timestamp": 1462629479859
            }]
        }
        """;
    }
}
