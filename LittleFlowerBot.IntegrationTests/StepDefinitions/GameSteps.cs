using System.Net;
using System.Text;
using FluentAssertions;
using LittleFlowerBot.DbContexts;
using LittleFlowerBot.IntegrationTests.Infrastructure;
using LittleFlowerBot.Models.Game;
using LittleFlowerBot.Models.Game.GuessNumber;
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

    [When(@"使用者 ""(.*)"" 不是 Bot 好友")]
    public void When使用者不是Bot好友(string userId)
    {
        var fakeService = _factory.ServiceProvider.GetRequiredService<FakeLineUserService>();
        fakeService.SetNotFollower(userId);
    }

    [Given(@"猜數字的目標數字為 (\d+)")]
    public void Given猜數字的目標數字為(int target)
    {
        var fakeRandom = _factory.ServiceProvider.GetRequiredService<FakeRandomGenerator>();
        fakeRandom.NextValue = target;
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

    [Then(@"系統應該有私訊給 ""(.*)"" 包含 ""(.*)"" 的訊息")]
    public void Then系統應該有私訊給包含的訊息(string userId, string expectedText)
    {
        var messages = TestTextRenderer.Messages;
        var prefix = $"[私訊 {userId}]";
        messages.Should().Contain(msg => msg.StartsWith(prefix) && msg.Contains(expectedText),
            $"預期有私訊給 \"{userId}\" 包含 \"{expectedText}\"，但實際訊息為：[{string.Join(", ", messages)}]");
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

    [Then(@"資料庫中應該有 (\d+) 筆五子棋遊戲結果")]
    public void Then資料庫中應該有N筆五子棋遊戲結果(int expectedCount)
    {
        var context = _factory.ServiceProvider.GetRequiredService<MongoDbContext>();
        var results = context.BoardGameResults
            .Find(r => r.GameType == GameType.Gomoku)
            .ToList();

        results.Should().HaveCount(expectedCount,
            $"預期有 {expectedCount} 筆五子棋遊戲結果");
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
