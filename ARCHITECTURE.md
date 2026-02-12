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
  - `LineChatController` - Line Bot 訊息處理
  - `MgmtController` - 管理功能
  - `SubscriptionsController` - 訂閱管理
- `Middlewares/` - HTTP 中介軟體
  - `GlobalExceptionHandlerMiddleware` - 全域例外處理
- `HealthChecks/` - 健康檢查實作
  - `ApplicationHealthCheck` - 應用程式狀態檢查
  - `MemoryHealthCheck` - 記憶體監控

---

### 🎯 應用層（Application Layer）

**位置**: `/Services`

**職責**:
- 業務流程編排
- 應用服務實作
- 事件處理邏輯

**主要元件**:
- `Services/EventHandler/` - Line Bot 事件處理器
  - `GameHandler` - 遊戲相關事件處理
  - `RecordHandler` - 記錄管理
  - `RegistrationHandler` - 註冊流程處理
- `Services/RedisConfigurationService` - Redis 配置服務

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
  - `BoardGame/` - 棋盤遊戲
    - `ChessGames/ChineseChess/` - 象棋實作
    - `KiGames/Gomoku/` - 五子棋實作
    - `KiGames/TicTacToe/` - 井字遊戲實作
  - `GuessNumber/` - 猜數字遊戲
- `Models/Subscribe/` - 訂閱領域模型
  - `Subscription` - 訂閱實體

#### 領域例外
- `Models/GameExceptions/` - 遊戲相關例外
  - `NotYourTurnException` - 不是你的回合
  - `PlayerExistException` - 玩家已存在
  - `NotYourChessException` - 不是你的棋子
  - `MoveInvalidException` - 移動無效
  - `CoordinateValidException` - 座標無效
- `Models/Exceptions/` - 其他領域例外
  - `LineNotifyTokenInvalidException` - Line Notify Token 無效

#### 領域服務接口
- `Models/Game/IGameFactory` - 遊戲工廠接口
- `Models/Renderer/ITextRenderer` - 文字渲染器接口
- `Models/Renderer/IRendererFactory` - 渲染器工廠接口
- `Models/Message/IMessage` - 訊息服務接口

---

### 🔧 基礎設施層（Infrastructure Layer）

**位置**: `/DbContexts`, `/Repositories`, `/Models/Caches`, `/Models/Renderer`

**職責**:
- 資料持久化
- 外部服務整合
- 快取實作
- 第三方 API 呼叫

**主要元件**:

#### 資料存取
- `DbContexts/LittleFlowerBotContext` - EF Core DbContext
- `Migrations/` - 資料庫遷移檔案
- `Repositories/` - Repository 實作
  - `BoardGameResultsRepository` - 遊戲結果資料存取
  - `SubscriptionRepository` - 訂閱資料存取

#### 快取服務
- `Models/Caches/` - 快取實作
  - `GameBoardCache` - 遊戲盤面快取
  - `RegistrationCache` - 註冊資訊快取

#### 外部服務
- `Models/Renderer/` - 渲染服務實作
  - `LineNotifySender` - Line Notify 發送服務
  - `LineNotifySubscription` - Line Notify 訂閱服務
  - `ConsoleRenderer` - 控制台輸出（開發用）
- `Models/Message/` - 訊息服務實作
  - `LineMessage` - Line Bot 訊息發送

---

### 📦 共用層（Shared/Common）

**位置**: `/Models/Requests`, `/Models/Responses`, `/Models/ViewModels`, `/Utils`

**職責**:
- 資料傳輸對象（DTOs）
- 共用工具類別
- 跨層共用的模型

**主要元件**:
- `Models/Requests/` - 請求 DTOs
  - `LineNotifyRequest` - Line Notify 請求
- `Models/Responses/` - 回應 DTOs
  - `ErrorResponse` - 錯誤回應
  - `LineNotifyTokenResponse` - Token 回應
- `Models/HealthCheck/` - 健康檢查 DTOs
  - `HealthCheckResponse` - 健康檢查回應
- `Models/ViewModels/` - 視圖模型
- `Utils/` - 工具類別
  - `DictionaryJsonConverter` - JSON 轉換器

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
1. HTTP Request
   ↓
2. Controller (Presentation)
   ↓
3. Event Handler (Application)
   ↓
4. Domain Service (Domain)
   ↓
5. Repository (Infrastructure)
   ↓
6. Database
```

### 範例：玩家下棋

```
LineChatController
   ↓ 接收 Line Webhook
GameHandler (EventHandler)
   ↓ 解析命令
Game.Act() (Domain)
   ↓ 驗證規則
GameBoard.Move() (Domain)
   ↓ 更新盤面
GameBoardCache.Set() (Infrastructure)
   ↓ 儲存狀態
Redis
```

---

## 技術棧

### 框架與函式庫
- **.NET 10.0** - 應用程式框架
- **ASP.NET Core** - Web API 框架
- **Entity Framework Core 9.0** - ORM
- **StackExchange.Redis** - Redis 客戶端
- **NUnit 4.2** - 單元測試框架

### 資料庫與快取
- **PostgreSQL** - 主要資料庫
- **Redis** - 分散式快取

### 外部服務
- **Line Messaging API** - Line Bot 整合
- **Line Notify API** - 通知推送

---

## 設計模式

本專案採用了以下設計模式：

### 1. **Factory Pattern（工廠模式）**
- `GameFactory` - 創建不同類型的遊戲
- `RendererFactory` - 創建不同類型的渲染器

### 2. **Repository Pattern（倉儲模式）**
- `BoardGameResultsRepository` - 封裝資料存取邏輯
- `SubscriptionRepository` - 封裝訂閱資料操作

### 3. **Strategy Pattern（策略模式）**
- `ITextRenderer` - 不同的文字輸出策略
- `IMessage` - 不同的訊息發送策略

### 4. **Chain of Responsibility（責任鏈模式）**
- `ILineEventHandler` - 事件處理器鏈

### 5. **Dependency Injection（依賴注入）**
- 全專案使用 DI 容器管理依賴關係

---

## 測試策略

### 單元測試
- 位置：`/LittleFlowerBotTests`
- 測試覆蓋：領域邏輯、服務、工具類別
- 框架：NUnit + NSubstitute

### 測試組織
```
LittleFlowerBotTests/
├── Models/
│   ├── Game/ - 遊戲邏輯測試
│   └── Cache/ - 快取測試
├── Services/ - 服務測試
├── HealthChecks/ - 健康檢查測試
└── Utils/ - 工具類別測試
```

---

## 部署架構

```
Internet
    ↓
Heroku (Platform)
    ↓
LittleFlowerBot (Web App)
    ↓
├── PostgreSQL (Database)
└── Redis (Cache)
```

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
