Feature: Health Check API
    作為系統管理員
    我想要檢查系統的健康狀態
    以便確保所有服務都正常運作

Background:
    Given 測試環境已準備就緒

Scenario: 檢查完整的健康狀態
    When 我發送 GET 請求到 "/health"
    Then 回應狀態碼應該是 200
    And 回應內容應該是 JSON 格式
    And JSON 回應中應該包含 "status" 欄位
    And "status" 欄位的值應該是 "Healthy"

Scenario: 檢查就緒狀態
    When 我發送 GET 請求到 "/health/ready"
    Then 回應狀態碼應該是 200
    And JSON 回應中應該包含 "checks" 欄位
    And "checks" 欄位應該包含 "PostgreSQL" 檢查項目

Scenario: 檢查存活狀態
    When 我發送 GET 請求到 "/health/live"
    Then 回應狀態碼應該是 200
    And JSON 回應中應該包含 "checks" 欄位
    And "checks" 欄位應該包含 "Application" 檢查項目

Scenario: 檢查 PostgreSQL 連線
    When 我發送 GET 請求到 "/health"
    Then 回應狀態碼應該是 200
    And PostgreSQL 健康檢查應該通過

Scenario: 檢查 Redis 連線
    When 我發送 GET 請求到 "/health"
    Then 回應狀態碼應該是 200
    And Redis 健康檢查應該通過

Scenario: 檢查記憶體使用狀況
    When 我發送 GET 請求到 "/health"
    Then 回應狀態碼應該是 200
    And 記憶體健康檢查應該包含使用量資訊
