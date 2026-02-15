using System;
using System.Text.RegularExpressions;

namespace LittleFlowerBot.Models.Game.GuessNumber
{
    public class GuessNumberGame : Game
    {
        private readonly IRandomGenerator _random;

        public GuessNumberGame(IRandomGenerator random)
        {
            _random = random;
            GameBoard = new GuessNumberBoard();
        }
        
        public override void Act(string userId, string cmd)
        {
            if (IsCmdInvalid(cmd))
            {
                return;
            }
            
            var number = Int32.Parse(cmd);
            GetState()._guessCount++;
            if (IsGuessSuccess(number))
            {
                if (IsNumberInInterval(number))
                {
                    CalculateNewInterval(number);
                }
                ShowInterval();
            }
            else
            {
                Render("猜對了! 總共" + GetState()._guessCount + "次");
                GetState()._isGameOver = true;
            }
        }

        private void CalculateNewInterval(int number)
        {
            if (number > GetState()._target)
            {
                GetState()._end = number;
            }
            else
            {
                GetState()._start = number;
            }
        }

        private bool IsNumberInInterval(int number)
        {
            return number > GetState()._start && number < GetState()._end;
        }

        private bool IsGuessSuccess(int number)
        {
            return number != GetState()._target;
        }

        private bool IsCmdInvalid(string cmd)
        {
            return !new Regex(@"^[0-9]+$").IsMatch(cmd);
        }

        public override void StartGame()
        {
            GetState()._start = 0;
            GetState()._end = 100;
            GetState()._guessCount = 0;
            GetState()._target = _random.Next(GetState()._end);
            ShowInterval();
        }

        private void ShowInterval()
        {
            Render("猜" + GetState()._start + " - " + GetState()._end);
        }

        private GuessNumberBoard GetState()
        {
            return (GuessNumberBoard) GameBoard;
        }
    }
}