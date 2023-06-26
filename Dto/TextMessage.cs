namespace bot.Dto
{
    public class TextMessage : MessageBase, IReplyable
    {
        public decimal ChatId { get; set; }
        public decimal UserId { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}