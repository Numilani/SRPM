using System.Reflection;
using Discord;
using Discord.Addons.Hosting;
using Discord.Addons.Hosting.Util;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SimpleRPManager.Context;
using IResult = Discord.Interactions.IResult;

namespace SimpleRPManager.Services;

public class InteractionHandler : DiscordClientService
{
    private readonly IServiceProvider _provider;
    private readonly InteractionService _interactionService;
    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _env;
    private ulong DEV_GUILD;

    public InteractionHandler(DiscordSocketClient client, ILogger<DiscordClientService> logger,
        IServiceProvider provider, InteractionService interactionService, IConfiguration config,
        IHostEnvironment env) : base(client, logger)
    {
        _provider = provider;
        _interactionService = interactionService;
        _configuration = config;
        _env = env;

        DEV_GUILD = Convert.ToUInt64(_configuration["Discord:DevGuild"]);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Client.MessageReceived += HandleMessages;
        Client.InteractionCreated += HandleInteraction;

        _interactionService.SlashCommandExecuted += SlashCommandExecuted;
        _interactionService.ContextCommandExecuted += ContextCommandExecuted;
        _interactionService.ComponentCommandExecuted += ComponentCommandExecuted;

        // Go find and collect all modules
        await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
        await Client.WaitForReadyAsync(stoppingToken);

#if DEBUG
        await _interactionService.RegisterCommandsToGuildAsync(DEV_GUILD);
#else
        await _interactionService.RegisterCommandsGloballyAsync();
#endif
    }

    private async Task HandleMessages(SocketMessage arg)
    {
        // // Don't process the command if it was a system message
        // var message = arg as SocketUserMessage;
        // if (message == null) return;
        //
        // // Create a WebSocket-based command context based on the message
        // var context = new SocketCommandContext(Client, message);
        //
        // // Execute the command with the command context we just
        // // created, along with the service provider for precondition checks.
        // await _commands.ExecuteAsync(
        //     context: context,
        //     argPos: argPos,
        //     services: null);
        if (arg.Channel.GetType() == typeof(SocketTextChannel))
        {
            var hooks = await (arg.Channel as SocketTextChannel).GetWebhooksAsync();
            if (hooks.Count(x => x.Name.StartsWith("SRPM_")) > 0)
            {
                var db = _provider.GetRequiredService<AppDbContext>();
                var settings = db.PlayerSettings.Find((arg.Channel as SocketTextChannel).Guild.Id, arg.Author.Id);
                if (settings is null || settings.ActiveCharacterId is null) return;

                var character = db.Characters.Find(settings.ActiveCharacterId);
                await CommonServices.SendWebhookMessage(character, (arg.Channel as SocketTextChannel), arg.Content);
                await arg.DeleteAsync();
            }
        }
    }

private Task ComponentCommandExecuted(ComponentCommandInfo commandInfo, IInteractionContext context, IResult result)
{
    if (!result.IsSuccess)
    {
        switch (result.Error)
        {
            case InteractionCommandError.UnmetPrecondition:
                // implement
                break;
            case InteractionCommandError.UnknownCommand:
                // implement
                break;
            case InteractionCommandError.BadArgs:
                // implement
                break;
            case InteractionCommandError.Exception:
                // implement
                break;
            case InteractionCommandError.Unsuccessful:
                // implement
                break;
            default:
                break;
        }
    }

    return Task.CompletedTask;
}

private Task ContextCommandExecuted(ContextCommandInfo context, IInteractionContext arg2, IResult result)
{
    if (!result.IsSuccess)
    {
        switch (result.Error)
        {
            case InteractionCommandError.UnmetPrecondition:
                // implement
                break;
            case InteractionCommandError.UnknownCommand:
                // implement
                break;
            case InteractionCommandError.BadArgs:
                // implement
                break;
            case InteractionCommandError.Exception:
                // implement
                break;
            case InteractionCommandError.Unsuccessful:
                // implement
                break;
            default:
                break;
        }
    }

    return Task.CompletedTask;
}

private Task SlashCommandExecuted(SlashCommandInfo commandInfo, IInteractionContext context, IResult result)
{
    if (!result.IsSuccess)
    {
        switch (result.Error)
        {
            case InteractionCommandError.UnmetPrecondition:
                // implement
                break;
            case InteractionCommandError.UnknownCommand:
                // implement
                break;
            case InteractionCommandError.BadArgs:
                // implement
                break;
            case InteractionCommandError.Exception:
                // implement
                break;
            case InteractionCommandError.Unsuccessful:
                // implement
                break;
            default:
                break;
        }
    }

    return Task.CompletedTask;
}

private async Task HandleInteraction(SocketInteraction arg)
{
    try
    {
        // Create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules
        var ctx = new SocketInteractionContext(Client, arg);
        await _interactionService.ExecuteCommandAsync(ctx, _provider);
    }
    catch (Exception ex)
    {
        Logger.LogError(ex, "Exception occurred whilst attempting to handle interaction.");

        if (arg.Type == InteractionType.ApplicationCommand)
        {
            var msg = await arg.GetOriginalResponseAsync();
            await msg.DeleteAsync();
        }
    }
}

}