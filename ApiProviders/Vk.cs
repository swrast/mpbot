using bot.Dto;
using bot.Exceptions;
using bot.Models;
using bot.Services;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.Attachments;
using VkNet.Model.GroupUpdate;
using VkNet.Model.RequestParams;

namespace bot.ApiProviders
{
    public class VkApi : IApiProvider
    {
        public VkNet.VkApi Api { get; }

        private IReplyable? _message { get; }
        private MessageBase? _base { get; }
        private ApplicationContext _context { get; }
        private User _user { get; }

        public VkApi(ApiAccessorService apiAccessor, ApplicationContext context, object request, User user)
        {
            Api = apiAccessor.Vk;
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
            await Api.Messages.SendAsync(new MessagesSendParams
            {
                Message = text,
                RandomId = 0,
                PeerId = (long)_message!.ChatId,
                DisableMentions = true
            });
        }

        public async Task<string> GetNick(decimal id)
        {
            string nick = "Неизвестно";

            if (_user.Nickname == string.Empty)
            {
                var vkUser = await Api.Users.GetAsync(new string[] { id.ToString() });
                if (vkUser.Count > 0)
                {
                    nick = vkUser[0].FirstName + " " + vkUser[0].LastName;

                    _user.Nickname = nick;

                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                nick = _user.Nickname;
            }

            return nick;
        }

        public async Task<string> GetMention(decimal id) => $"[id{id}|{await GetNick(id)}]";

        public async Task<IEnumerable<string>> GetPictures()
            => await Task.FromResult((_base!.Origin! as MessageNew)!
            .Message.Attachments.Where((x) => x.Type == typeof(Photo))
            .Select((x) => (x.Instance as Photo)!.Sizes.Where((y) => y.Type == PhotoSizeType.Z).First().Url.ToString()));
    }
}