using Discord;
using Discord.Addons.Hosting;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using SimpleRPManager.Context;
using SimpleRPManager.Services;

namespace SimpleRPManager;

public static class Program
{
    public static async Task Main()
    {
        HostApplicationBuilder appBuilder = Host.CreateApplicationBuilder();

        // Set up logging
        appBuilder.Logging.ClearProviders();
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(appBuilder.Configuration)
            .CreateLogger();
        appBuilder.Logging.AddSerilog();
        
        // Set up services here

        appBuilder.Services.AddDbContext<AppDbContext>(opts =>
        {
            opts.UseNpgsql(appBuilder.Configuration["ConnectionStrings:Default"]);
        });
        
        appBuilder.Services.AddDiscordHost((config, _) =>
        {
            config.SocketConfig = new DiscordSocketConfig()
            {
                LogLevel = LogSeverity.Verbose,
                AlwaysDownloadUsers = true,
                MessageCacheSize = 200,
                GatewayIntents = GatewayIntents.All
            };

            config.Token = appBuilder.Configuration["Discord:BotToken"] ?? throw new InvalidOperationException("Bot Token must be set in the configuration.");
        });
        appBuilder.Services.AddInteractionService((config, _) =>
        {
            config.LogLevel = LogSeverity.Verbose;
            config.UseCompiledLambda = true;
        });
        appBuilder.Services.AddHostedService<InteractionHandler>();
        
        IHost app = appBuilder.Build();
        
        await app.RunAsync();
    }
}