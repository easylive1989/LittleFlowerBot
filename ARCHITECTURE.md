# LittleFlowerBot 架構文件

## 架構概覽

本專案採用分層架構設計，各層職責清晰分離，便於維護和測試。

## 分層結構

### 📊 展示層（Presentation Layer）

**位置**: `/Controllers`, `/Middlewares`, `/HealthChecks`

**職責**:
- 處理 HTTP 請求和回應
- 全域錯誤處理
- 健康檢查端點
- API 路由定義

**主要元件**:
- `Controllers/` - Web API 控制器
  - `LineChatController` - Line Bot Webhook 入口
  - `BoardImageController` - 棋盤圖片端點
  - `MgmtController` - 管理功能
- `Middlewares/` - HTTP 中介軟體
  - `GlobalExceptionHandlerMiddleware` - 全域例外處理
- `HealthChecks/` - 健康檢查實作
  - `ApplicationHealthCheck` - 應用程式狀態檢查
  - `MemoryHealthCheck` - 記憶體監控
  - `MongoDbHealthCheck` - MongoDB 連線檢查

---

### 🎯 應用層（Application Layer）

**位置**: `/Services`

**職責**:
- 業務流程編排
- 應用服務實作
- 事件處理邏輯

**主要元件**:
- `Services/EventHandler/` - Line Bot 事件處理器
  - `GameHandler` - 遊戲指令處理（開新局、下棋、認輸等）
  - `RecordHandler` - 戰績查詢處理

---

### 🏗️ 領域層（Domain Layer）

**位置**: `/Models`

**職責**:
- 核心業務邏輯
- 領域模型定義
- 遊戲規則實作

**主要元件**:

#### 領域模型
- `Models/Game/` - 遊戲領域模型
  - `Game` - 遊戲基底類別
  - `GameFactory` - 遊戲工廠
  - `BoardGame/` - 棋盤遊戲基底
    - `ChessGames/ChineseChess/` - 象棋實作
    - `KiGames/Gomoku/` - 五子棋實作
    - `KiGames/TicTacToe/` - 井字遊戲實作
  - `GuessNumber/` - 猜數字遊戲
  - `Battleship/` - 海戰棋遊戲
- `Models/GameResult/` - 遊戲結果領域模型
  - `BoardGameResult` - 戰績紀錄

#### 領域例外
- `Models/GameExceptions/` - 遊戲相關例外
  - `NotYourTurnException` - 不是你的回合
  - `PlayerExistException` - 玩家已存在
  - `NotYourChessException` - 不是你的棋子
  - `MoveInvalidException` - 移動無效
  - `CoordinateValidException` - 座標無效

#### 領域服務接口
- `Models/Game/IGameFactory` - 遊戲工廠接口
- `Models/Renderer/ITextRenderer` - 文字渲染器接口
- `Models/Renderer/IRendererFactory` - 渲染器工廠接口
- `Models/Message/IMessage` - 訊息服務接口
- `Models/Message/ILineUserService` - Line 使用者查詢接口（好友檢查）

---

### 🔧 基礎設施層（Infrastructure Layer）

**位置**: `/DbContexts`, `/Repositories`, `/Models/Caches`, `/Models/Renderer`, `/Models/Message`

**職責**:
- 資料持久化
- 外部服務整合
- 快取實作
- 第三方 API 呼叫

**主要元件**:

#### 資料存取
- `DbContexts/MongoDbContext` - MongoDB 上下文（提供 collection 存取）
- `Repositories/` - Repository 實作
  - `BoardGameResultsRepository` - 遊戲戰績資料存取

#### 快取服務
- `Models/Caches/` - 遊戲盤面狀態
  - `GameBoardCache` - 遊戲盤面快取（以 MongoDB 為存放媒介）
  - `GameStateDocument` - 盤面序列化 Document

#### 外部服務整合
- `Models/Renderer/` - 渲染服務實作
  - `BufferedReplyRenderer` - 緩衝後一次性 reply 給 Line（生產環境）
  - `ConsoleRenderer` - 控制台輸出（開發環境）
- `Models/Message/` - Line API 客戶端
  - `LineMessage` - Line Push / Reply 訊息發送
  - `LineUserService` - 查詢使用者是否為 Bot 好友
- `Models/Game/Battleship/BattleshipBoardImageRenderer` - 海戰棋盤面圖片渲染（SkiaSharp）

---

### 📦 共用層（Shared/Common）

**位置**: `/Models/HealthCheck`, `/Models/Message`, `/Utils`, `/Extensions`

**職責**:
- 資料傳輸對象（DTOs）
- 共用工具類別
- 跨層共用的模型

**主要元件**:
- `Models/HealthCheck/HealthCheckResponse` - 健康檢查回應 DTO
- `Models/Message/QuickReplyItem` / `ReplyMessageItem` - Line 訊息結構
- `Extensions/EventExtensions` - Line Event 擴充方法（取得 senderId / userId / text）
- `Utils/DictionaryJsonConverter` - JSON 轉換器

---

## 依賴規則

```
展示層 (Presentation)
    ↓ 依賴
應用層 (Application)
    ↓ 依賴
領域層 (Domain)
    ↑ 被實作
基礎設施層 (Infrastructure)
```

**核心原則**:
1. **領域層** 不依賴任何其他層（純粹的業務邏輯）
2. **應用層** 只依賴領域層
3. **基礎設施層** 實作領域層定義的接口
4. **展示層** 協調應用層和基礎設施層

---

## 資料流程

### 典型的請求處理流程

```
1. HTTP Request (Line Webhook)
   ↓
2. LineChatController (Presentation)
   ↓
3. GameHandler / RecordHandler (Application)
   ↓
4. Game.Act() (Domain)
   ↓
5. GameBoardCache / Repository (Infrastructure)
   ↓
6. MongoDB
```

### 範例：玩家下棋

```
LineChatController
   ↓ 接收 Line Webhook
GameHandler (EventHandler)
   ↓ 解析命令、判斷新局 / 既有遊戲
Game.Act() (Domain)
   ↓ 驗證輸入格式、套用規則
GameBoard.Move() (Domain)
   ↓ 更新盤面
GameBoardCache.Set() (Infrastructure)
   ↓ 儲存盤面狀態
MongoDB
```

---

## 技術棧

### 框架與函式庫
- **.NET 10.0** - 應用程式框架
- **ASP.NET Core** - Web API 框架
- **MongoDB.Driver 3.4** - MongoDB 官方驅動
- **LineBotSDK 2.0** - Line Bot 客戶端
- **SkiaSharp 3.116** - 棋盤圖片渲染
- **NUnit 4.2** + **NSubstitute 5.3** - 測試

### 資料儲存
- **MongoDB** - 戰績與遊戲盤面狀態

### 外部服務
- **Line Messaging API** - Line Bot 整合

---

## 設計模式

本專案採用了以下設計模式：

### 1. **Factory Pattern（工廠模式）**
- `GameFactory` - 創建不同類型的遊戲
- `RendererFactory` - 創建不同類型的渲染器

### 2. **Repository Pattern（倉儲模式）**
- `BoardGameResultsRepository` - 封裝戰績資料存取邏輯

### 3. **Strategy Pattern（策略模式）**
- `ITextRenderer` - 不同的文字輸出策略（Console / Line Reply）
- `IMessage` - 不同的訊息發送策略

### 4. **Chain of Responsibility（責任鏈模式）**
- `ILineEventHandler` - 事件處理器鏈（GameHandler、RecordHandler 依序處理）

### 5. **Dependency Injection（依賴注入）**
- 全專案使用 DI 容器管理依賴關係

---

## 測試策略

### 單元測試
- 位置：`/LittleFlowerBotTests`
- 測試覆蓋：領域邏輯、服務、工具類別
- 框架：NUnit + NSubstitute

### 整合測試
- 位置：`/LittleFlowerBot.IntegrationTests`

### 測試組織
```
LittleFlowerBotTests/
├── Models/
│   ├── Game/ - 遊戲邏輯測試
│   └── Caches/ - 快取測試
├── Services/ - 服務測試
├── HealthChecks/ - 健康檢查測試
└── Utils/ - 工具類別測試
```

---

## 部署架構

```
Internet
    ↓
Render (Docker container)
    ↓
LittleFlowerBot (Web App)
    ↓
MongoDB Atlas (Database)
```

部署設定見 `render.yaml` 與 `Dockerfile`。

---

## 未來改進方向

1. **更嚴格的分層**
   - 考慮將領域層抽取為獨立專案
   - 實作 CQRS 模式

2. **測試覆蓋率**
   - 增加整合測試
   - 加入端到端測試

3. **監控與日誌**
   - 整合 Application Insights
   - 結構化日誌

4. **效能優化**
   - 實作快取策略
   - 資料庫查詢優化

---

## 參考資源

- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [ASP.NET Core Architecture](https://docs.microsoft.com/en-us/dotnet/architecture/)
- [Domain-Driven Design](https://martinfowler.com/tags/domain%20driven%20design.html)
