﻿using LittleFlowerBot.Models.Game.GuessNumber;
using System;
using System.Collections.Generic;
using LittleFlowerBot.Models.Game.BoardGame.ChessGames.ChineseChess;
using LittleFlowerBot.Models.Game.BoardGame.KiGames.Gomoku;
using LittleFlowerBot.Models.Game.BoardGame.KiGames.TicTacToe;

namespace LittleFlowerBot.Models.Game
{
    public class GameFactory : IGameFactory
    {
        private readonly IServiceProvider _serviceProvider;

        private readonly Dictionary<Type, Type> _gameList = new Dictionary<Type, Type>()
        {
            {typeof(GuessNumberBoard), typeof(GuessNumberGame)},
            {typeof(TicTacToeBoard), typeof(TicTacToeGame)},
            {typeof(GomokuBoard), typeof(GomokuGame)},
            {typeof(ChineseChessBoard), typeof(ChineseChessGame)},
        };

        public GameFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Game CreateGame(Type type)
        {
            return _serviceProvider.GetService(_gameList[type]) as Game;
        }

        public Game CreateGame(IGameBoard gameBoard)
        {
            var game = _serviceProvider.GetService(gameBoard.GetType()) as Game;
            if (game != null)
            {
                game.GameBoard = gameBoard;
            }
            return game;
        }
    }
}