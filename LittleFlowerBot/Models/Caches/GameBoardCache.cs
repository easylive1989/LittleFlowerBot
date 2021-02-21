using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Threading.Tasks;
using LittleFlowerBot.Models.Game;
using LittleFlowerBot.Models.Game.BoardGame;
using LittleFlowerBot.Models.Game.BoardGame.KiGames;
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
            
            var gameStateBytes = await _redisCache.GetAsync(gameId);
            if (gameStateBytes != null)
            {
                return (IGameBoard)ByteArrayToObject(gameStateBytes);
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
        
        private byte[] ObjectToByteArray(Object obj)
        {
            if(obj == null)
                return null;

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);

            return ms.ToArray();
        }

        private Object ByteArrayToObject(byte[] arrBytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            Object obj = (Object) binForm.Deserialize(memStream);

            return obj;
        }
    }
}