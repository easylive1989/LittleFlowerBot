using System;
using System.Text;

namespace LittleFlowerBot.Models.Game.BoardGame.KiGames.TicTacToe
{
    [Serializable]
    public class TicTacToeBoard : KiBoard
    {
        public TicTacToeBoard() : base(3, 3)
        {
        }

        public override bool IsDraw()
        {
            return GetTurn() >= 9;
        }

        protected override bool IsSomeoneWin()
        {
            var gameBoard = this;
            for (int i = 0; i < 3; i++)
            {
                if (gameBoard[i, 0] == gameBoard[i, 1] &&
                    gameBoard[i, 1] == gameBoard[i, 2] &&
                    gameBoard[i, 0] != Ki.Empty)
                {
                    return true;
                }

                if (gameBoard[0, i] == gameBoard[1, i] &&
                    gameBoard[1, i] == gameBoard[2, i] &&
                    gameBoard[0, i] != Ki.Empty)
                {
                    return true;
                }
            }

            if (gameBoard[0, 0] == gameBoard[1, 1] &&
                gameBoard[1, 1] == gameBoard[2, 2] &&
                gameBoard[0, 0] != Ki.Empty)
            {                    
                return true;
            }

            if (gameBoard[0, 2] == gameBoard[1, 1] &&
                gameBoard[1, 1] == gameBoard[2, 0] &&
                gameBoard[0, 2] != Ki.Empty)
            {                    
                return true;
            }

            return false;
        }
        
        public override string GetBoardString()
        {
            var builder = new StringBuilder();
            builder.Append("00ⒶⒷⒸ\n");
            for (int i = 0; i < Row; i++)
            {
                builder.Append($"{i + 1:D2}");
                for (int j = 0; j < Column; j++)
                {
                    switch (GameBoardArray[i][j])
                    {
                        case Ki.Circle:
                            builder.Append("●");
                            break;

                        case Ki.Cross:
                            builder.Append("○");
                            break;

                        case Ki.Empty:
                            builder.Append("＿");
                            break;
                    }
                }
                builder.Append("\n");
            }
            return builder.ToString();
        }
    }
}