using System.ComponentModel.DataAnnotations;

namespace LittleFlowerBot.Models.Subscribe
{
    public class Subscription
    {
        [Required]
        [Key]
        public string Sender { get; set; }
        [Required]
        public string Receiver { get; set; }
    }
}