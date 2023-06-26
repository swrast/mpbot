namespace bot.Commands
{
    public class PingCommand : CommandBase
    {
        [HandleMessage("пинг")]
        public async Task Ping()
        {
            await Reply("понг");
        }
    }
}