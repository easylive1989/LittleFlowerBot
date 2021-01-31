using System;

namespace LittleFlowerBot.Models.Game
{
    public interface IGameFactory
    {
        Game CreateGame(Type type);

        Game CreateGame(IGameBoard gameBoard);
    }
}