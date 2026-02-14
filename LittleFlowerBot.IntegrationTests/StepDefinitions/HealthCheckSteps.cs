using System.Net;
using System.Text.Json;
using FluentAssertions;
using LittleFlowerBot.IntegrationTests.Infrastructure;
using TechTalk.SpecFlow;

namespace LittleFlowerBot.IntegrationTests.StepDefinitions;

[Binding]
public class HealthCheckSteps
{
    private readonly HttpClient _httpClient;
    private readonly IntegrationTestWebApplicationFactory _factory;
    private readonly ScenarioContext _scenarioContext;
    private HttpResponseMessage? _response;
    private JsonDocument? _jsonResponse;

    public HealthCheckSteps(
        HttpClient httpClient,
        IntegrationTestWebApplicationFactory factory,
        ScenarioContext scenarioContext)
    {
        _httpClient = httpClient;
        _factory = factory;
        _scenarioContext = scenarioContext;
    }

    [Given(@"測試環境已準備就緒")]
    public void Given測試環境已準備就緒()
    {
        _factory.MongoConnectionString.Should().NotBeNullOrEmpty();
    }

    [When(@"我發送 GET 請求到 ""(.*)""")]
    public async Task When我發送GET請求到(string endpoint)
    {
        _response = await _httpClient.GetAsync(endpoint);
    }

    [Then(@"回應狀態碼應該是 (.*)")]
    public async Task Then回應狀態碼應該是(int statusCode)
    {
        var response = _response;
        if (response == null && _scenarioContext.TryGetValue("LastHttpResponse", out HttpResponseMessage? contextResponse))
        {
            response = contextResponse;
        }

        response.Should().NotBeNull();

        if (response!.StatusCode != (HttpStatusCode)statusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"實際狀態碼: {(int)response.StatusCode} ({response.StatusCode})");
            Console.WriteLine($"響應內容: {content}");
        }

        response.StatusCode.Should().Be((HttpStatusCode)statusCode);
    }

    [Then(@"回應內容應該是 JSON 格式")]
    public async Task Then回應內容應該是JSON格式()
    {
        _response.Should().NotBeNull();
        var content = await _response!.Content.ReadAsStringAsync();

        var act = () => JsonDocument.Parse(content);
        act.Should().NotThrow();

        _jsonResponse = JsonDocument.Parse(content);
    }

    [Then(@"JSON 回應中應該包含 ""(.*)"" 欄位")]
    public async Task ThenJSON回應中應該包含欄位(string fieldName)
    {
        if (_jsonResponse == null)
        {
            var content = await _response!.Content.ReadAsStringAsync();
            _jsonResponse = JsonDocument.Parse(content);
        }

        _jsonResponse.RootElement.TryGetProperty(fieldName, out _).Should().BeTrue(
            $"JSON 回應應該包含 '{fieldName}' 欄位");
    }

    [Then(@"""(.*)"" 欄位的值應該是 ""(.*)""")]
    public async Task Then欄位的值應該是(string fieldName, string expectedValue)
    {
        if (_jsonResponse == null)
        {
            var content = await _response!.Content.ReadAsStringAsync();
            _jsonResponse = JsonDocument.Parse(content);
        }

        _jsonResponse.RootElement.TryGetProperty(fieldName, out var property).Should().BeTrue();
        property.GetString().Should().Be(expectedValue);
    }

    [Then(@"""(.*)"" 欄位應該包含 ""(.*)"" 檢查項目")]
    public async Task Then欄位應該包含檢查項目(string fieldName, string checkName)
    {
        if (_jsonResponse == null)
        {
            var content = await _response!.Content.ReadAsStringAsync();
            _jsonResponse = JsonDocument.Parse(content);
        }

        _jsonResponse.RootElement.TryGetProperty(fieldName, out var checksProperty).Should().BeTrue();
        checksProperty.TryGetProperty(checkName, out _).Should().BeTrue(
            $"檢查項目應該包含 '{checkName}'");
    }

    [Then(@"MongoDB 健康檢查應該通過")]
    public async Task ThenMongoDB健康檢查應該通過()
    {
        if (_jsonResponse == null)
        {
            var content = await _response!.Content.ReadAsStringAsync();
            _jsonResponse = JsonDocument.Parse(content);
        }

        _jsonResponse.RootElement
            .GetProperty("checks")
            .TryGetProperty("MongoDB", out var mongoCheck)
            .Should().BeTrue();

        mongoCheck.GetProperty("status").GetString().Should().Be("Healthy");
    }

    [Then(@"記憶體健康檢查應該包含使用量資訊")]
    public async Task Then記憶體健康檢查應該包含使用量資訊()
    {
        if (_jsonResponse == null)
        {
            var content = await _response!.Content.ReadAsStringAsync();
            _jsonResponse = JsonDocument.Parse(content);
        }

        _jsonResponse.RootElement
            .GetProperty("checks")
            .TryGetProperty("Memory", out var memoryCheck)
            .Should().BeTrue();

        memoryCheck.TryGetProperty("data", out var data).Should().BeTrue();
        data.TryGetProperty("allocatedMB", out _).Should().BeTrue();
        data.TryGetProperty("gen0Collections", out _).Should().BeTrue();
    }
}
