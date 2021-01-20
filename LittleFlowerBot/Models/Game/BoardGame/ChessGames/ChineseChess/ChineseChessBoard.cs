using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LittleFlowerBot.Extensions;
using LittleFlowerBot.Models.Game.BoardGame.ChessGames.ChineseChess.StepRule;
using LittleFlowerBot.Models.GameExceptions;

namespace LittleFlowerBot.Models.Game.BoardGame.ChessGames.ChineseChess
{
    [Serializable]
    public class ChineseChessBoard : GameBoard<ChineseChess>
    {
        

        private int _turn;
        
        public ChineseChessBoard() : base(10, 9)
        {
            GameBoardArray[0][0] = ChineseChess.Rook;
            GameBoardArray[0][1] = ChineseChess.Horse;
            GameBoardArray[0][2] = ChineseChess.Elephant;
            GameBoardArray[0][3] = ChineseChess.Guard;
            GameBoardArray[0][4] = ChineseChess.General;
            GameBoardArray[0][5] = ChineseChess.Guard;
            GameBoardArray[0][6] = ChineseChess.Elephant;
            GameBoardArray[0][7] = ChineseChess.Horse;
            GameBoardArray[0][8] = ChineseChess.Rook;
            GameBoardArray[2][1] = ChineseChess.Cannon;
            GameBoardArray[2][7] = ChineseChess.Cannon;
            GameBoardArray[3][0] = ChineseChess.Soldier;
            GameBoardArray[3][2] = ChineseChess.Soldier;
            GameBoardArray[3][4] = ChineseChess.Soldier;
            GameBoardArray[3][6] = ChineseChess.Soldier;
            GameBoardArray[3][8] = ChineseChess.Soldier;

            GameBoardArray[9][0] = ChineseChess.Chariot;
            GameBoardArray[9][1] = ChineseChess.Knight;
            GameBoardArray[9][2] = ChineseChess.Minister;
            GameBoardArray[9][3] = ChineseChess.Adviser;
            GameBoardArray[9][4] = ChineseChess.King;
            GameBoardArray[9][5] = ChineseChess.Adviser;
            GameBoardArray[9][6] = ChineseChess.Minister;
            GameBoardArray[9][7] = ChineseChess.Knight;
            GameBoardArray[9][8] = ChineseChess.Chariot;
            GameBoardArray[7][1] = ChineseChess.RedCannon;
            GameBoardArray[7][7] = ChineseChess.RedCannon;
            GameBoardArray[6][0] = ChineseChess.Pawn;
            GameBoardArray[6][2] = ChineseChess.Pawn;
            GameBoardArray[6][4] = ChineseChess.Pawn;
            GameBoardArray[6][6] = ChineseChess.Pawn;
            GameBoardArray[6][8] = ChineseChess.Pawn;
        }

        public override bool IsDraw()
        {
            return false;
        }

        protected override bool IsSomeoneWin()
        {
            return !Contains(ChineseChess.General) || !Contains(ChineseChess.King);
        }

        protected override void JoinImpl(Player player)
        {
            var isFirstPlayJoin = PlayerMap.Values.All(x => x == null);
            var targetChessSet = isFirstPlayJoin ? ChineseChessExtensions.BlackChessGroup : ChineseChessExtensions.RedChessGroup;
            foreach (var chess in targetChessSet)
            {
                PlayerMap[chess] = player;
            }
        }

        public override void Move(Player player, string cmd)
        {
            if (!IsYourTurn(player))
            {
                throw new NotYourTurnException();
            }

            var step = GetStep(cmd);

            if (!IsYourChess(player, step.FromX, step.FromY))
            {
                throw new NotYourChessException();
            }
            
            if (!IsMoveMatchRule(step))
            {
                throw new MoveInvalidException();
            }

            if (!IsMoveValid(step))
            {
                throw new CoordinateValidException();
            }
                
            MoveChess(step);

            _turn++;
        }

        private bool IsMoveValid(Step step)
        {
            var fromChess = this[step.FromX, step.FromY];
            var toChess = this[step.ToX, step.ToY];
            if (fromChess.GetChessGroup().Contains(toChess))
            {
                return false;
            }

            return true;
        }

        private bool IsMoveMatchRule(Step step)
        {
            var chess = this[step.FromX, step.FromY];
            var chessMoveRule = GetChessMoveRule();
            if (chessMoveRule.ContainsKey(chess))
            {
                return chessMoveRule[chess].IsMatch(GameBoardArray, step);
            }

            return true;
        }

        private bool IsYourChess(Player player, int x, int y)
        {
            var chess = this[x, y];
            if (PlayerMap.ContainsKey(chess))
            {
                return PlayerMap[chess].Equals(player);
            }

            return false;
        }

        private bool IsYourTurn(Player player)
        {
            var idx = _turn % PlayerMoveOrder.Count;
            return PlayerMoveOrder[idx].Equals(player);
        }
        
        private void MoveChess(Step step)
        {
            var tmpChess = this[step.FromX, step.FromY];
            this[step.ToX, step.ToY] = tmpChess;
            this[step.FromX, step.FromY] = ChineseChess.Empty;
        }

        private Step GetStep(string cmd)
        {
            string[] move = cmd.Split('>');
            string[] fromCoordinate = move[0].Split(',');
            string[] toCoordinate = move[1].Split(',');
            return new Step()
            {
                FromX = Int16.Parse(fromCoordinate[0]) - 1,
                FromY = fromCoordinate[1].ToLower()[0] - 97,
                ToX = Int16.Parse(toCoordinate[0]) - 1,
                ToY = toCoordinate[1].ToLower()[0] - 97
            };
        }

        public override string GetBoardString()
        {
            var builder = new StringBuilder();
            builder.Append("00ⒶⒷⒸⒹⒺⒻⒼⒽⒾ\n");
            for (int i = 0; i < Row; i++)
            {
                builder.Append($"{i + 1:D2}");
                for (int j = 0; j < Column; j++)
                {
                    var chess = GameBoardArray[i][j];
                    var displayDict = GetDisplayDict();
                    if (displayDict.ContainsKey(chess))
                    {
                        builder.Append(displayDict[chess]);
                    }
                    else
                    {
                        if (i == 4)
                        {
                            builder.Append("┴");
                        } 
                        else if (i == 5)
                        {
                            builder.Append("┬");
                        } 
                        else
                        {
                            builder.Append("┼");
                        }
                    }
                }
                builder.Append("\n");
            }
            return builder.ToString();
        }
        
        private bool Contains(ChineseChess chess)
        {
            for (int i = 0; i < Row; i++)
            {
                for (int j = 0; j < Column; j++)
                {
                    if (GameBoardArray[i][j] == chess)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        
        private static Dictionary<ChineseChess, string> GetDisplayDict()
        {
            return new Dictionary<ChineseChess, string>()
            {
                {ChineseChess.King, "帥"},
                {ChineseChess.General, "將"},
                {ChineseChess.Adviser, "仕"},
                {ChineseChess.Guard, "士"},
                {ChineseChess.Minister, "相"},
                {ChineseChess.Elephant, "象"},
                {ChineseChess.Chariot, "俥"},
                {ChineseChess.Rook, "車"},
                {ChineseChess.Knight, "傌"},
                {ChineseChess.Horse, "馬"},
                {ChineseChess.RedCannon, "炮"},
                {ChineseChess.Cannon, "包"},
                {ChineseChess.Pawn, "兵"},
                {ChineseChess.Soldier, "卒"},
            };
        }

        private static Dictionary<ChineseChess, IStepRule> GetChessMoveRule()
        {
            return new Dictionary<ChineseChess, IStepRule>()
            {
                {ChineseChess.Pawn, new PawnStepRule()},
                {ChineseChess.Soldier, new PawnStepRule()},
                {ChineseChess.Cannon, new CannonStepRule()},
                {ChineseChess.RedCannon, new CannonStepRule()},
                {ChineseChess.Horse, new HorseStepRule()},
                {ChineseChess.Knight, new HorseStepRule()},
                {ChineseChess.Rook, new RookStepRule()},
                {ChineseChess.Chariot, new RookStepRule()},
                {ChineseChess.Elephant, new ElephantStepRule()},
                {ChineseChess.Minister, new ElephantStepRule()},
                {ChineseChess.Guard, new GuardStepRule()},
                {ChineseChess.Adviser, new GuardStepRule()},
                {ChineseChess.General, new GeneralStepRule()},
                {ChineseChess.King, new GeneralStepRule()},
            };
        }

    }
}