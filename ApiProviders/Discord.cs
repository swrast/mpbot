using bot.Dto;
using bot.Exceptions;
using bot.Models;
using bot.Services;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace bot.ApiProviders
{
    public class DiscordApi : IApiProvider
    {
        public DiscordClient Api { get; }

        private MessageBase? _base { get; }
        private IReplyable? _message { get; }
        private User _user { get; }

        public DiscordApi(ApiAccessorService apiAccessor, object request, User user)
        {
            Api = apiAccessor.Discord;
            _message = request as IReplyable;
            _base = request as MessageBase;
            _user = user;

            if (_message == null)
            {
                throw new NullRequestException();
            }
        }

        public async Task Reply(string text)
        {
            await new DiscordMessageBuilder()
                .WithContent(text)
                .WithAllowedMentions(Mentions.None)
                .SendAsync((_base!.Origin as MessageCreateEventArgs)!.Channel);
        }

        public Task<string> GetNick(decimal id) => throw new NotImplementedException();

        public async Task<string> GetMention(decimal id) => await Task.FromResult<string>($"<@{id}>");

        public async Task<IEnumerable<string>> GetPictures()
            => await Task.FromResult((_base!.Origin as MessageCreateEventArgs)!.Message.Attachments
            .Where((x) => new List<string> { "image/png", "image/jpeg" }.Contains(x.MediaType))
            .Select((x) => x.Url));
    }
}