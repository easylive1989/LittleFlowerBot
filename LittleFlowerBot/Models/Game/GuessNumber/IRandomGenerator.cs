namespace LittleFlowerBot.Models.Game.GuessNumber
{
    public interface IRandomGenerator
    {
        int Next(int maxValue);
    }

    public class RandomGenerator : IRandomGenerator
    {
        private readonly Random _random = new();

        public int Next(int maxValue) => _random.Next(maxValue);
    }
}
