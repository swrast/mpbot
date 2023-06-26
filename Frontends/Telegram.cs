using bot.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace bot.Frontends
{
    public class TelegramFrontend : BackgroundService, IUpdateHandler
    {
        private TelegramBotClient _client { get; set; }
        private ReceiverOptions _options { get; set; }

        private ILogger _logger { get; }
        private RouterService _router { get; }

        public TelegramFrontend(ILogger<TelegramFrontend> logger, RouterService router, ApiAccessorService apiAccessor)
        {
            _client = new TelegramBotClient(ConfigManager.Configuration.Tokens.Telegram);
            _options = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };

            _logger = logger;
            _router = router;
            apiAccessor.Telegram = _client;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _client.StartReceiving(
                updateHandler: this,
                receiverOptions: _options
            );

            return Task.CompletedTask;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message == null)
            {
                return;
            }

            Message messageInstance = update.Message;

            Dto.TextMessage msg = new Dto.TextMessage
            {
                ChatId = messageInstance.Chat.Id,
                UserId = messageInstance.From!.Id,
                Text = (messageInstance.Text != null ? messageInstance.Text : messageInstance.Caption)!,
                Origin = messageInstance,
                From = Frontends.Telegram
            };

            await _router.ProcessTextMessage(msg);
        }

        public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(1, exception, "Telegram polling error");

            return Task.CompletedTask;
        }
    }
}