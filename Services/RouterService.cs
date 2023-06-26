using bot.Dto;
using Microsoft.Extensions.Logging;
using bot.Commands;
using Microsoft.Extensions.DependencyInjection;
using bot.ApiProviders;
using bot.Models;
using Microsoft.EntityFrameworkCore;

namespace bot.Services
{
    public class RouterService
    {
        private ILogger _logger { get; }
        private IServiceProvider _services { get; }
        private ApiAccessorService _apiAccessor { get; }
        private ApplicationContext _context { get; }

        private List<(string[] Aliases, Type Command, string FunctionName)> _commands { get; } = new List<(string[], Type, string)>();

        public RouterService(ILogger<RouterService> logger, IServiceProvider services, ApiAccessorService apiAccessor, ApplicationContext context)
        {
            _logger = logger;
            _services = services;
            _apiAccessor = apiAccessor;
            _context = context;

            IEnumerable<Type> types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(CommandBase).IsAssignableFrom(p));

            foreach (Type t in types)
            {
                if (t.IsAbstract)
                {
                    continue;
                }

                foreach (var m in t.GetMethods())
                {
                    { // Text Messages
                        HandleMessageAttribute? handleMessageAttribute = Attribute.GetCustomAttribute(m, typeof(HandleMessageAttribute)) as HandleMessageAttribute;
                        if (handleMessageAttribute == null)
                        {
                            continue;
                        }

                        var entry = (handleMessageAttribute.Aliases, t, m.Name);
                        _commands.Add(entry);

                        logger.LogInformation("Found " + entry.Name + " for [" + string.Join(", ", entry.Aliases) + "] in " + entry.t.Name);
                    }
                }
            }
        }

        public async Task ProcessTextMessage(TextMessage msg) // todo: rework completely
        {
            if (msg.Text == null || msg.Text == "")
            {
                return;
            }

            string[] tokens = msg.Text.TrimStart('/', '!').Split(" ");
            if (tokens.Length < 1)
            {
                return;
            }

            foreach (var entry in _commands)
            {
                if (entry.Aliases.Contains(tokens[0]))
                {
                    CommandBase handler = (ActivatorUtilities.CreateInstance(_services, entry.Command) as CommandBase)!;
                    Type t = null!;
                    User? dbUser = null!;

                    switch (msg.From)
                    {
                        case Frontends.Frontends.Vk:
                            t = typeof(VkApi);
                            dbUser = await _context.Users.FirstOrDefaultAsync((x) => x.VkId == msg.UserId);
                            if (dbUser == null)
                            {
                                dbUser = new User
                                {
                                    VkId = msg.UserId
                                };
                            }
                            break;
                        case Frontends.Frontends.Telegram:
                            if ((msg.Origin as Telegram.Bot.Types.Message)!.From!.Username == "GroupAnonymousBot")
                            {
                                return;
                            }

                            t = typeof(TelegramApi);
                            dbUser = await _context.Users.FirstOrDefaultAsync((x) => x.TelegramId == msg.UserId);
                            if (dbUser == null)
                            {
                                dbUser = new User
                                {
                                    TelegramId = msg.UserId
                                };
                            }
                            break;
                        case Frontends.Frontends.Discord:
                            t = typeof(DiscordApi);
                            dbUser = await _context.Users.FirstOrDefaultAsync((x) => x.DiscordId == msg.UserId);
                            if (dbUser == null)
                            {
                                dbUser = new User
                                {
                                    DiscordId = msg.UserId
                                };
                            }
                            break;
                    }

                    _context.Users.Attach(dbUser!);

                    var api = (ActivatorUtilities.CreateInstance(_services, t, msg, dbUser) as IApiProvider)!;

                    handler.SetupBase(msg, api, dbUser, tokens.Skip(1));
                    await handler.Setup();

                    var method = entry.Command.GetMethod(entry.FunctionName)!;
                    ExcludeFrontendAttribute? excludeFrontendAttribute = Attribute.GetCustomAttribute(method, typeof(ExcludeFrontendAttribute)) as ExcludeFrontendAttribute;
                    if (excludeFrontendAttribute != null)
                    {
                        if (excludeFrontendAttribute.Blacklist.Contains(msg.From))
                        {
                            await api.Reply(await api.GetMention(msg.UserId) + ", к сожалению, эта команда не поддерживается на платформе " + msg.From);

                            return;
                        }
                    }

                    try
                    {
                        await (Task)method.Invoke(handler, null)!;
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(2, e, "Command handling error");
                    }
                }
            }
        }
    }
}