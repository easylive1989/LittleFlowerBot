# 小遊戲 Line Bot
![build](https://github.com/easylive1989/LittleFlowerBot/actions/workflows/dotnetcore.yml/badge.svg)
[![BCH compliance](https://bettercodehub.com/edge/badge/easylive1989/LittleFlowerBot?branch=master)](https://bettercodehub.com/)

一個基於 Line Messaging API 的小遊戲機器人，支援多種雙人 / 單人遊戲。

![image](https://github.com/easylive1989/LittleFlowerBot/blob/master/Images/LittleFlowerBot%20Banner.png)

## 開始使用

1. 將 `LittleFlowerBot` 加為好友（或加進群組 / 聊天室）
2. 直接輸入指令即可開始遊戲，例如 `玩五子棋`

> 📖 詳細玩法請見 [HOW TO PLAY.md](./HOW%20TO%20PLAY.md)

### 遊戲列表
- 玩猜數字
- 玩井字遊戲
- 玩五子棋
- 玩象棋
- 玩海戰棋

# 系統相關

![image](https://github.com/easylive1989/LittleFlowerBot/blob/master/Images/message%20processing.png)

## 使用技術

### 核心框架
- **.NET 10.0** - 應用程式框架
- **ASP.NET Core** - Web API 框架

### 資料庫
- **MongoDB** - 主要資料庫（戰績與遊戲狀態）

### 外部服務
- **Line Messaging API** - Line Bot 訊息處理

### 圖像渲染
- **SkiaSharp** - 棋盤圖片渲染（海戰棋等）

### 測試
- **NUnit 4.2** - 單元測試框架
- **NSubstitute 5.3** - Mocking 框架

### 部署
- **Render** - 雲端平台（Docker）

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
│   └── EventHandler/   # Line Bot 事件處理器
├── Models/              # 領域模型與 DTOs
│   ├── Game/           # 遊戲邏輯
│   ├── Caches/         # 快取服務
│   ├── Renderer/       # 渲染服務
│   └── ...
├── Repositories/        # 資料存取層
└── DbContexts/         # MongoDB Context
```

## API 端點

### 健康檢查
- `GET /health` - 完整健康檢查（所有項目）
- `GET /health/ready` - 就緒檢查（含 MongoDB 連線檢查）
- `GET /health/live` - 存活檢查（僅應用程式狀態）

### Line Webhook
- `POST /api/LineChat/Callback` - Line Bot Webhook 端點

## 開發指南

### 前置需求
- .NET 10.0 SDK
- MongoDB（本機 `mongodb://localhost:27017` 或 MongoDB Atlas）

### 本地開發

1. 克隆專案
```bash
git clone https://github.com/easylive1989/LittleFlowerBot.git
cd LittleFlowerBot
```

2. 設定 MongoDB 連線（其中一種方式）：

**方式 A：本機 MongoDB（預設）**
無需設定，`appsettings.Development.json` 已預設 `mongodb://localhost:27017`。

**方式 B：使用 MongoDB Atlas**
```bash
export MONGODB_URI="mongodb+srv://user:password@cluster.xxxxx.mongodb.net/LittleFlowerBot?retryWrites=true&w=majority"
```

3. 啟動應用程式
```bash
dotnet run --project LittleFlowerBot
```

4. 執行測試
```bash
dotnet test
```

### 設定 User Secrets（敏感資訊）

```bash
cd LittleFlowerBot
dotnet user-secrets set "LineChannelToken" "your_line_channel_token"
```

## 功能特色

### ✅ 已實作功能
- 🎮 多種遊戲支援（猜數字、井字遊戲、五子棋、象棋、海戰棋）
- 💾 MongoDB 資料持久化（戰績與遊戲盤面）
- 🖼️ SkiaSharp 棋盤圖片渲染
- 📊 健康檢查端點（liveness / readiness / full）
- 🛡️ 全域錯誤處理中介軟體
- 📝 結構化日誌記錄
- 🧪 完整的單元測試

### 🎯 設計模式
- Factory Pattern（遊戲工廠、渲染器工廠）
- Repository Pattern（資料存取抽象）
- Strategy Pattern（不同的渲染策略）
- Dependency Injection（全專案依賴注入）

## 部署

本專案以 Docker 容器化部署，目前運行於 [Render](https://render.com/)。

### Render 部署

專案根目錄已包含 `render.yaml`（Blueprint）與 `Dockerfile`。

**所需環境變數**：

| Key | 說明 |
|---|---|
| `MONGODB_URI` | MongoDB 連線字串（必填） |
| `LineChannelToken` | Line Bot Channel Access Token（必填） |
| `BaseUrl` | 服務對外 URL（用於組合圖片連結） |
| `ASPNETCORE_URLS` | `http://*:10000` |
| `ASPNETCORE_ENVIRONMENT` | `Production` |

部署完成後，記得到 Line Developers Console 設定 Webhook URL 為：
```
https://<your-service>.onrender.com/api/LineChat/Callback
```

## 貢獻

歡迎提交 Pull Request 或開啟 Issue！

## 授權

MIT License
