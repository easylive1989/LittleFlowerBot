namespace LittleFlowerBot.Models.Game.BoardGame.ChessGames.ChineseChess.StepRule
{
    public interface IStepRule
    {
        bool IsMatch(ChineseChess[][] board, Step step);
    }
}