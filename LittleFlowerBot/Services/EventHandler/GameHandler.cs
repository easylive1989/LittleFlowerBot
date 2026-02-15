using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using isRock.LineBot;
using LittleFlowerBot.Extensions;
using LittleFlowerBot.Models.Caches;
using LittleFlowerBot.Models.Game;
using LittleFlowerBot.Models.Game.BoardGame.ChessGames.ChineseChess;
using LittleFlowerBot.Models.Game.BoardGame.KiGames.Gomoku;
using LittleFlowerBot.Models.Game.BoardGame.KiGames.TicTacToe;
using LittleFlowerBot.Models.Game.GuessNumber;
using LittleFlowerBot.Models.Renderer;

namespace LittleFlowerBot.Services.EventHandler
{
    public class GameHandler : ILineEventHandler
    {
        private readonly Dictionary<string, Type> _cmdGameTypeDict = new Dictionary<string, Type>()
        {
            {"玩猜數字", typeof(GuessNumberBoard)},
            {"玩井字遊戲", typeof(TicTacToeBoard)},
            {"玩五子棋", typeof(GomokuBoard)},
            {"玩象棋", typeof(ChineseChessBoard)},
        };

        private readonly IGameFactory _gameFactory;
        private readonly IGameBoardCache _gameBoardCache;
        private readonly IRendererFactory _rendererFactory;

        public GameHandler(IGameFactory gameFactory, IGameBoardCache gameBoardCache, IRendererFactory rendererFactory)
        {
            _gameFactory = gameFactory;
            _gameBoardCache = gameBoardCache;
            _rendererFactory = rendererFactory;
        }

        public async Task Act(Event @event)
        {
            string gameId = @event.SenderId();
            var userId = @event.UserId();
            var text = @event.Text();
            await Act(gameId, userId, text, @event.replyToken);
        }

        public async Task Act(string gameId, string userId, string cmd, string? replyToken = null)
        {
            var renderer = _rendererFactory.Get(replyToken);

            if (cmd == "小遊戲說明")
            {
                renderer.Render(string.Join("\n", new[]
                {
                    "【小遊戲指令說明】",
                    "玩猜數字 - 開始猜數字遊戲",
                    "玩井字遊戲 - 開始井字遊戲",
                    "玩五子棋 - 開始五子棋遊戲",
                    "玩象棋 - 開始象棋遊戲",
                    "我認輸了 - 放棄目前的遊戲",
                    "我的戰績 - 查看遊戲戰績",
                }));
                (renderer as BufferedReplyRenderer)?.Flush();
                return;
            }

            // 如果是建立遊戲指令，移除舊遊戲並建立新遊戲
            if (IsCreateGameCmd(cmd))
            {
                await _gameBoardCache.Remove(gameId);
                var gameType = _cmdGameTypeDict[cmd];
                var game = _gameFactory.CreateGame(gameType);
                game.TextRenderer = renderer;
                game.StartGame();
                await _gameBoardCache.Set(gameId, game.GameBoard);
            }
            else
            {
                var gameBoard = await _gameBoardCache.Get(gameId);
                if (gameBoard != null)
                {
                    var game = _gameFactory.CreateGame(gameBoard);
                    game.TextRenderer = renderer;
                    game.Act(userId, cmd);
                    if (game.GameBoard.IsGameOver() || cmd == "我認輸了")
                    {
                        game.GameOver();
                        await _gameBoardCache.Remove(gameId);
                    }
                    else
                    {
                        await _gameBoardCache.Set(gameId, game.GameBoard);
                    }
                }
            }

            (renderer as BufferedReplyRenderer)?.Flush();
        }

        private bool IsCreateGameCmd(string cmd)
        {
            return _cmdGameTypeDict.ContainsKey(cmd);
        }
    }
}
