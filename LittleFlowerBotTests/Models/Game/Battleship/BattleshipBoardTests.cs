using System.Text.Json;
using LittleFlowerBot.Models.Game.Battleship;
using NUnit.Framework;

namespace LittleFlowerBotTests.Models.Game.Battleship
{
    [TestFixture]
    public class BattleshipBoardTests
    {
        [Test]
        public void NewBoard_ShouldBeInWaitingPlayersPhase()
        {
            var board = new BattleshipBoard();

            Assert.That(board.Phase, Is.EqualTo(BattleshipPhase.WaitingPlayers));
        }

        [Test]
        public void NewBoard_IsGameOver_ShouldBeFalse()
        {
            var board = new BattleshipBoard();

            Assert.That(board.IsGameOver(), Is.False);
        }

        [Test]
        public void NewBoard_ShouldHaveNoPlayers()
        {
            var board = new BattleshipBoard();

            Assert.That(board.Players, Is.Empty);
        }

        [Test]
        public void Join_FirstPlayer_ShouldAddToPlayers()
        {
            var board = new BattleshipBoard();

            board.Join("playerA");

            Assert.That(board.Players, Has.Count.EqualTo(1));
            Assert.That(board.Players[0], Is.EqualTo("playerA"));
        }

        [Test]
        public void Join_SecondPlayer_ShouldTransitionToSetup()
        {
            var board = new BattleshipBoard();

            board.Join("playerA");
            board.Join("playerB");

            Assert.That(board.Phase, Is.EqualTo(BattleshipPhase.Setup));
            Assert.That(board.Players, Has.Count.EqualTo(2));
        }

        [Test]
        public void Join_SamePlayerTwice_ShouldNotDuplicate()
        {
            var board = new BattleshipBoard();

            board.Join("playerA");
            board.Join("playerA");

            Assert.That(board.Players, Has.Count.EqualTo(1));
        }

        [Test]
        public void Join_ThirdPlayer_ShouldNotAdd()
        {
            var board = new BattleshipBoard();

            board.Join("playerA");
            board.Join("playerB");
            board.Join("playerC");

            Assert.That(board.Players, Has.Count.EqualTo(2));
        }

        [Test]
        public void Join_SecondPlayer_ShouldInitPlayerStates()
        {
            var board = new BattleshipBoard();

            board.Join("playerA");
            board.Join("playerB");

            Assert.That(board.PlayerStates.ContainsKey("playerA"), Is.True);
            Assert.That(board.PlayerStates.ContainsKey("playerB"), Is.True);
        }

        [Test]
        public void PlayerState_OwnGrid_ShouldBe10x10AllEmpty()
        {
            var board = new BattleshipBoard();
            board.Join("playerA");
            board.Join("playerB");

            var state = board.PlayerStates["playerA"];

            Assert.That(state.OwnGrid.Length, Is.EqualTo(10));
            Assert.That(state.OwnGrid[0].Length, Is.EqualTo(10));
            Assert.That(state.OwnGrid[0][0], Is.EqualTo(CellState.Empty));
        }

        [Test]
        public void IsPlayerFull_TwoPlayers_ShouldBeTrue()
        {
            var board = new BattleshipBoard();
            board.Join("playerA");
            board.Join("playerB");

            Assert.That(board.IsPlayerFull(), Is.True);
        }

        [Test]
        public void IsPlayerFull_OnePlayer_ShouldBeFalse()
        {
            var board = new BattleshipBoard();
            board.Join("playerA");

            Assert.That(board.IsPlayerFull(), Is.False);
        }
        [Test]
        public void Board_ShouldSerializeAndDeserializeJson()
        {
            var board = new BattleshipBoard();
            board.Join("playerA");
            board.Join("playerB");

            var json = JsonSerializer.Serialize(board);
            var deserialized = JsonSerializer.Deserialize<BattleshipBoard>(json);

            Assert.That(deserialized, Is.Not.Null);
            Assert.That(deserialized!.Phase, Is.EqualTo(BattleshipPhase.Setup));
            Assert.That(deserialized.Players, Has.Count.EqualTo(2));
            Assert.That(deserialized.PlayerStates.ContainsKey("playerA"), Is.True);
        }
    }
}
