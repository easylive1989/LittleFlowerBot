using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LittleFlowerBot.Models.Game;

namespace LittleFlowerBot.Models.GameResult
{
    public class BoardGameResult
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        
        public string UserId { get; set; }

        public GameType GameType { get; set; }
        
        public GameResult Result { get; set; }
        
        public DateTime GameOverTime { get; set; }
    }
}