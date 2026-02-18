Feature: 井字遊戲
    作為玩家
    我想要透過 LINE 聊天室玩井字遊戲
    以便與朋友互動

Background:
    Given 測試環境已準備就緒

Scenario: 創建井字遊戲
    When 使用者 "userA" 在群組 "group1" 發送訊息 "玩井字遊戲"
    Then 回應狀態碼應該是 200
    And 系統應該回覆包含 "輸入++參加遊戲" 的訊息

Scenario: 兩位玩家加入遊戲
    When 使用者 "userA" 在群組 "group1" 發送訊息 "玩井字遊戲"
    And 使用者 "userA" 在群組 "group1" 發送訊息 "++"
    And 使用者 "userB" 在群組 "group1" 發送訊息 "++"
    Then 系統應該回覆包含 "遊戲開始" 的訊息

Scenario: 完成一局遊戲並記錄勝負
    When 使用者 "userA" 在群組 "group1" 發送訊息 "玩井字遊戲"
    And 使用者 "userA" 在群組 "group1" 發送訊息 "++"
    And 使用者 "userB" 在群組 "group1" 發送訊息 "++"
    And 使用者 "userA" 在群組 "group1" 發送訊息 "2b"
    And 使用者 "userB" 在群組 "group1" 發送訊息 "1a"
    And 使用者 "userA" 在群組 "group1" 發送訊息 "2a"
    And 使用者 "userB" 在群組 "group1" 發送訊息 "1b"
    And 使用者 "userA" 在群組 "group1" 發送訊息 "2c"
    Then 系統應該回覆包含 "遊戲結束!" 的訊息
    And 資料庫中應該有 2 筆井字遊戲結果

Scenario: 平局遊戲
    When 使用者 "userA" 在群組 "group1" 發送訊息 "玩井字遊戲"
    And 使用者 "userA" 在群組 "group1" 發送訊息 "++"
    And 使用者 "userB" 在群組 "group1" 發送訊息 "++"
    And 使用者 "userA" 在群組 "group1" 發送訊息 "2b"
    And 使用者 "userB" 在群組 "group1" 發送訊息 "1a"
    And 使用者 "userA" 在群組 "group1" 發送訊息 "1b"
    And 使用者 "userB" 在群組 "group1" 發送訊息 "3b"
    And 使用者 "userA" 在群組 "group1" 發送訊息 "2a"
    And 使用者 "userB" 在群組 "group1" 發送訊息 "2c"
    And 使用者 "userA" 在群組 "group1" 發送訊息 "3a"
    And 使用者 "userB" 在群組 "group1" 發送訊息 "1c"
    And 使用者 "userA" 在群組 "group1" 發送訊息 "3c"
    Then 系統應該回覆包含 "遊戲結束!" 的訊息
    And 資料庫中應該有 2 筆平局結果
