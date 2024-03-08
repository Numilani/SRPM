using Discord;
using Discord.Addons.Hosting;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NpgsqlTypes;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL;
using Serilog.Sinks.PostgreSQL.ColumnWriters;

public class Program
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
        
        IHost app = appBuilder.Build();

        Log.Warning("App Generic Host built, running...");
        
        await app.RunAsync();
    }
}