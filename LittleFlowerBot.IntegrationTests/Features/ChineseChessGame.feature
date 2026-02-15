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
    # 黑方車從 (1,i) 下移至 (2,i)
    And 使用者 "userA" 在群組 "group4" 發送訊息 "1,i>2,i"
    # 紅方俥從 (10,i) 上移至 (9,i)
    And 使用者 "userB" 在群組 "group4" 發送訊息 "10,i>9,i"
    # 黑方車從 (2,i) 左移至 (2,f)
    And 使用者 "userA" 在群組 "group4" 發送訊息 "2,i>2,f"
    # 紅方俥從 (9,i) 左移至 (9,h)
    And 使用者 "userB" 在群組 "group4" 發送訊息 "9,i>9,h"
    # 黑方車從 (2,f) 直下吃掉紅方仕 (10,f)
    And 使用者 "userA" 在群組 "group4" 發送訊息 "2,f>10,f"
    # 紅方俥從 (9,h) 左移至 (9,f)
    And 使用者 "userB" 在群組 "group4" 發送訊息 "9,h>9,f"
    # 黑方車從 (10,f) 左移吃掉紅方帥 (10,e)，遊戲結束
    And 使用者 "userA" 在群組 "group4" 發送訊息 "10,f>10,e"
    Then 系統應該回覆包含 "遊戲結束!" 的訊息
