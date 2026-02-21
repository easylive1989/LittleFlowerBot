using System;
using System.Collections.Generic;

namespace LittleFlowerBot.Models.Game.Battleship
{
    [Serializable]
    public class BattleshipBoard : IGameBoard
    {
        public BattleshipPhase Phase { get; set; } = BattleshipPhase.WaitingPlayers;
        public List<string> Players { get; set; } = new();
        public Dictionary<string, PlayerState> PlayerStates { get; set; } = new();
        public int CurrentTurnIndex { get; set; }
        public string? WinnerId { get; set; }

        public bool IsGameOver()
        {
            return Phase == BattleshipPhase.GameOver;
        }

        public bool IsPlayerFull()
        {
            return Players.Count >= 2;
        }

        public void Join(string userId)
        {
            if (Players.Contains(userId) || IsPlayerFull())
                return;

            Players.Add(userId);

            if (IsPlayerFull())
            {
                Phase = BattleshipPhase.Setup;
                foreach (var playerId in Players)
                {
                    PlayerStates[playerId] = new PlayerState();
                }
            }
        }
    }
}
