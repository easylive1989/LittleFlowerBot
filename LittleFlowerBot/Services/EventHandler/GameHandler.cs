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
            await Act(gameId, userId, text);
        }
        
        public async Task Act(string gameId, string userId, string cmd)
        {
            var gameBoard = await _gameBoardCache.Get(gameId);
            if (gameBoard != null)
            {
                var game = _gameFactory.CreateGame(gameBoard);
                game.TextRenderer = _rendererFactory.Get(gameId);
                if (game.IsMatch(cmd))
                {
                    game.Act(userId, cmd);
                    if (game.GameBoard.IsGameOver())
                    {
                        await _gameBoardCache.Remove(gameId);
                    }
                    else
                    {
                        await _gameBoardCache.Set(gameId, game.GameBoard);
                    }
                }
                else if(cmd == "我認輸了" )
                {
                    game.GameOver();
                    await _gameBoardCache.Remove(gameId);
                }
            }
            else
            {
                if (IsCreateGameCmd(cmd))
                {
                    var gameType = _cmdGameTypeDict[cmd];
                    var game = _gameFactory.CreateGame(gameType);
                    game.TextRenderer = _rendererFactory.Get(gameId);
                    await _gameBoardCache.Set(gameId, game.GameBoard);
                    game.StartGame();
                }
            }
        }

        private bool IsCreateGameCmd(string cmd)
        {
            return _cmdGameTypeDict.ContainsKey(cmd);
        }
    }
}