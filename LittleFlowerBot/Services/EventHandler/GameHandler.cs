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

namespace LittleFlowerBot.Services.EventHandler
{
    public class GameHandler : IEventHandler
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

        public GameHandler(IGameFactory gameFactory, IGameBoardCache gameBoardCache)
        {
            _gameFactory = gameFactory;
            _gameBoardCache = gameBoardCache;
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
            var gameState = await _gameBoardCache.Get(gameId);
            if (gameState != null)
            {
                var game = _gameFactory.CreateGame(gameState.GetType());
                game.GameBoard = gameState;
                game.SenderId = gameId;
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
                    game.SenderId = gameId;
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