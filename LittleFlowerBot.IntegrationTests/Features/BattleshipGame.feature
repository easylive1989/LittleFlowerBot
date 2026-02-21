Feature: 海戰棋遊戲
    作為玩家
    我想要透過 LINE 聊天室玩海戰棋
    以便與朋友進行策略對戰

Background:
    Given 測試環境已準備就緒

Scenario: 非好友玩家無法加入海戰棋
    When 使用者 "userA" 在群組 "group1" 發送訊息 "玩海戰棋"
    And 使用者 "userC" 不是 Bot 好友
    And 使用者 "userC" 在群組 "group1" 發送訊息 "++"
    Then 系統應該回覆包含 "好友" 的訊息

Scenario: 創建海戰棋遊戲
    When 使用者 "userA" 在群組 "group1" 發送訊息 "玩海戰棋"
    Then 系統應該回覆包含 "++" 的訊息

Scenario: 兩位玩家加入遊戲並開始佈置
    When 使用者 "userA" 在群組 "group1" 發送訊息 "玩海戰棋"
    And 使用者 "userA" 在群組 "group1" 發送訊息 "++"
    And 使用者 "userB" 在群組 "group1" 發送訊息 "++"
    Then 系統應該回覆包含 "佈置" 的訊息

Scenario: 完整遊戲流程至戰鬥開始
    When 使用者 "userA" 在群組 "group1" 發送訊息 "玩海戰棋"
    And 使用者 "userA" 在群組 "group1" 發送訊息 "++"
    And 使用者 "userB" 在群組 "group1" 發送訊息 "++"
    # playerA 佈置船艦
    And 使用者 "userA" 在群組 "group1" 發送訊息 "放置 航母 a1 橫"
    And 使用者 "userA" 在群組 "group1" 發送訊息 "放置 戰艦 a2 橫"
    And 使用者 "userA" 在群組 "group1" 發送訊息 "放置 巡洋艦 a3 橫"
    And 使用者 "userA" 在群組 "group1" 發送訊息 "放置 潛艇 a4 橫"
    And 使用者 "userA" 在群組 "group1" 發送訊息 "放置 驅逐艦 a5 橫"
    # playerB 佈置船艦
    And 使用者 "userB" 在群組 "group1" 發送訊息 "放置 航母 a1 橫"
    And 使用者 "userB" 在群組 "group1" 發送訊息 "放置 戰艦 a2 橫"
    And 使用者 "userB" 在群組 "group1" 發送訊息 "放置 巡洋艦 a3 橫"
    And 使用者 "userB" 在群組 "group1" 發送訊息 "放置 潛艇 a4 橫"
    And 使用者 "userB" 在群組 "group1" 發送訊息 "放置 驅逐艦 a5 橫"
    Then 系統應該回覆包含 "戰鬥開始" 的訊息

Scenario: 完整遊戲流程至勝利
    When 使用者 "userA" 在群組 "group1" 發送訊息 "玩海戰棋"
    And 使用者 "userA" 在群組 "group1" 發送訊息 "++"
    And 使用者 "userB" 在群組 "group1" 發送訊息 "++"
    And 使用者 "userA" 在群組 "group1" 發送訊息 "放置 航母 a1 橫"
    And 使用者 "userA" 在群組 "group1" 發送訊息 "放置 戰艦 a2 橫"
    And 使用者 "userA" 在群組 "group1" 發送訊息 "放置 巡洋艦 a3 橫"
    And 使用者 "userA" 在群組 "group1" 發送訊息 "放置 潛艇 a4 橫"
    And 使用者 "userA" 在群組 "group1" 發送訊息 "放置 驅逐艦 a5 橫"
    And 使用者 "userB" 在群組 "group1" 發送訊息 "放置 航母 a1 橫"
    And 使用者 "userB" 在群組 "group1" 發送訊息 "放置 戰艦 a2 橫"
    And 使用者 "userB" 在群組 "group1" 發送訊息 "放置 巡洋艦 a3 橫"
    And 使用者 "userB" 在群組 "group1" 發送訊息 "放置 潛艇 a4 橫"
    And 使用者 "userB" 在群組 "group1" 發送訊息 "放置 驅逐艦 a5 橫"
    # 攻擊流程 - playerA 擊沉 playerB 所有船艦
    And 使用者 "userA" 在群組 "group1" 發送訊息 "a1"
    And 使用者 "userB" 在群組 "group1" 發送訊息 "j10"
    And 使用者 "userA" 在群組 "group1" 發送訊息 "b1"
    And 使用者 "userB" 在群組 "group1" 發送訊息 "j9"
    And 使用者 "userA" 在群組 "group1" 發送訊息 "c1"
    And 使用者 "userB" 在群組 "group1" 發送訊息 "j8"
    And 使用者 "userA" 在群組 "group1" 發送訊息 "d1"
    And 使用者 "userB" 在群組 "group1" 發送訊息 "j7"
    And 使用者 "userA" 在群組 "group1" 發送訊息 "e1"
    And 使用者 "userB" 在群組 "group1" 發送訊息 "j6"
    And 使用者 "userA" 在群組 "group1" 發送訊息 "a2"
    And 使用者 "userB" 在群組 "group1" 發送訊息 "j5"
    And 使用者 "userA" 在群組 "group1" 發送訊息 "b2"
    And 使用者 "userB" 在群組 "group1" 發送訊息 "j4"
    And 使用者 "userA" 在群組 "group1" 發送訊息 "c2"
    And 使用者 "userB" 在群組 "group1" 發送訊息 "j3"
    And 使用者 "userA" 在群組 "group1" 發送訊息 "d2"
    And 使用者 "userB" 在群組 "group1" 發送訊息 "j2"
    And 使用者 "userA" 在群組 "group1" 發送訊息 "a3"
    And 使用者 "userB" 在群組 "group1" 發送訊息 "j1"
    And 使用者 "userA" 在群組 "group1" 發送訊息 "b3"
    And 使用者 "userB" 在群組 "group1" 發送訊息 "i10"
    And 使用者 "userA" 在群組 "group1" 發送訊息 "c3"
    And 使用者 "userB" 在群組 "group1" 發送訊息 "i9"
    And 使用者 "userA" 在群組 "group1" 發送訊息 "a4"
    And 使用者 "userB" 在群組 "group1" 發送訊息 "i8"
    And 使用者 "userA" 在群組 "group1" 發送訊息 "b4"
    And 使用者 "userB" 在群組 "group1" 發送訊息 "i7"
    And 使用者 "userA" 在群組 "group1" 發送訊息 "c4"
    And 使用者 "userB" 在群組 "group1" 發送訊息 "i6"
    And 使用者 "userA" 在群組 "group1" 發送訊息 "a5"
    And 使用者 "userB" 在群組 "group1" 發送訊息 "i5"
    And 使用者 "userA" 在群組 "group1" 發送訊息 "b5"
    Then 系統應該回覆包含 "勝利" 的訊息
