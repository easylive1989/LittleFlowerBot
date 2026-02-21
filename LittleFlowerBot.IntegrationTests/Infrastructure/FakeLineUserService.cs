using System.Collections.Generic;
using System.Threading.Tasks;
using LittleFlowerBot.Models.Message;

namespace LittleFlowerBot.IntegrationTests.Infrastructure;

/// <summary>
/// 測試用的好友檢查服務
/// 預設所有使用者都是好友，可透過 SetNotFollower 設定非好友
/// </summary>
public class FakeLineUserService : ILineUserService
{
    private readonly HashSet<string> _notFollowers = new();

    public Task<bool> IsFollower(string userId)
    {
        return Task.FromResult(!_notFollowers.Contains(userId));
    }

    public void SetNotFollower(string userId)
    {
        _notFollowers.Add(userId);
    }

    public void Clear()
    {
        _notFollowers.Clear();
    }
}
