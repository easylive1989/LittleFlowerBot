using System;

namespace LittleFlowerBot.Models.Game.GuessNumber
{
    [Serializable]
    public class GuessNumberState : IGameState
    {
        public int _start;
        public int _end;
        public int _target;
        public int _guessCount;
        public bool _isGameOver;
        
        public bool IsGameOver()
        {
            return _isGameOver;
        }
    }
}