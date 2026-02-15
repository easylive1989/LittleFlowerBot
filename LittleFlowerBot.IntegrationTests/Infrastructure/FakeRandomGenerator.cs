using LittleFlowerBot.Models.Game.GuessNumber;

namespace LittleFlowerBot.IntegrationTests.Infrastructure;

public class FakeRandomGenerator : IRandomGenerator
{
    public int NextValue { get; set; }

    public int Next(int maxValue) => NextValue;
}
