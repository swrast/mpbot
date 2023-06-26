namespace bot.Dto
{
    public class ChatJoin : IReplyable
    {
        public decimal ChatId { get; set; }
        public decimal UserId { get; set; }
    }
}