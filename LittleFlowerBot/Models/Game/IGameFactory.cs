using System;

namespace LittleFlowerBot.Models.Game
{
    public interface IGameFactory
    {
        Game CreateGame(Type type);
    }
}