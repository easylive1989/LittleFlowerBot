using LittleFlowerBot.Models.Game.Battleship;
using LittleFlowerBot.Models.Renderer;
using LittleFlowerBot.Repositories;
using NSubstitute;
using NUnit.Framework;

namespace LittleFlowerBotTests.Models.Game.Battleship
{
    [TestFixture]
    public class BattleshipGameTests
    {
        private BattleshipGame _game = null!;
        private ITextRenderer _renderer = null!;

        private void StartGame()
        {
            _renderer = Substitute.For<ITextRenderer>();
            var repository = Substitute.For<IBoardGameResultsRepository>();
            _game = new BattleshipGame(repository);
            _game.TextRenderer = _renderer;
            _game.GameBoard = new BattleshipBoard();
            _game.StartGame();
        }

        private void Act(string userId, string cmd)
        {
            _game.Act(userId, cmd);
        }

        private void MessageShouldContain(string text)
        {
            _renderer.Received().Render(Arg.Is<string>(s => s.Contains(text)));
        }

        private void PrivateMessageShouldContain(string userId, string text)
        {
            _renderer.Received().RenderPrivate(userId, Arg.Is<string>(s => s.Contains(text)));
        }

        [Test]
        public void StartGame_ShouldShowInviteMessage()
        {
            StartGame();

            _renderer.Received(1).Render(Arg.Is<string>(s => s.Contains("++")));
        }

        [Test]
        public void Act_FirstPlayerJoin_ShouldShowWaitingMessage()
        {
            StartGame();
            _renderer.ClearReceivedCalls();

            Act("playerA", "++");

            _renderer.Received().Render(Arg.Is<string>(s => s.Contains("playerA")));
        }

        [Test]
        public void Act_SamePlayerJoinTwice_ShouldShowAlreadyJoinedMessage()
        {
            StartGame();
            Act("playerA", "++");
            _renderer.ClearReceivedCalls();

            Act("playerA", "++");

            _renderer.Received().Render(Arg.Is<string>(s => s.Contains("已經加入")));
        }

        [Test]
        public void Act_TwoPlayersJoin_ShouldStartSetupPhase()
        {
            StartGame();

            Act("playerA", "++");
            Act("playerB", "++");

            _renderer.Received().Render(Arg.Is<string>(s => s.Contains("佈置")));
        }

        [Test]
        public void Act_TwoPlayersJoin_ShouldPrivateMessageBothPlayers()
        {
            StartGame();

            Act("playerA", "++");
            Act("playerB", "++");

            _renderer.Received().RenderPrivate("playerA", Arg.Any<string>());
            _renderer.Received().RenderPrivate("playerB", Arg.Any<string>());
        }

        // === 佈置船艦測試 ===

        private void GivenTwoPlayerJoined()
        {
            StartGame();
            Act("playerA", "++");
            Act("playerB", "++");
            _renderer.ClearReceivedCalls();
        }

        [Test]
        public void PlaceShip_ValidPlacement_ShouldSucceed()
        {
            GivenTwoPlayerJoined();

            Act("playerA", "放置 航母 a1 橫");

            _renderer.Received().RenderPrivate("playerA", Arg.Is<string>(s => s.Contains("航母")));
        }

        [Test]
        public void PlaceShip_ShouldUpdateOwnGrid()
        {
            GivenTwoPlayerJoined();

            Act("playerA", "放置 驅逐艦 a1 橫");

            var board = (BattleshipBoard)_game.GameBoard;
            var state = board.PlayerStates["playerA"];
            Assert.That(state.OwnGrid[0][0], Is.EqualTo(CellState.Ship));
            Assert.That(state.OwnGrid[0][1], Is.EqualTo(CellState.Ship));
            Assert.That(state.OwnGrid[0][2], Is.EqualTo(CellState.Empty));
        }

        [Test]
        public void PlaceShip_Vertical_ShouldPlaceCorrectly()
        {
            GivenTwoPlayerJoined();

            Act("playerA", "放置 驅逐艦 a1 直");

            var board = (BattleshipBoard)_game.GameBoard;
            var state = board.PlayerStates["playerA"];
            Assert.That(state.OwnGrid[0][0], Is.EqualTo(CellState.Ship));
            Assert.That(state.OwnGrid[1][0], Is.EqualTo(CellState.Ship));
            Assert.That(state.OwnGrid[2][0], Is.EqualTo(CellState.Empty));
        }

        [Test]
        public void PlaceShip_OutOfBounds_ShouldShowError()
        {
            GivenTwoPlayerJoined();

            Act("playerA", "放置 航母 g1 橫");

            _renderer.Received().RenderPrivate("playerA", Arg.Is<string>(s => s.Contains("超出")));
        }

        [Test]
        public void PlaceShip_Overlapping_ShouldShowError()
        {
            GivenTwoPlayerJoined();
            Act("playerA", "放置 驅逐艦 a1 橫");
            _renderer.ClearReceivedCalls();

            Act("playerA", "放置 巡洋艦 a1 直");

            _renderer.Received().RenderPrivate("playerA", Arg.Is<string>(s => s.Contains("重疊")));
        }

        [Test]
        public void PlaceShip_DuplicateType_ShouldShowError()
        {
            GivenTwoPlayerJoined();
            Act("playerA", "放置 驅逐艦 a1 橫");
            _renderer.ClearReceivedCalls();

            Act("playerA", "放置 驅逐艦 c1 橫");

            _renderer.Received().RenderPrivate("playerA", Arg.Is<string>(s => s.Contains("已放置")));
        }

        [Test]
        public void PlaceShip_InvalidCommand_ShouldBeIgnored()
        {
            GivenTwoPlayerJoined();

            Act("playerA", "放置 abc");

            _renderer.DidNotReceive().RenderPrivate(Arg.Any<string>(), Arg.Any<string>());
        }

        [Test]
        public void PlaceShip_AllShipsPlaced_ShouldMarkSetupComplete()
        {
            GivenTwoPlayerJoined();

            PlaceAllShips("playerA");

            var board = (BattleshipBoard)_game.GameBoard;
            Assert.That(board.PlayerStates["playerA"].IsSetupComplete, Is.True);
        }

        [Test]
        public void PlaceShip_BothPlayersComplete_ShouldStartBattle()
        {
            GivenTwoPlayerJoined();

            PlaceAllShips("playerA");
            PlaceAllShips("playerB");

            var board = (BattleshipBoard)_game.GameBoard;
            Assert.That(board.Phase, Is.EqualTo(BattleshipPhase.Battle));
        }

        [Test]
        public void PlaceShip_BothPlayersComplete_ShouldAnnounceInGroup()
        {
            GivenTwoPlayerJoined();

            PlaceAllShips("playerA");
            _renderer.ClearReceivedCalls();
            PlaceAllShips("playerB");

            _renderer.Received().Render(Arg.Is<string>(s => s.Contains("開始")));
        }

        [Test]
        public void PlaceShip_ShouldSendBoardImageToPlayer()
        {
            GivenTwoPlayerJoined();

            Act("playerA", "放置 驅逐艦 a1 橫");

            _renderer.Received().RenderPrivateImage("playerA", Arg.Any<byte[]>());
        }

        [Test]
        public void PlaceShip_ShowsProgress()
        {
            GivenTwoPlayerJoined();

            Act("playerA", "放置 驅逐艦 a1 橫");

            _renderer.Received().Render(Arg.Is<string>(s => s.Contains("1/5")));
        }

        private void PlaceAllShips(string userId)
        {
            Act(userId, "放置 航母 a1 橫");
            Act(userId, "放置 戰艦 a2 橫");
            Act(userId, "放置 巡洋艦 a3 橫");
            Act(userId, "放置 潛艇 a4 橫");
            Act(userId, "放置 驅逐艦 a5 橫");
        }

        // === 攻擊流程測試 ===

        private void GivenBattlePhase()
        {
            GivenTwoPlayerJoined();
            PlaceAllShips("playerA");
            PlaceAllShips("playerB");
            _renderer.ClearReceivedCalls();
        }

        [Test]
        public void Attack_Hit_ShouldAnnounceInGroup()
        {
            GivenBattlePhase();

            // playerA 的船在 row=0, col=0-4; playerB 的船也在相同位置
            // playerA 先攻，攻擊 playerB 的 a1 (row=0, col=0) 應該命中
            Act("playerA", "a1");

            _renderer.Received().Render(Arg.Is<string>(s => s.Contains("命中")));
        }

        [Test]
        public void Attack_Miss_ShouldAnnounceInGroup()
        {
            GivenBattlePhase();

            // playerB 沒有船在 row=9 (第10行)
            Act("playerA", "a10");

            _renderer.Received().Render(Arg.Is<string>(s => s.Contains("未命中")));
        }

        [Test]
        public void Attack_ShouldSendBoardImagesToPlayers()
        {
            GivenBattlePhase();

            Act("playerA", "a1"); // hit

            // Attacker gets attack grid image
            _renderer.Received().RenderPrivateImage("playerA", Arg.Any<byte[]>());
            // Defender gets own grid image
            _renderer.Received().RenderPrivateImage("playerB", Arg.Any<byte[]>());
        }

        [Test]
        public void Attack_NotYourTurn_ShouldShowError()
        {
            GivenBattlePhase();

            // playerA 先攻，playerB 不應該能攻擊
            Act("playerB", "a1");

            _renderer.Received().Render(Arg.Is<string>(s => s.Contains("不是你的回合")));
        }

        [Test]
        public void Attack_ShouldAlternateTurns()
        {
            GivenBattlePhase();

            Act("playerA", "a10"); // miss
            _renderer.ClearReceivedCalls();
            Act("playerB", "a10"); // playerB's turn now

            _renderer.DidNotReceive().Render(Arg.Is<string>(s => s.Contains("不是你的回合")));
        }

        [Test]
        public void Attack_DuplicateCoordinate_ShouldShowError()
        {
            GivenBattlePhase();

            Act("playerA", "a10");
            Act("playerB", "a10");
            _renderer.ClearReceivedCalls();
            Act("playerA", "a10"); // 重複攻擊

            _renderer.Received().RenderPrivate("playerA", Arg.Is<string>(s => s.Contains("已經攻擊過")));
        }

        [Test]
        public void Attack_SinkShip_ShouldAnnounce()
        {
            GivenBattlePhase();

            // playerB 的驅逐艦在 row=4, col=0-1
            Act("playerA", "a5"); // row=4, col=0 → hit
            Act("playerB", "a10"); // miss, just to alternate
            Act("playerA", "b5"); // row=4, col=1 → hit, sink!

            _renderer.Received().Render(Arg.Is<string>(s => s.Contains("擊沉")));
        }

        [Test]
        public void Attack_AllShipsSunk_ShouldEndGame()
        {
            GivenBattlePhase();

            // Sink all of playerB's ships
            SinkAllShips("playerA", "playerB");

            var board = (BattleshipBoard)_game.GameBoard;
            Assert.That(board.IsGameOver(), Is.True);
        }

        [Test]
        public void Attack_AllShipsSunk_ShouldAnnounceWinner()
        {
            GivenBattlePhase();

            SinkAllShips("playerA", "playerB");

            _renderer.Received().Render(Arg.Is<string>(s => s.Contains("勝利")));
        }

        /// <summary>
        /// Attacks all of defender's ships, alternating turns with defender missing.
        /// Defender's ships are at:
        /// 航母:   row=0, col=0-4
        /// 戰艦:   row=1, col=0-3
        /// 巡洋艦: row=2, col=0-2
        /// 潛艇:   row=3, col=0-2
        /// 驅逐艦: row=4, col=0-1
        /// </summary>
        private void SinkAllShips(string attacker, string defender)
        {
            var targets = new[]
            {
                "a1", "b1", "c1", "d1", "e1", // 航母 row=0
                "a2", "b2", "c2", "d2",        // 戰艦 row=1
                "a3", "b3", "c3",              // 巡洋艦 row=2
                "a4", "b4", "c4",              // 潛艇 row=3
                "a5", "b5",                    // 驅逐艦 row=4
            };

            var defenderMissTarget = 10; // row=9, different columns to avoid duplicates
            foreach (var target in targets)
            {
                Act(attacker, target);
                if (!((BattleshipBoard)_game.GameBoard).IsGameOver())
                {
                    var missCol = (char)('a' + (defenderMissTarget % 10));
                    Act(defender, $"{missCol}{(defenderMissTarget / 10) + 6}");
                    defenderMissTarget++;
                }
            }
        }
    }
}
