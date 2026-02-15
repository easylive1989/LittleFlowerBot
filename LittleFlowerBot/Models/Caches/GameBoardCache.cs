using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using LittleFlowerBot.DbContexts;
using LittleFlowerBot.Models.Game;
using LittleFlowerBot.Models.Game.BoardGame;
using LittleFlowerBot.Models.Game.BoardGame.ChessGames.ChineseChess;
using LittleFlowerBot.Models.Game.BoardGame.KiGames;
using LittleFlowerBot.Models.Game.BoardGame.KiGames.Gomoku;
using LittleFlowerBot.Models.Game.BoardGame.KiGames.TicTacToe;
using LittleFlowerBot.Models.Game.GuessNumber;
using LittleFlowerBot.Utils;
using MongoDB.Driver;

namespace LittleFlowerBot.Models.Caches
{
    public class GameBoardCache : IGameBoardCache
    {
        private readonly MongoDbContext _context;
        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions();

        private Dictionary<Type, GameType> _gameTypesMap = new Dictionary<Type, GameType>()
        {
            {typeof(TicTacToeBoard), GameType.TicTacToe},
            {typeof(GomokuBoard), GameType.Gomoku},
            {typeof(ChineseChessBoard), GameType.ChineseChess},
            {typeof(GuessNumberBoard), GameType.GuessNumber},
        };

        public GameBoardCache(MongoDbContext context)
        {
            _context = context;
            _jsonSerializerOptions.Converters.Add(new DictionaryJsonConverter<Ki, Player>());
        }

        public async Task Set(string gameId, IGameBoard gameBoard)
        {
            var gameBoardJson = JsonSerializer.Serialize(gameBoard, gameBoard.GetType(), _jsonSerializerOptions);

            var document = new GameStateDocument
            {
                Id = gameId,
                GameType = (int)_gameTypesMap[gameBoard.GetType()],
                State = gameBoardJson
            };

            await _context.GameStates.ReplaceOneAsync(
                Builders<GameStateDocument>.Filter.Eq(d => d.Id, gameId),
                document,
                new ReplaceOptions { IsUpsert = true });
        }

        public async Task<IGameBoard> Get(string gameId)
        {
            var filter = Builders<GameStateDocument>.Filter.Eq(d => d.Id, gameId);
            using var cursor = await _context.GameStates.FindAsync(filter);
            var document = await cursor.FirstOrDefaultAsync();

            if (document != null)
            {
                var gameType = (GameType)document.GameType;
                switch (gameType)
                {
                    case GameType.GuessNumber:
                        return JsonSerializer.Deserialize<GuessNumberBoard>(document.State, _jsonSerializerOptions);
                    case GameType.TicTacToe:
                        return JsonSerializer.Deserialize<TicTacToeBoard>(document.State, _jsonSerializerOptions);
                    case GameType.Gomoku:
                        return JsonSerializer.Deserialize<GomokuBoard>(document.State, _jsonSerializerOptions);
                    case GameType.ChineseChess:
                        return JsonSerializer.Deserialize<ChineseChessBoard>(document.State, _jsonSerializerOptions);
                }
            }

            return null;
        }

        public async Task Remove(string gameId)
        {
            await _context.GameStates.DeleteOneAsync(
                Builders<GameStateDocument>.Filter.Eq(d => d.Id, gameId));
        }

        public List<string> GetGameIdList()
        {
            var filter = Builders<GameStateDocument>.Filter.Empty;
            using var cursor = _context.GameStates.FindSync(filter);
            return cursor.ToList().Select(d => d.Id).ToList();
        }
    }
}
