using bot.Dto;
using bot.Exceptions;
using bot.Services;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace bot.ApiProviders
{
    public class TelegramApi : IApiProvider
    {
        public TelegramBotClient Api { get; }

        private IReplyable? _message;
        private MessageBase? _base;
        private ApplicationContext _context;
        private Models.User _user { get; }

        public TelegramApi(ApiAccessorService apiAccessor, ApplicationContext context, object request, Models.User user)
        {
            Api = apiAccessor.Telegram;
            _message = request as IReplyable;
            _base = request as MessageBase;
            _context = context;
            _user = user;

            if (_message == null)
            {
                throw new NullRequestException();
            }
        }

        public async Task Reply(string text)
        {
            await Api.SendTextMessageAsync(
                chatId: new ChatId((long)_message!.ChatId),
                parseMode: ParseMode.MarkdownV2,
                text: text,
                disableNotification: true
            );
        }

        public async Task<string> GetNick(decimal id)
        {
            string nick = "Неизвестно";

            if (_user.Nickname == "")
            {
                var u = await Api.GetChatMemberAsync((long)_message!.ChatId, (long)_message.UserId);

                nick = u.User.FirstName + " " + u.User.LastName;

                _user.Nickname = nick;

                await _context.SaveChangesAsync();
            }
            else
            {
                nick = _user.Nickname;
            }

            return nick.Trim();
        }

        public async Task<string> GetMention(decimal id) => $"[{await GetNick(id)}](tg://user?id={id})";

        public async Task<IEnumerable<string>> GetPictures()
        {
            var photo = (_base!.Origin as Message)!.Photo;
            if (photo == null)
            {
                return new string[] { };
            }

            return await Task.WhenAll(photo.Select(async (x) => (await Api.GetFileAsync(x.FileId)).FilePath!));
        }
    }
}