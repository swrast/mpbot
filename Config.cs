using Config.Net;

namespace bot.ConfigEntities
{
    public interface ITokens
    {
        string Vk { get; }
        string Discord { get; }
        string Telegram { get; }
    }
    public interface IDatabase
    {
        string ConnectionString { get; }
    }

    public interface IConfig
    {
        ITokens Tokens { get; }
        IDatabase Database { get; }
    }
}

public class ConfigManager
{
    public static bot.ConfigEntities.IConfig Configuration { get; set; } =
        new ConfigurationBuilder<bot.ConfigEntities.IConfig>().UseIniFile("config.ini").UseEnvironmentVariables().Build();
}