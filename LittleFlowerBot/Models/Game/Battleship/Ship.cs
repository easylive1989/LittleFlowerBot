using System;
using System.Collections.Generic;
using System.Linq;

namespace LittleFlowerBot.Models.Game.Battleship
{
    [Serializable]
    public class Ship
    {
        public ShipType Type { get; set; }
        public List<Coordinate> Coordinates { get; set; } = new();
        public bool IsSunk => Coordinates.Count > 0 && Coordinates.All(c => c.IsHit);
    }

    [Serializable]
    public class Coordinate
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public bool IsHit { get; set; }
    }
}
