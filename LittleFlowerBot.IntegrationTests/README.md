# LittleFlowerBot 整合測試

## 概述

本專案使用 **SpecFlow** 進行 BDD（行為驅動開發）風格的整合測試，並使用 **Testcontainers** 自動管理 Docker 容器。

## 技術棧

- **SpecFlow 3.9** - BDD 測試框架
- **NUnit 4.3** - 測試執行器
- **Testcontainers** - Docker 容器管理
- **FluentAssertions** - 流暢的斷言語法
- **WebApplicationFactory** - ASP.NET Core 整合測試

## 前置需求

### 必須安裝

1. **.NET 10.0 SDK**
   ```bash
   dotnet --version  # 應該顯示 10.x.x
   ```

2. **Docker Desktop**
   ```bash
   docker --version  # 確認 Docker 已安裝並運行
   docker ps        # 確認可以連接到 Docker daemon
   ```

### 驗證環境

```bash
# 確認 Docker 正常運作
docker run hello-world

# 確認可以拉取測試需要的映像
docker pull postgres:15-alpine
docker pull redis:7-alpine
```

## 執行測試

### 方式 1: 使用命令列

```bash
# 切換到測試專案目錄
cd LittleFlowerBot.IntegrationTests

# 執行所有整合測試
dotnet test

# 執行特定的 Feature
dotnet test --filter "FullyQualifiedName~HealthCheck"

# 顯示詳細輸出
dotnet test --verbosity detailed
```

### 方式 2: 使用 IDE

#### Visual Studio 2022
1. 開啟測試總管（Test Explorer）
2. 點擊「執行全部」
3. 查看測試結果

#### Visual Studio Code
1. 安裝 .NET Test Explorer 擴充套件
2. 在側邊欄選擇測試總管
3. 點擊測試旁的播放按鈕

#### Rider
1. 開啟測試總管（Unit Tests）
2. 右鍵點擊專案 → Run All Tests
3. 查看測試結果

## 測試架構

### 目錄結構

```
LittleFlowerBot.IntegrationTests/
├── Features/              # SpecFlow Feature 檔案
│   └── HealthCheck.feature
├── StepDefinitions/       # Step Definition 實作
│   └── HealthCheckSteps.cs
├── Infrastructure/        # 測試基礎設施
│   └── IntegrationTestWebApplicationFactory.cs
├── Hooks/                 # SpecFlow Hooks
│   └── TestHooks.cs
└── README.md
```

### 測試流程

```
1. [BeforeTestRun]
   ↓
   啟動 PostgreSQL Docker 容器
   ↓
   啟動 Redis Docker 容器
   ↓
   啟動測試 Web 伺服器
   ↓
2. [BeforeScenario]
   ↓
   注入依賴到測試情境
   ↓
3. 執行測試步驟
   ↓
4. [AfterScenario]
   ↓
5. [AfterTestRun]
   ↓
   停止並清理所有 Docker 容器
```

## 撰寫新的測試

### 步驟 1: 創建 Feature 檔案

在 `Features/` 目錄下創建新的 `.feature` 檔案：

```gherkin
Feature: 遊戲管理 API
    作為玩家
    我想要創建和加入遊戲
    以便與其他玩家一起玩

Scenario: 創建新遊戲
    Given 我是已註冊的玩家
    When 我發送 POST 請求到 "/api/games" 與遊戲類型 "TicTacToe"
    Then 回應狀態碼應該是 201
    And 回應應該包含遊戲 ID
```

### 步驟 2: 實作 Step Definitions

在 `StepDefinitions/` 目錄下創建對應的 C# 檔案：

```csharp
[Binding]
public class GameManagementSteps
{
    private readonly HttpClient _httpClient;
    private HttpResponseMessage? _response;

    public GameManagementSteps(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    [Given(@"我是已註冊的玩家")]
    public void Given我是已註冊的玩家()
    {
        // 設定測試資料
    }

    [When(@"我發送 POST 請求到 ""(.*)"" 與遊戲類型 ""(.*)""")]
    public async Task When我發送POST請求(string endpoint, string gameType)
    {
        var content = new StringContent(
            JsonSerializer.Serialize(new { gameType }),
            Encoding.UTF8,
            "application/json");

        _response = await _httpClient.PostAsync(endpoint, content);
    }

    // ... 其他步驟定義
}
```

## Docker 容器管理

### Testcontainers 自動管理

測試執行時，Testcontainers 會自動：

1. **拉取映像**（如果本地不存在）
   ```
   postgres:15-alpine
   redis:7-alpine
   ```

2. **啟動容器**
   - PostgreSQL: 隨機端口
   - Redis: 隨機端口

3. **執行資料庫遷移**
   - 自動套用 EF Core Migrations

4. **測試結束後清理**
   - 停止容器
   - 刪除容器
   - 清理資源

### 手動查看容器

測試執行中，可以查看運行的容器：

```bash
# 列出所有容器
docker ps

# 查看 PostgreSQL 日誌
docker logs <container-id>

# 連接到 PostgreSQL
docker exec -it <container-id> psql -U test_user -d littleflowerbot_test
```

## 常見問題

### Q: 測試失敗：無法連接到 Docker

**錯誤訊息**:
```
Cannot connect to Docker daemon
```

**解決方案**:
1. 確認 Docker Desktop 已啟動
2. 檢查 Docker daemon 是否運行：`docker ps`
3. 重啟 Docker Desktop

---

### Q: 測試很慢

**原因**: 首次執行需要下載 Docker 映像

**解決方案**:
```bash
# 預先拉取映像
docker pull postgres:15-alpine
docker pull redis:7-alpine
```

---

### Q: 端口衝突

**錯誤訊息**:
```
Port 5432 is already in use
```

**解決方案**:
Testcontainers 自動使用隨機端口，不會衝突。
如果遇到此錯誤，檢查是否有其他測試實例正在運行。

---

### Q: 如何除錯測試？

**方法 1**: 使用 IDE 除錯器
- 在 Step Definition 中設定中斷點
- 使用「Debug Test」執行

**方法 2**: 輸出日誌
```csharp
[When(@"我發送 GET 請求到 ""(.*)""")]
public async Task When我發送GET請求到(string endpoint)
{
    Console.WriteLine($"Sending request to: {endpoint}");
    _response = await _httpClient.GetAsync(endpoint);
    Console.WriteLine($"Response: {await _response.Content.ReadAsStringAsync()}");
}
```

---

### Q: 如何跳過整合測試？

```bash
# 只執行單元測試（排除整合測試專案）
dotnet test --filter "FullyQualifiedName!~IntegrationTests"
```

## 效能優化

### 1. 容器重用

可以配置 Testcontainers 重用容器：

```csharp
_postgresContainer = new PostgreSqlBuilder()
    .WithImage("postgres:15-alpine")
    .WithReuse(true)  // 啟用容器重用
    .Build();
```

### 2. 平行執行

```bash
# 平行執行測試（需要注意資料庫隔離）
dotnet test --parallel
```

## CI/CD 整合

### GitHub Actions 範例

```yaml
name: Integration Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v3

    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '10.0.x'

    - name: Restore dependencies
      run: dotnet restore

    - name: Run integration tests
      run: dotnet test LittleFlowerBot.IntegrationTests --verbosity normal
```

## 最佳實踐

### ✅ 應該做的事

1. **每個 Feature 專注於單一功能**
2. **使用有意義的情境名稱**
3. **保持 Step Definitions 簡潔**
4. **測試結束後清理資料**
5. **使用 FluentAssertions 提高可讀性**

### ❌ 不應該做的事

1. **不要在測試中使用生產資料庫**
2. **不要假設測試執行順序**
3. **不要在 Step Definitions 中寫業務邏輯**
4. **不要忽略容器清理**

## 參考資源

- [SpecFlow 文件](https://docs.specflow.org/)
- [Testcontainers 文件](https://dotnet.testcontainers.org/)
- [WebApplicationFactory 文件](https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests)
- [FluentAssertions 文件](https://fluentassertions.com/)

## 支援

遇到問題？
1. 查看本 README 的「常見問題」部分
2. 檢查 Docker 和 .NET 環境是否正確設定
3. 在專案 Issues 中提問
