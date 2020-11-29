using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using LittleFlowerBot.Models.Game;
using Microsoft.Extensions.Caching.Distributed;

namespace LittleFlowerBot.Models.Caches
{
    public class GameBoardCache : IGameBoardCache
    {
        private readonly IDistributedCache _redisCache;
        private readonly Dictionary<string, IGameBoard> _gameStateCache = new Dictionary<string, IGameBoard>();

        public GameBoardCache(IDistributedCache redisCache)
        {
            _redisCache = redisCache;
        }

        public async Task Set(string gameId, IGameBoard gameBoard)
        {
            byte[] gameStateBytes = ObjectToByteArray(gameBoard);
            await _redisCache.SetAsync(gameId, gameStateBytes);
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