using System;
using System.Collections.Generic;
using System.Linq;
using LittleFlowerBot.Models.GameExceptions;

namespace LittleFlowerBot.Models.Game.BoardGame
{
    [Serializable]
    public abstract class GameBoard<T> : IGameBoard where T : Enum
    {
        protected GameBoard(int row, int column)
        {
            Row = row;
            Column = column;
            InitKiPlayerMap();
            InitGameBoard();
        }

        public int Row { get; set; }
        public int Column { get; set; }
        public T[][] GameBoardArray { get; set; }

        public Dictionary<T, Player> PlayerMap { get; set; } = new Dictionary<T, Player>();
        public List<Player> PlayerMoveOrder { get; set; } = new List<Player>();

        protected T this[int i, int j]
        {
            get => GameBoardArray[i][j];
            set => GameBoardArray[i][j] = value;
        }

        public bool IsGameOver()
        {
            return IsSomeoneWin() || IsDraw();
        }

        public abstract string GetBoardString();

        public abstract byte[] GetBoardImage();

        public abstract bool IsDraw();

        public bool IsPlayerFully()
        {
            return PlayerMap.All(x => x.Value != null);
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

        public abstract void Move(Player player, string cmd);

        protected abstract bool IsSomeoneWin();

        protected abstract void JoinImpl(Player player);

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

        private void InitKiPlayerMap()
        {
            foreach (var value in Enum.GetValues(typeof(T)))
            {
                if (!value.Equals(default(T)))
                {
                    PlayerMap.Add((T) value, null);
                }
            }
        }

        private bool IsPlayerJoin(Player player)
        {
            return PlayerMap.ContainsValue(player);
        }
    }
}