namespace bot.Dto
{
    public class MessageBase
    {
        public object Origin { get; set; } = new object();

        public Frontends.Frontends From { get; set; }
    }
}