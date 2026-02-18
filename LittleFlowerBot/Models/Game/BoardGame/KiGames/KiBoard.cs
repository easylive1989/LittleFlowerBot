using System;
using System.Linq;
using LittleFlowerBot.Models.GameExceptions;

namespace LittleFlowerBot.Models.Game.BoardGame.KiGames
{
    [Serializable]
    public abstract class KiBoard : GameBoard<Ki>
    {
        public int CurMoveX { get; set; }
        public int CurMoveY { get; set; }

        public Player CurPlayer { get; set; }
        
        protected KiBoard(int row, int column) : base(row, column)
        {
        }

        public override void Move(Player player, string cmd)
        {
            CurPlayer = player;
            
            GetCoordinate(cmd, out var x, out var y);
            
            Move(player, x, y);
        }

        protected override void JoinImpl(Player player)
        {
            foreach (var key in PlayerMap.Keys)
            {
                if (PlayerMap[key] == null)
                {
                    PlayerMap[key] = player;
                    break;
                }
            }
        }
        
        public Player GetCurrentPlayer()
        {
            return CurPlayer;
        }

        public Player GetNextPlayer()
        {
            return PlayerMoveOrder.First(x => !x.Equals(CurPlayer));
        }

        private void GetCoordinate(string cmd, out int x, out int y)
        {
            int i = 0;
            while (i < cmd.Length && char.IsDigit(cmd[i])) i++;
            x = Int16.Parse(cmd.Substring(0, i)) - 1;
            y = char.ToLower(cmd[i]) - 97;
        }
        
        private void Move(Player player, int x, int y)
        {
            if (!IsYourTurn(player))
            {
                throw new NotYourTurnException();
            }

            if (!IsCoordinateValid(x, y))
            {
                throw new CoordinateValidException();
            }

            CurMoveX = x;
            CurMoveY = y;
            this[x, y] = GetCurrentKi();
        }
        
        private bool IsYourTurn(Player player)
        {
            var idx = GetTurn() % PlayerMoveOrder.Count;
            return PlayerMoveOrder[idx].Equals(player);
        }
        
        private bool IsCoordinateValid(int x, int y)
        {
            return this[x, y].Equals(default(Ki));
        }
        
        private Ki GetCurrentKi()
        {
            var idx = GetTurn() % PlayerMap.Count;
            var keyValuePair = PlayerMap.ElementAt(idx);
            return keyValuePair.Key;
        }
        
        protected int GetTurn()
        {
            int turn = 0;
            for (int i = 0; i < Row; i++)
            {
                for (int j = 0; j < Column; j++)
                {
                    if (!GameBoardArray[i][j].Equals(default(Ki)))
                    {
                        turn++;
                    }
                }
            }

            return turn;
        }
    }
}