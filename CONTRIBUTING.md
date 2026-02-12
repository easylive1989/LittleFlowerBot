# 貢獻指南

感謝你對 LittleFlowerBot 的關注！本文件將幫助你了解如何為專案做出貢獻。

## 目錄

- [開發環境設定](#開發環境設定)
- [專案架構](#專案架構)
- [編碼規範](#編碼規範)
- [提交規範](#提交規範)
- [測試規範](#測試規範)
- [Pull Request 流程](#pull-request-流程)

---

## 開發環境設定

### 必要工具

1. **.NET 10.0 SDK**
   ```bash
   # 檢查版本
   dotnet --version
   ```

2. **PostgreSQL** (版本 13+)
   ```bash
   # macOS
   brew install postgresql

   # Ubuntu
   sudo apt-get install postgresql
   ```

3. **Redis** (選用)
   ```bash
   # macOS
   brew install redis

   # Ubuntu
   sudo apt-get install redis-server
   ```

4. **IDE 建議**
   - Visual Studio 2022
   - Visual Studio Code + C# Extension
   - JetBrains Rider

### 初始化專案

```bash
# 1. Fork 並克隆專案
git clone https://github.com/YOUR_USERNAME/LittleFlowerBot.git
cd LittleFlowerBot

# 2. 設定上游倉庫
git remote add upstream https://github.com/easylive1989/LittleFlowerBot.git

# 3. 還原套件
dotnet restore

# 4. 建立資料庫
createdb littleflowerbot

# 5. 執行資料庫遷移
dotnet ef database update --project LittleFlowerBot

# 6. 設定 User Secrets
dotnet user-secrets init --project LittleFlowerBot
dotnet user-secrets set "LINE_CHANNEL_TOKEN" "your_token" --project LittleFlowerBot

# 7. 執行測試
dotnet test

# 8. 啟動應用程式
dotnet run --project LittleFlowerBot
```

---

## 專案架構

本專案採用**分層架構**設計。在開發新功能前，請先閱讀 [ARCHITECTURE.md](./ARCHITECTURE.md)。

### 核心原則

1. **領域層獨立** - 領域邏輯不應依賴外部框架
2. **依賴注入** - 使用建構子注入，避免 Service Locator
3. **接口導向** - 定義清晰的接口，便於測試
4. **單一職責** - 每個類別只負責一件事

### 加入新功能的步驟

#### 範例：加入新遊戲

1. **定義領域模型** (`Models/Game/`)
   ```csharp
   public class MyNewGame : Game
   {
       // 遊戲邏輯
   }
   ```

2. **實作遊戲盤面** (`Models/Game/MyNewGame/`)
   ```csharp
   public class MyNewGameBoard : GameBoard
   {
       // 盤面邏輯
   }
   ```

3. **更新遊戲工廠** (`Models/Game/GameFactory.cs`)
   ```csharp
   public Game CreateGame(GameType gameType)
   {
       return gameType switch
       {
           GameType.MyNewGame => _serviceProvider.GetService<MyNewGame>(),
           // ...
       };
   }
   ```

4. **註冊服務** (`Program.cs`)
   ```csharp
   builder.Services.AddScoped<MyNewGame>();
   ```

5. **撰寫測試** (`LittleFlowerBotTests/Models/Game/`)
   ```csharp
   [TestFixture]
   public class MyNewGameTests
   {
       // 測試案例
   }
   ```

---

## 編碼規範

### C# 編碼風格

遵循 [C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)

#### 命名規範

```csharp
// ✅ 好的範例
public class GameFactory { }              // PascalCase for classes
public interface IGameFactory { }         // I prefix for interfaces
public void CreateGame() { }              // PascalCase for methods
private readonly ILogger _logger;         // _camelCase for private fields
public string UserName { get; set; }      // PascalCase for properties

// ❌ 不好的範例
public class gameFactory { }              // 類別名稱應為 PascalCase
public interface GameFactory { }          // 接口應有 I 前綴
private ILogger logger;                   // 私有欄位應有 _ 前綴
public string userName { get; set; }      // 屬性應為 PascalCase
```

#### 程式碼組織

```csharp
// ✅ 檔案組織順序
namespace LittleFlowerBot.Models.Game
{
    // 1. Using statements
    using System;
    using System.Collections.Generic;

    // 2. Class declaration
    public class Game
    {
        // 3. Private fields
        private readonly ILogger _logger;

        // 4. Constructors
        public Game(ILogger logger)
        {
            _logger = logger;
        }

        // 5. Public properties
        public string GameId { get; set; }

        // 6. Public methods
        public void StartGame() { }

        // 7. Private methods
        private void ValidateGame() { }
    }
}
```

### 註解規範

```csharp
/// <summary>
/// 創建新遊戲
/// </summary>
/// <param name="gameType">遊戲類型</param>
/// <returns>遊戲實例</returns>
/// <exception cref="ArgumentException">當遊戲類型不支援時拋出</exception>
public Game CreateGame(GameType gameType)
{
    // TODO: 實作其他遊戲類型
    // FIXME: 修正記憶體洩漏問題
    // NOTE: 這裡使用工廠模式
}
```

### Nullable Reference Types

專案已啟用 Nullable Reference Types，請確保：

```csharp
// ✅ 明確標註可為 null
public string? OptionalName { get; set; }

// ✅ 不可為 null 的參數驗證
public void ProcessGame(Game game)
{
    ArgumentNullException.ThrowIfNull(game);
    // ...
}

// ❌ 避免使用 ! 運算子（除非你確定不會為 null）
var name = user!.Name;  // 不好
```

---

## 提交規範

### Commit Message 格式

使用 [Conventional Commits](https://www.conventionalcommits.org/) 規範：

```
<type>(<scope>): <subject>

<body>

<footer>
```

#### Type 類型

- `feat`: 新功能
- `fix`: 錯誤修復
- `docs`: 文件更新
- `style`: 程式碼格式（不影響功能）
- `refactor`: 重構（不是新功能也不是錯誤修復）
- `test`: 測試相關
- `chore`: 建置流程或輔助工具

#### 範例

```bash
# 新功能
feat(game): 加入四子棋遊戲

實作四子棋的遊戲邏輯，包括：
- 遊戲盤面
- 勝利判定
- 遊戲規則

Closes #123

# 錯誤修復
fix(cache): 修正 Redis 連線逾時問題

當 Redis 不可用時，現在會自動切換到記憶體快取

Fixes #456

# 重構
refactor(services): 將 Redis 配置抽取為獨立服務

提高可測試性和可維護性
```

---

## 測試規範

### 測試結構

```csharp
[TestFixture]
public class GameFactoryTests
{
    private IServiceProvider _serviceProvider;
    private GameFactory _factory;

    [SetUp]
    public void Setup()
    {
        // Arrange - 設定測試環境
        _serviceProvider = Substitute.For<IServiceProvider>();
        _factory = new GameFactory(_serviceProvider);
    }

    [Test]
    public void CreateGame_WithValidType_ReturnsCorrectGame()
    {
        // Arrange
        var gameType = GameType.TicTacToe;

        // Act
        var result = _factory.CreateGame(gameType);

        // Assert
        ClassicAssert.IsInstanceOf<TicTacToeGame>(result);
    }

    [Test]
    public void CreateGame_WithInvalidType_ThrowsException()
    {
        // Arrange
        var gameType = (GameType)999;

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            _factory.CreateGame(gameType));
    }
}
```

### 測試命名

```
MethodName_Scenario_ExpectedBehavior
```

範例：
- `CreateGame_WithValidType_ReturnsCorrectGame`
- `CreateGame_WithNullParameter_ThrowsArgumentNullException`
- `Move_WhenNotYourTurn_ThrowsNotYourTurnException`

### 測試覆蓋率目標

- **領域邏輯**: 80%+
- **服務層**: 70%+
- **工具類別**: 90%+

---

## Pull Request 流程

### 1. 創建 Feature Branch

```bash
# 從最新的 master 創建分支
git checkout master
git pull upstream master
git checkout -b feat/add-new-game
```

### 2. 開發與測試

```bash
# 定期提交
git add .
git commit -m "feat(game): add new game logic"

# 執行測試
dotnet test

# 確保編譯無誤
dotnet build
```

### 3. 同步最新代碼

```bash
# 拉取上游最新代碼
git fetch upstream
git rebase upstream/master

# 解決衝突（如果有）
# ...
git rebase --continue
```

### 4. 推送並創建 PR

```bash
# 推送到你的 Fork
git push origin feat/add-new-game
```

然後在 GitHub 上創建 Pull Request。

### 5. PR 檢查清單

在創建 PR 前，請確認：

- [ ] 所有測試通過 (`dotnet test`)
- [ ] 程式碼可以成功編譯 (`dotnet build`)
- [ ] 沒有引入新的警告
- [ ] 已更新相關文件
- [ ] 遵循編碼規範
- [ ] Commit messages 符合規範
- [ ] 已處理所有 TODO 和 FIXME
- [ ] PR 描述清楚說明改動內容

### 6. Code Review

- 積極回應 reviewer 的意見
- 及時修改建議的改進
- 保持禮貌和專業

---

## 常見問題

### Q: 如何執行單一測試？

```bash
dotnet test --filter "FullyQualifiedName~GameFactoryTests"
```

### Q: 如何產生測試覆蓋率報告？

```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Q: 如何更新資料庫 Schema？

```bash
# 加入新的 Migration
dotnet ef migrations add YourMigrationName --project LittleFlowerBot

# 更新資料庫
dotnet ef database update --project LittleFlowerBot
```

### Q: 如何除錯測試？

在 Visual Studio 或 VS Code 中，在測試方法上按右鍵選擇「Debug Test」。

---

## 需要幫助？

- 📖 閱讀 [ARCHITECTURE.md](./ARCHITECTURE.md) 了解架構設計
- 💬 在 Issues 中提問
- 📧 聯繫維護者

---

## 行為準則

請遵守友善、尊重、專業的原則。我們歡迎所有形式的貢獻！

感謝你的貢獻！ 🎉
