using System;
using System.Text.RegularExpressions;
using LittleFlowerBot.Models.Renderer;

namespace LittleFlowerBot.Models.Game.GuessNumber
{
    public class GuessNumberGame : Game
    {
        public Random Random = new Random();

        public GuessNumberGame(ITextRenderer textRenderer) : base(textRenderer)
        {
            GameState = new GuessNumberState();
        }
        
        public override void Act(string userId, string cmd)
        {
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

        public override bool IsMatch(string cmd)
        {
            return new Regex(@"^[0-9]+$").IsMatch(cmd);
        }

        public override void StartGame()
        {
            GetState()._start = 0;
            GetState()._end = 100;
            GetState()._guessCount = 0;
            GetState()._target = Random.Next(GetState()._end);
            ShowInterval();
        }

        private void ShowInterval()
        {
            Render("猜" + GetState()._start + " - " + GetState()._end);
        }

        private GuessNumberState GetState()
        {
            return (GuessNumberState) GameState;
        }
    }
}