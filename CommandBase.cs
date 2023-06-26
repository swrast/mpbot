using bot.ApiProviders;
using bot.Dto;
using bot.Services;
using Telegram.Bot.Types;

namespace bot.Commands
{
    public abstract class CommandBase
    {
        protected TextMessage? _textMessage { get; set; } = new TextMessage();
        protected IApiProvider _api { get; set; } = null!;

        protected IEnumerable<string> _args = new string[] { };
        protected Models.User _dbUser { get; set; } = null!;

        public Task Setup() => Task.CompletedTask;

        public void SetupBase<T>(T request, IApiProvider api, Models.User user, IEnumerable<string> args = null!)
        {
            _api = api;
            _dbUser = user;

            _textMessage = request as TextMessage;

            if (args != null)
            {
                _args = args;
            }
        }

        protected async Task Reply(string text)
        {
            await _api.Reply(await _api.GetMention(_textMessage!.UserId) + ", " + text);
        }
    }

    public class HandleMessageAttribute : Attribute
    {
        public string[] Aliases { get; }

        public HandleMessageAttribute(params string[] aliases)
        {
            Aliases = aliases;
        }
    }

    public class ExcludeFrontendAttribute : Attribute
    {
        public Frontends.Frontends[] Blacklist;

        public ExcludeFrontendAttribute(params Frontends.Frontends[] blacklist)
        {
            Blacklist = blacklist;
        }
    }
}