using System.Globalization;
using Discord;
using Discord.Webhook;
using Discord.WebSocket;
using Serilog;
using shortid;
using shortid.Configuration;
using SimpleRPManager.Entities;

namespace SimpleRPManager.Services;

public class CommonServices
{
    private static GenerationOptions genOpts = new GenerationOptions(true, false);
    public static string GenerateSimpleUid()
    {
        return ShortId.Generate(genOpts);
    }

    public static async Task<DiscordWebhookClient> GetChannelWebhook(SocketTextChannel? channel)
    {
        if (channel?.GetWebhooksAsync().Result.Count(x => x.Name.StartsWith("SRPM_")) == 0)
        {
            return null;
        }
        return new DiscordWebhookClient(channel?.GetWebhooksAsync().Result.First(x => x.Name.StartsWith("SRPM_")));
    }
    
    public static async Task<bool> SendWebhookMessage(Character character, SocketTextChannel channel, string text)
    {
        try
        {
            var webhook = await GetChannelWebhook(channel);
            if (webhook is null)
            {
                // await FollowupAsync("This channel is not set up as an RP channel - you may not speak ICly here!");
                return false;
            }
            
            if (character.ImageUrl is not null)
            {
                await webhook.SendMessageAsync(text, avatarUrl: character.ImageUrl, username: CultureInfo.CurrentCulture.TextInfo.ToTitleCase(character.Name.ToLower()));
                return true;
            }
            else
            {
                await webhook.SendMessageAsync(text, username: CultureInfo.CurrentCulture.TextInfo.ToTitleCase(character.Name.ToLower()));
                return true;
            }
            
            // await DeleteOriginalResponseAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Couldn't execute SayAs command");
            return false;
            // await RespondAsync($"!! Ran into an issue: {ex.Message}", ephemeral:true);
        }
    }
}