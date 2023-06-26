using DSharpPlus;
using Telegram.Bot;
using VkNet;

namespace bot.Services
{
    public class ApiAccessorService
    {
        public VkApi Vk { get; set; } = null!;
        public DiscordClient Discord { get; set; } = null!;
        public TelegramBotClient Telegram { get; set; } = null!;
    }
}