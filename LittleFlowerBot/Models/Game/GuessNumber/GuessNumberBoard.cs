using System;

namespace LittleFlowerBot.Models.Game.GuessNumber
{
    [Serializable]
    public class GuessNumberBoard : IGameBoard
    {
        public int _start { get; set; }
        public int _end { get; set; }
        public int _target { get; set; }
        public int _guessCount { get; set; }
        public bool _isGameOver { get; set; }
        
        public bool IsGameOver()
        {
            return _isGameOver;
        }
    }
}