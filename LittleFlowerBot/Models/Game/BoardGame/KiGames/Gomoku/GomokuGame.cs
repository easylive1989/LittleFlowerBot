using System;
using System.Text.RegularExpressions;
using LittleFlowerBot.Models.GameResult;
using LittleFlowerBot.Repositories;

namespace LittleFlowerBot.Models.Game.BoardGame.KiGames.Gomoku
{
    public class GomokuGame : BoardGame<Ki>
    {
        private readonly IBoardGameResultsRepository _boardGameResultsRepository;

        public GomokuGame(IBoardGameResultsRepository boardGameResultsRepository)
        {
            _boardGameResultsRepository = boardGameResultsRepository;
            GameBoard = new GomokuBoard();
        }

        protected override bool IsCmdValid(string cmd)
        {
            return new Regex(@"^([1-9]|1[0-5]),[a-o]$").IsMatch(cmd.ToLower());
        }
        
        public override void GameOver()
        {
            base.GameOver();
            var gameOverTime = DateTime.Now;
            _boardGameResultsRepository.AddGameResult(new BoardGameResult()
            {
                UserId = GetKiBoard().GetCurrentPlayer().Id,
                Result = GetBoard().IsDraw() ? GameResult.GameResult.Draw : GameResult.GameResult.Win,
                GameType = GameType.Gomoku,
                GameOverTime = gameOverTime
            });

            _boardGameResultsRepository.AddGameResult(new BoardGameResult()
            {
                UserId = GetKiBoard().GetNextPlayer().Id,
                Result = GetBoard().IsDraw() ? GameResult.GameResult.Draw : GameResult.GameResult.Lose,
                GameType = GameType.Gomoku,
                GameOverTime = gameOverTime
            });
        }

        private KiBoard GetKiBoard()
        {
            return (KiBoard) GameBoard;
        }
    }
}