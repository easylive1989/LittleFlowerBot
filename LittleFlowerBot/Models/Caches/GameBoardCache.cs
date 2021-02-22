using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using LittleFlowerBot.Models.Game;
using LittleFlowerBot.Models.Game.BoardGame;
using LittleFlowerBot.Models.Game.BoardGame.ChessGames.ChineseChess;
using LittleFlowerBot.Models.Game.BoardGame.KiGames;
using LittleFlowerBot.Models.Game.BoardGame.KiGames.Gomoku;
using LittleFlowerBot.Models.Game.BoardGame.KiGames.TicTacToe;
using LittleFlowerBot.Models.Game.GuessNumber;
using LittleFlowerBot.Utils;
using Microsoft.Extensions.Caching.Distributed;

namespace LittleFlowerBot.Models.Caches
{
    public class GameBoardCache : IGameBoardCache
    {
        private readonly IDistributedCache _redisCache;
        private readonly Dictionary<string, IGameBoard> _gameStateCache = new Dictionary<string, IGameBoard>();
        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions();

        public GameBoardCache(IDistributedCache redisCache)
        {
            _redisCache = redisCache;
            _jsonSerializerOptions.Converters.Add(new DictionaryJsonConverter<Ki, Player>());
        }

        public async Task Set(string gameId, IGameBoard gameBoard)
        {
            var gameBoardJson = JsonSerializer.Serialize(gameBoard, _jsonSerializerOptions);
            await _redisCache.SetStringAsync($"{gameId}:type", gameBoard.GetType().ToString());
            await _redisCache.SetStringAsync($"{gameId}:state", gameBoardJson);
            _gameStateCache[gameId] = gameBoard;
        }

        public async Task<IGameBoard> Get(string gameId)
        {
            if (_gameStateCache.ContainsKey(gameId))
            {
                return _gameStateCache[gameId];
            }
            
            var gameTypeString = await _redisCache.GetStringAsync($"{gameId}:type");
            var gameStateString = await _redisCache.GetStringAsync($"{gameId}:state");
            if (gameTypeString != null && gameStateString != null)
            {
                var gameType = Enum.Parse<GameType>(gameTypeString);
                switch (gameType)
                {
                    case GameType.GuessNumber:
                        return JsonSerializer.Deserialize<GuessNumberBoard>(gameStateString, _jsonSerializerOptions);
                    case GameType.TicTacToe:
                        return JsonSerializer.Deserialize<TicTacToeBoard>(gameStateString, _jsonSerializerOptions);
                    case GameType.Gomoku:
                        return JsonSerializer.Deserialize<GomokuBoard>(gameStateString, _jsonSerializerOptions);
                    case GameType.ChineseChess:
                        return JsonSerializer.Deserialize<ChineseChessBoard>(gameStateString, _jsonSerializerOptions);
                }
            }

            return null;
        }

        public async Task Remove(string gameId)
        {
            _gameStateCache.Remove(gameId);
            await _redisCache.RemoveAsync(gameId);
        }

        public List<string> GetGameIdList()
        {
            return _gameStateCache.Keys.ToList();
        }
    }
}