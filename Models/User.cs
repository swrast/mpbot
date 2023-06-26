namespace bot.Models
{
    public class User : Auditable
    {
        public int Id { get; set; }

        public string Secret { get; set; } = string.Empty;
        public string Nickname { get; set; } = string.Empty;

        public decimal VkId { get; set; }
        public decimal DiscordId { get; set; }
        public decimal TelegramId { get; set; }

        // public ulong Dota2Id { get; set; } 
        // etc...
    }
}