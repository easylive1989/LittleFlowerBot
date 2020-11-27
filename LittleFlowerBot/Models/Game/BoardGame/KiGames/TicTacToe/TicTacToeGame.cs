using System;
using System.Text.RegularExpressions;
using LittleFlowerBot.Models.GameResult;
using LittleFlowerBot.Models.Renderer;
using LittleFlowerBot.Repositories;

namespace LittleFlowerBot.Models.Game.BoardGame.KiGames.TicTacToe
{
    public class TicTacToeGame : BoardGame<Ki>
    {
        private readonly IBoardGameResultsRepository _boardGameResultsRepository;

        public TicTacToeGame(ITextRenderer textRenderer, IBoardGameResultsRepository boardGameResultsRepository) : 
            base(textRenderer)
        {
            _boardGameResultsRepository = boardGameResultsRepository;
            GameBoard = new TicTacToeBoard();
        }

        public override bool IsMatch(string cmd)
        {
            if (!GetBoard().IsPlayerFully())
            {
                return cmd.Equals("++");
            }
            else
            {
                return new Regex(@"^[1-3],[a-c]$").IsMatch(cmd);
            }
        }

        public override void GameOver()
        {
            base.GameOver();
            var gameOverTime = DateTime.Now;
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