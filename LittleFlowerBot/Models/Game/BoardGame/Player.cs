using System;

namespace LittleFlowerBot.Models.Game.BoardGame
{
    [Serializable]
    public class Player
    {
        public string Id { get; set; }

        public Player(string id)
        {
            Id = id;
        }
        
        public override bool Equals(object? obj)
        {
            var id = obj as Player;

            if (id == null)
            {
                return false;
            }

            return Id.Equals(id.Id);
        }
        
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}