using bot.Frontends;
using bot.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureLogging((logging) =>
{
    logging.AddConsole();
});

builder.ConfigureServices((services) =>
{
    services.AddOptions();

    services.AddSingleton<ILogger>((svc) =>
        svc.GetRequiredService<ILogger>()
    );

    services.AddDbContext<ApplicationContext>((options) => options.UseNpgsql(ConfigManager.Configuration.Database.ConnectionString));

    services.AddSingleton<RouterService>();
    services.AddSingleton<ApiAccessorService>();

    services.AddHostedService<VkFrontend>();
    services.AddHostedService<DiscordFrontend>();
    services.AddHostedService<TelegramFrontend>();

    services.PostConfigureAll<HostOptions>((options) => options.ShutdownTimeout = TimeSpan.FromSeconds(1));
});

await builder.RunConsoleAsync();