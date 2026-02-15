Feature: 猜數字遊戲
    作為玩家
    我想要透過 LINE 聊天室玩猜數字
    以便娛樂

Background:
    Given 測試環境已準備就緒

Scenario: 創建猜數字遊戲
    When 使用者 "userA" 在群組 "group2" 發送訊息 "玩猜數字"
    Then 回應狀態碼應該是 200
    And 系統應該回覆包含 "猜" 的訊息

Scenario: 猜數字直到猜對
    Given 猜數字的目標數字為 87
    When 使用者 "userA" 在群組 "group2" 發送訊息 "玩猜數字"
    Then 系統應該回覆包含 "猜0 - 100" 的訊息
    When 使用者 "userA" 在群組 "group2" 發送訊息 "50"
    Then 系統應該回覆包含 "猜50 - 100" 的訊息
    When 使用者 "userA" 在群組 "group2" 發送訊息 "87"
    Then 系統應該回覆包含 "猜對了" 的訊息
