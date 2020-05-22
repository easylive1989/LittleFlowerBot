using System;
using System.Collections.Generic;
using System.Linq;
using LittleFlowerBot.Models.GameExceptions;

namespace LittleFlowerBot.Models.Game.BoardGame
{
    [Serializable]
    public abstract class GameBoard<T> : IGameState where T : Enum
    {
        protected readonly int Row;
        protected readonly int Column;
        protected T[][] GameBoardArray;

        protected readonly Dictionary<T, Player> KiPlayerMap = new Dictionary<T, Player>();
        protected readonly List<Player> PlayerMoveOrder = new List<Player>();

        protected GameBoard(int row, int column)
        {
            Row = row;
            Column = column;
            InitKiPlayerMap();
            InitGameBoard();
        }

        private void InitKiPlayerMap()
        {
            foreach (var value in Enum.GetValues(typeof(T)))
            {
                if (!value.Equals(default(T)))
                {
                    KiPlayerMap.Add((T) value, null);
                }
            }
        }
        
        public void Join(Player player)
        {
            if (IsPlayerJoin(player))
            {
                throw new PlayerExistException();
            }
            
            JoinImpl(player);            
            
            PlayerMoveOrder.Add(player);
        }

        private bool IsPlayerJoin(Player player)
        {
            return KiPlayerMap.ContainsValue(player);
        }

        public bool IsTwoPlayers()
        {
            return KiPlayerMap.All(x => x.Value != null);
        }

        protected T this[int i, int j]
        {
            get => GameBoardArray[i][j];
            set => GameBoardArray[i][j] = value;
        }
        
        public bool IsGameOver()
        {
            return IsSomeoneWin() || IsDraw();
        }

        protected abstract void JoinImpl(Player player);

        public abstract void Move(Player player, string cmd);
        
        public abstract string GetBoardString();

        public abstract bool IsDraw();

        protected abstract bool IsSomeoneWin();

        private void InitGameBoard()
        {
            GameBoardArray = new T[Row][];
            for (int i = 0; i < Row; i++)
            {
                GameBoardArray[i] = new T[Column];
                for (int j = 0; j < Column; j++)
                {
                    GameBoardArray[i][j] = default;
                }
            }
        }
    }
}