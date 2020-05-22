using System;
using System.Text.RegularExpressions;
using LittleFlowerBot.Models.GameResult;
using LittleFlowerBot.Models.Renderer;
using LittleFlowerBot.Repositories;

namespace LittleFlowerBot.Models.Game.BoardGame.KiGames.Gomoku
{
    public class GomokuGame : BoardGame<Ki>
    {
        private readonly IBoardGameResultsRepository _boardGameResultsRepository;

        public GomokuGame(ITextRenderer textRenderer, IBoardGameResultsRepository boardGameResultsRepository) : 
            base(textRenderer)
        {
            _boardGameResultsRepository = boardGameResultsRepository;
            GameState = new GomokuBoard();
        }
        
        public override bool IsMatch(string cmd)
        {
            if (!GetBoard().IsTwoPlayers())
            {
                return cmd.Equals("++");
            }
            else
            {
                return new Regex(@"^([1-9]|1[0-5]),[a-o]$").IsMatch(cmd.ToLower());
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
            return (KiBoard) GameState;
        }
    }
}