using bot.Services;
using Microsoft.Extensions.Hosting;
using VkNet;
using VkNet.Exception;
using VkNet.Model;
using VkNet.Model.GroupUpdate;
using VkNet.Model.RequestParams;

namespace bot.Frontends
{
    public class VkFrontend : BackgroundService
    {
        private VkApi _api { get; set; } = new VkApi();

        private RouterService _router { get; }
        public VkFrontend(RouterService router, ApiAccessorService apiAccessor)
        {
            _api.Authorize(new ApiAuthParams
            {
                AccessToken = ConfigManager.Configuration.Tokens.Vk
            });

            _router = router;

            apiAccessor.Vk = _api;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            long g = (await _api.Groups.GetByIdAsync(null, null, null))[0].Id;

            LongPollServerResponse s = await _api.Groups.GetLongPollServerAsync((ulong)g);

            string ts = s.Ts;

            while (true)
            {
                BotsLongPollHistoryResponse poll;
                try
                {
                    poll = await _api.Groups.GetBotsLongPollHistoryAsync(new BotsLongPollHistoryParams
                    {
                        Server = s.Server,
                        Ts = ts,
                        Key = s.Key,
                        Wait = 25,
                    });
                }
                catch (LongPollKeyExpiredException)
                {
                    s = await _api.Groups.GetLongPollServerAsync((ulong)g);
                    continue;
                }

                ts = poll.Ts;

                if (poll.Updates == null)
                {
                    continue;
                }

                foreach (GroupUpdate e in poll.Updates)
                {
                    MessageNew? messageInstance = e.Instance as MessageNew;
                    if (messageInstance == null)
                    {
                        continue;
                    }

                    Dto.TextMessage msg = new Dto.TextMessage
                    {
                        ChatId = (decimal)messageInstance.Message.PeerId!,
                        UserId = (decimal)messageInstance.Message.FromId!,
                        Text = messageInstance.Message.Text,
                        Origin = messageInstance,
                        From = Frontends.Vk,
                    };

                    Console.WriteLine("got it 3");

                    await _router.ProcessTextMessage(msg);
                }
            }
        }
    }
}