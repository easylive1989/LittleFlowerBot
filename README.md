# 小遊戲 Line Bot
![build](https://github.com/easylive1989/LittleFlowerBot/actions/workflows/dotnetcore.yml/badge.svg)
[![BCH compliance](https://bettercodehub.com/edge/badge/easylive1989/LittleFlowerBot?branch=master)](https://bettercodehub.com/)


### 註冊群組
在某個群組加入LittleFlowerBot與LineNotify，在群組中輸入我要註冊並點擊回覆訊息中的連結，並關聯此群組。
![image](https://github.com/easylive1989/LittleFlowerBot/blob/master/Images/LittleFlowerBot%20Banner.png)

### 遊戲列表
完成群組綁定之後，輸入下列文字可進行遊戲：
- 玩五子棋
- 玩猜數字
- 玩象棋
- 玩井字遊戲

# 系統相關

![image](https://github.com/easylive1989/LittleFlowerBot/blob/master/Images/message%20processing.png)

## 使用技術

### 核心框架
- **.NET 10.0** - 最新的 .NET 平台
- **ASP.NET Core** - Web API 框架（Minimal API 模式）
- **Entity Framework Core 9.0** - ORM 框架

### 資料庫與快取
- **PostgreSQL** - 主要資料庫
- **Redis** - 分散式快取（使用 StackExchange.Redis）

### 外部服務
- **Line Messaging API** - Line Bot 訊息處理
- **Line Notify API** - 推播通知

### 測試
- **NUnit 4.2** - 單元測試框架
- **NSubstitute 5.3** - Mocking 框架

### 部署
- **Heroku** - 雲端平台

## 架構設計

本專案採用**分層架構**設計，各層職責清晰分離：

```
📊 展示層 (Presentation)
    ↓
🎯 應用層 (Application)
    ↓
🏗️ 領域層 (Domain)
    ↑
🔧 基礎設施層 (Infrastructure)
```

詳細架構說明請參考 [ARCHITECTURE.md](./ARCHITECTURE.md)

## 專案結構

```
LittleFlowerBot/
├── Controllers/         # Web API 控制器
├── Middlewares/         # HTTP 中介軟體
├── HealthChecks/        # 健康檢查
├── Services/            # 應用服務
├── Models/              # 領域模型與 DTOs
│   ├── Game/           # 遊戲邏輯
│   ├── Caches/         # 快取服務
│   ├── Renderer/       # 渲染服務
│   └── ...
├── Repositories/        # 資料存取層
└── DbContexts/         # EF Core DbContext
```

## API 端點

### 健康檢查
- `GET /health` - 完整健康檢查（所有項目）
- `GET /health/ready` - 就緒檢查（Kubernetes readiness probe）
- `GET /health/live` - 存活檢查（Kubernetes liveness probe）

### Line Webhook
- `POST /api/linebot` - Line Bot Webhook 端點

## 開發指南

### 前置需求
- .NET 10.0 SDK
- PostgreSQL
- Redis（選用，開發環境會自動使用記憶體快取）

### 本地開發

1. 克隆專案
```bash
git clone https://github.com/easylive1989/LittleFlowerBot.git
cd LittleFlowerBot
```

2. 設定資料庫連線
```bash
# 在 appsettings.Development.json 中設定
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=littleflowerbot;Username=postgres;Password=your_password"
  }
}
```

3. 執行資料庫遷移
```bash
dotnet ef database update --project LittleFlowerBot
```

4. 啟動應用程式
```bash
dotnet run --project LittleFlowerBot
```

5. 執行測試
```bash
dotnet test
```

### 設定 User Secrets（敏感資訊）

```bash
cd LittleFlowerBot
dotnet user-secrets set "LINE_CHANNEL_TOKEN" "your_line_channel_token"
dotnet user-secrets set "LINE_NOTIFY_CLIENT_ID" "your_client_id"
dotnet user-secrets set "LINE_NOTIFY_CLIENT_SECRET" "your_client_secret"
```

## 功能特色

### ✅ 已實作功能
- 🎮 多種遊戲支援（五子棋、象棋、井字遊戲、猜數字）
- 🔔 Line Notify 通知整合
- 💾 Redis 分散式快取
- 📊 詳細的健康檢查端點
- 🛡️ 全域錯誤處理中介軟體
- 📝 結構化日誌記錄
- 🧪 完整的單元測試（78+ 測試）

### 🎯 設計模式
- Factory Pattern（遊戲工廠、渲染器工廠）
- Repository Pattern（資料存取抽象）
- Strategy Pattern（不同的渲染策略）
- Dependency Injection（全專案依賴注入）

## 效能與監控

### Health Check 回應範例

```json
{
  "status": "Healthy",
  "totalDuration": 45,
  "timestamp": "2024-02-12T10:30:00Z",
  "checks": {
    "PostgreSQL": {
      "status": "Healthy",
      "duration": 12
    },
    "Application": {
      "status": "Healthy",
      "duration": 2,
      "data": {
        "version": "1.0.0",
        "uptime": "2d 5h 30m",
        "memoryUsedMB": 145
      }
    },
    "Memory": {
      "status": "Healthy",
      "duration": 1,
      "data": {
        "allocatedMB": 128,
        "gen0Collections": 15
      }
    }
  }
}
```

## 部署

本專案可以部署到 Heroku 或任何支援 .NET 的平台。

### Heroku 部署

1. 設定環境變數
```bash
heroku config:set DATABASE_URL=postgres://...
heroku config:set HEROKU_REDIS_MAUVE_URL=redis://...
heroku config:set LINE_CHANNEL_TOKEN=...
```

2. 推送到 Heroku
```bash
git push heroku master
```

## 貢獻

歡迎提交 Pull Request 或開啟 Issue！

## 授權

MIT License
