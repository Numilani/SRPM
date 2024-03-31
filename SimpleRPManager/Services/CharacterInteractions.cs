using Discord.Addons.Hosting;
using Discord.Interactions;

namespace SimpleRPManager.Services;

public class CharacterInteractions : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("testmsg", "test the bot!")]
    public async Task Test(string msg)
    {
        await RespondAsync($"You said: {msg}!");
    }
}