using System;
using System.Collections.Generic;

namespace LittleFlowerBot.Models.Game.Battleship
{
    [Serializable]
    public class PlayerState
    {
        public CellState[][] OwnGrid { get; set; }
        public CellState[][] AttackGrid { get; set; }
        public List<Ship> Ships { get; set; } = new();
        public bool IsSetupComplete { get; set; }

        public PlayerState()
        {
            OwnGrid = InitGrid(CellState.Empty);
            AttackGrid = InitGrid(CellState.Unknown);
        }

        private static CellState[][] InitGrid(CellState defaultValue)
        {
            var grid = new CellState[10][];
            for (int i = 0; i < 10; i++)
            {
                grid[i] = new CellState[10];
                for (int j = 0; j < 10; j++)
                {
                    grid[i][j] = defaultValue;
                }
            }
            return grid;
        }
    }
}
