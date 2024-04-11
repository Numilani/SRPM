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
        var channel = Context.Channel as SocketTextChannel;
        var character = Db.Characters.FirstOrDefault(x => x.CharacterId == characterId);

        try
        {
            var webhook = await GetChannelWebhook(channel);
            if (webhook is null)
            {
                await RespondAsync("This channel is not set up as an RP channel - you may not speak ICly here!");
                return;
            }
            
            if (character.ImageUrl is not null)
            {
                await webhook.SendMessageAsync(text, avatarUrl: character.ImageUrl, username: CultureInfo.CurrentCulture.TextInfo.ToTitleCase(character.Name.ToLower()));
            }
            else
            {
                await webhook.SendMessageAsync(text, username: CultureInfo.CurrentCulture.TextInfo.ToTitleCase(character.Name.ToLower()));
            }
            
            await RespondAsync("done");
            await DeleteOriginalResponseAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Couldn't execute SayAs command");
            await RespondAsync($"!! Ran into an issue: {ex.Message}", ephemeral:true);
        }
    }
    
    [SlashCommand("makeRPchannel", "Initialize this channel as an RP channel!")]
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
    
    private static async Task<DiscordWebhookClient> GetChannelWebhook(SocketTextChannel? channel)
    {
        if (channel?.GetWebhooksAsync().Result.Count(x => x.Name.StartsWith("SRPM_")) == 0)
        {
            return null;
        }
        return new DiscordWebhookClient(channel?.GetWebhooksAsync().Result.First(x => x.Name.StartsWith("SRPM_")));
    }

}