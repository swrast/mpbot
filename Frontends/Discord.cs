using bot.Dto;
using bot.Services;
using DSharpPlus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace bot.Frontends
{
    public class DiscordFrontend : BackgroundService
    {
        private DiscordClient _client { get; set; }

        private RouterService _router { get; }

        public DiscordFrontend(RouterService router, ApiAccessorService apiAccessor)
        {
            _client = new DiscordClient(new DiscordConfiguration
            {
                Token = ConfigManager.Configuration.Tokens.Discord,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged,
                MinimumLogLevel = LogLevel.Critical
            });

            _router = router;
            apiAccessor.Discord = _client;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _client.ConnectAsync();

            _client.MessageCreated += async (_, e) =>
            {
                TextMessage msg = new TextMessage
                {
                    ChatId = e.Message.ChannelId,
                    UserId = e.Message.Author.Id,
                    Text = e.Message.Content,
                    Origin = e,
                    From = Frontends.Discord
                };

                await _router.ProcessTextMessage(msg);
            };

            await Task.Delay(-1);
        }
    }
}