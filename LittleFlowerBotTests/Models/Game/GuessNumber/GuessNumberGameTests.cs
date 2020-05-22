using System;
using LittleFlowerBot.Models.Game.GuessNumber;
using LittleFlowerBot.Models.Renderer;
using NSubstitute;
using NUnit.Framework;

namespace LittleFlowerBotTests.Models.Game.GuessNumber
{
    [TestFixture]
    public class GuessNumberGameTests
    {
        private GuessNumberGame _guessNumberGame;
        private ITextRenderer _renderer;

        [Test]
        public void IsMatch_PositiveNumber()
        {
            var lineBotAction = Substitute.For<ITextRenderer>();
            
            var isMatch = new GuessNumberGame(lineBotAction).IsMatch("99");

            Assert.IsTrue(isMatch);
        }

        [Test]
        public void IsMatch_NegativeNumber()
        {
            var lineBotAction = Substitute.For<ITextRenderer>();
            
            var isMatch = new GuessNumberGame(lineBotAction).IsMatch("-117");

            Assert.IsFalse(isMatch);
        }

        [Test]
        public void StartGame_ShowMessage()
        {
            StartGameWith(87);

            MessageShouldBe("猜0 - 100", 1);
        }

        [Test]
        public void GuessNumber_SmallThanTarget()
        {
            StartGameWith(87);

            GuessNumber("50");

            MessageShouldBe("猜50 - 100", 1);
        }

        [Test]
        public void GuessNumber_OutOfRange()
        {
            StartGameWith(87);

            GuessNumber("110");

            MessageShouldBe("猜0 - 100", 2);
        }

        [Test]
        public void GuessNumber_LargeThanTarget()
        {
            StartGameWith(23);

            GuessNumber("75");

            MessageShouldBe("猜0 - 75", 1);
        }

        [Test]
        public void GuessNumber_Success()
        {
            StartGameWith(23);

            GuessNumber("56");
            GuessNumber("23");

            MessageShouldBe("猜對了! 總共2次", 1);
        }

        private void MessageShouldBe(string message, int count)
        {
            _renderer.Received(count).Render("", message);
        }

        private void GuessNumber(string number)
        {
            _guessNumberGame.Act("1a2a", number);
        }

        private void StartGameWith(int target)
        {
            _renderer = Substitute.For<ITextRenderer>();
            var random = Substitute.For<Random>();
            random.Next(0).ReturnsForAnyArgs(target);
            _guessNumberGame = new GuessNumberGame(_renderer)
            {
                Random = random
            };
            _guessNumberGame.StartGame();
        }

    }
}