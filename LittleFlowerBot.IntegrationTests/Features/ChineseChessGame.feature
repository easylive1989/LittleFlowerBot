Feature: 象棋遊戲
    作為玩家
    我想要透過 LINE 聊天室玩象棋
    以便與朋友互動

Background:
    Given 測試環境已準備就緒

Scenario: 創建象棋遊戲
    When 使用者 "userA" 在群組 "group4" 發送訊息 "玩象棋"
    Then 回應狀態碼應該是 200
    And 系統應該回覆包含 "輸入++參加遊戲" 的訊息

Scenario: 兩位玩家加入遊戲
    When 使用者 "userA" 在群組 "group4" 發送訊息 "玩象棋"
    And 使用者 "userA" 在群組 "group4" 發送訊息 "++"
    And 使用者 "userB" 在群組 "group4" 發送訊息 "++"
    Then 系統應該回覆包含 "遊戲開始" 的訊息

Scenario: 完成一局遊戲
    When 使用者 "userA" 在群組 "group4" 發送訊息 "玩象棋"
    And 使用者 "userA" 在群組 "group4" 發送訊息 "++"
    And 使用者 "userB" 在群組 "group4" 發送訊息 "++"
    # 黑方車從 (1i) 下移至 (2i)
    And 使用者 "userA" 在群組 "group4" 發送訊息 "1i>2i"
    # 紅方俥從 (10i) 上移至 (9i)
    And 使用者 "userB" 在群組 "group4" 發送訊息 "10i>9i"
    # 黑方車從 (2i) 左移至 (2f)
    And 使用者 "userA" 在群組 "group4" 發送訊息 "2i>2f"
    # 紅方俥從 (9i) 左移至 (9h)
    And 使用者 "userB" 在群組 "group4" 發送訊息 "9i>9h"
    # 黑方車從 (2f) 直下吃掉紅方仕 (10f)
    And 使用者 "userA" 在群組 "group4" 發送訊息 "2f>10f"
    # 紅方俥從 (9h) 左移至 (9f)
    And 使用者 "userB" 在群組 "group4" 發送訊息 "9h>9f"
    # 黑方車從 (10f) 左移吃掉紅方帥 (10e)，遊戲結束
    And 使用者 "userA" 在群組 "group4" 發送訊息 "10f>10e"
    Then 系統應該回覆包含 "遊戲結束!" 的訊息
