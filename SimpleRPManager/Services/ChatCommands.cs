using System.Globalization;
using Discord.Interactions;
using Discord.Webhook;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SimpleRPManager.Context;
using SimpleRPManager.Services.AutocompleteHandlers;

namespace SimpleRPManager.Services;

public class ChatCommands : InteractionModuleBase<SocketInteractionContext>
{
    
    public AppDbContext Db { get; set; }

    public ChatCommands(AppDbContext db)
    {
        Db = db;
    }

    [SlashCommand("sayas", "Speak as an NPC", ignoreGroupNames:true)]
    public async Task SayAs([Autocomplete(typeof(CharacterAutocompleteHandler))] string characterId, string text)
    {
        await DeferAsync();

        var channel = Context.Channel as SocketTextChannel;
        var character = Db.Characters.FirstOrDefault(x => x.CharacterId == characterId);
            var webhook = await CommonServices.GetChannelWebhook(channel);

            if (await CommonServices.SendWebhookMessage(character, channel, text))
            {
                await DeleteOriginalResponseAsync();

            }
            else await RespondAsync($"!! Ran into an issue sending that message, try again later!", ephemeral:true);
    }
    
    [SlashCommand("make-ic", "Initialize this channel as an RP channel!")]
    public async Task CreateChannelWebhook(SocketTextChannel? channel)
    {
        if (channel.GetWebhooksAsync().Result.Count(x => x.Name.StartsWith("SRPM_")) == 0)
        {
            var x = await channel.CreateWebhookAsync(
                $"SRPM_{channel.Name.ToLower()}_{DateTime.Now.ToString("MMddyyHHmmssfff")}");
            await RespondAsync("This channel is now designated as an RP channel!");
        }
        else
        {
            await RespondAsync(
                "This channel is already an RP channel! If you wish to remove that status, simply delete the webhook in the channel's settings.");
        }
    }
    
    

}