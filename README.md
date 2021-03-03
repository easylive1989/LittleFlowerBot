## How to play
在某個群組加入LittleFlowerBot與LineNotify，在群組中輸入我要註冊並點擊回覆訊息中的連結，並關聯此群組。完成關聯之後，輸入下列文字可玩遊戲：
- 玩五子棋
- 玩猜數字
- 玩象棋
- 玩井字遊戲

## How to setup server
如果想自行架設Server，可以把代碼下載到你想部署的server，並在app.setting中設定下列資訊
### 設定 postgrel connection string
### 設定 redis connection string
### 設定 line notify token
### 設定 line bot webhook endpoint

執行
```dotnet run```