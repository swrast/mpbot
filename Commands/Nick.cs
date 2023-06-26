namespace bot.Commands
{
    public class NickCommand : CommandBase
    {
        private ApplicationContext _context { get; }

        public NickCommand(ApplicationContext context)
        {
            _context = context;
        }

        [HandleMessage("ник")]
        [ExcludeFrontend(Frontends.Frontends.Discord)]
        public async Task Nick()
        {
            string nick = string.Join(" ", _args);

            if (nick == "")
            {
                await Reply("ваш ник: " + await _api.GetNick(_textMessage!.UserId));

                return;
            }

            _dbUser.Nickname = nick;
            await _context.SaveChangesAsync();

            await Reply("успешно");
        }
    }
}