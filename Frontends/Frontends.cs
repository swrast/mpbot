using System.Runtime.Serialization;

namespace bot.Frontends
{
    public enum Frontends
    {
        [EnumMember(Value = "ВКонтакте")]
        Vk,
        [EnumMember(Value = "Telegram")]
        Telegram,
        [EnumMember(Value = "Discord")]
        Discord
    }
}