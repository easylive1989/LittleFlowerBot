using System;
using System.Text.RegularExpressions;
using LittleFlowerBot.Models.GameResult;
using LittleFlowerBot.Repositories;

namespace LittleFlowerBot.Models.Game.BoardGame.KiGames.TicTacToe
{
    public class TicTacToeGame : BoardGame<Ki>
    {
        private readonly IBoardGameResultsRepository _boardGameResultsRepository;

        public TicTacToeGame(IBoardGameResultsRepository boardGameResultsRepository)
        {
            _boardGameResultsRepository = boardGameResultsRepository;
            GameBoard = new TicTacToeBoard();
        }

        protected override bool IsCmdValid(string cmd)
        {
            return new Regex(@"^[1-3],[a-c]$").IsMatch(cmd);
        }

        public override void GameOver()
        {
            base.GameOver();
            var gameOverTime = DateTime.UtcNow;
            _boardGameResultsRepository.AddGameResult(new BoardGameResult()
            {
                UserId = GetKiBoard().GetCurrentPlayer().Id,
                Result = GetBoard().IsDraw() ? GameResult.GameResult.Draw : GameResult.GameResult.Win,
                GameType = GameType.TicTacToe,
                GameOverTime = gameOverTime
            });

            _boardGameResultsRepository.AddGameResult(new BoardGameResult()
            {
                UserId = GetKiBoard().GetNextPlayer().Id,
                Result = GetBoard().IsDraw() ? GameResult.GameResult.Draw : GameResult.GameResult.Lose,
                GameType = GameType.TicTacToe,
                GameOverTime = gameOverTime
            });
        }

        private KiBoard GetKiBoard()
        {
            return (KiBoard) GameBoard;
        }
    }
}