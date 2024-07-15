using Discord;
using Discord.Interactions;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SimpleRPManager.Context;
using SimpleRPManager.Data;
using SimpleRPManager.Entities;
using SimpleRPManager.Services.AutocompleteHandlers;

namespace SimpleRPManager.Services;

[Group("char", "Manage your characters")]
public class CharacterInteractionCommands : InteractionModuleBase<SocketInteractionContext>
{
    public AppDbContext DB { get; set; }

    public CharacterInteractionCommands(AppDbContext db)
    {
        DB = db;
    }

    [SlashCommand("create", "Create a new character")]
    public async Task CreateCharacter()
    {
        await RespondWithModalAsync<EditCharacterModal>("create_char");
    }

    [ModalInteraction("create_char", true)]
    public async Task CreateCharacterModalResponse(EditCharacterModal modal)
    {
        var character = new Character(Context.Guild.Id, Context.User.Id, modal.Name);
        await DeferAsync();
        if (DB.Characters.Where(x => x.Name == character.Name).Any())
        {
            await FollowupAsync("A character with this name already exists!");
            return;
        }

        if (!String.IsNullOrWhiteSpace(modal.Description))
        {
            character.Description = modal.Description;
        }

        if (!String.IsNullOrWhiteSpace(modal.ImageUrl))
        {
            character.ImageUrl = modal.ImageUrl;
        }

        try
        {
            await DB.Characters.AddAsync(character);
            await DB.SaveChangesAsync();
            await FollowupAsync($"{character.Name} has been created!", ephemeral: true);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to save new character");
            await FollowupAsync($"Something went wrong: {ex.Message}", ephemeral: true);
        }
    }

    [SlashCommand("setactive", "Set a character as your active character")]
    public async Task SetActiveCharacter(
        [Autocomplete(typeof(CharacterAutocompleteHandler))] string characterId
    )
    {
        await DeferAsync();
        var userSettings = DB.PlayerSettings.FirstOrDefault(x => x.PlayerId == Context.User.Id);
        var character = DB.Characters.FirstOrDefault(x => x.CharacterId == characterId);
        if (userSettings is null)
        {
            var newSettings = new PlayerSettings(Context.Guild.Id, Context.User.Id);
            await DB.PlayerSettings.AddAsync(newSettings);
            userSettings = newSettings;
            await DB.SaveChangesAsync();
        }

        if (character is null)
        {
            await FollowupAsync("Wasn't able to find that character!", ephemeral: true);
            return;
        }

        userSettings.ActiveCharacterId = character.CharacterId;
        await DB.SaveChangesAsync();
        await FollowupAsync($"{character.Name} is now your active character!", ephemeral: true);
    }

    [SlashCommand("view", "View your active character, or another character's info")]
    public async Task ViewCharacter(
        [Autocomplete(typeof(AllCharactersAutocompleteHandler))] string characterId = ""
    )
    {
        Character character;
        if (String.IsNullOrEmpty(characterId))
        {
            var settings = DB.PlayerSettings.Find(Context.Guild.Id, Context.User.Id);
            if (settings is null)
            {
                await RespondAsync("Couldn't fetch settings to find your active character!", ephemeral: true);
                return;
            }
        
            character = await DB.Characters.FindAsync(settings.ActiveCharacterId);
            if (character is null)
            {
                await RespondAsync(
                    "You don't appear to have an active character... are you trying to search for someone else's character?"
                , ephemeral: true);
                return;
            }
        }
        else
        {
            character = await DB.Characters.FindAsync(characterId);
        }

        var contents = $"""
            **Name:** {character.Name}
            **Status:** {character.ActivityStatus.ToString()}
            **Description:** 
            {character.Description}
            """;

        await RespondAsync(
            embed: new EmbedBuilder()
                .WithTitle($"Character Card: {character.Name}")
                .WithDescription(contents)
                .WithImageUrl(character.ImageUrl)
                .WithColor(Color.DarkBlue)
                .Build(), ephemeral: true
        );
    }

    [SlashCommand("list", "View all characters a specific player (or yourself) can RP as.")]
    public async Task ListYourCharacters(IUser user)
    {
        List<string> nameList = new();

        var characters = DB.Characters.Where(x => x.OwnerId == user.Id).ToList();
        foreach (var character in characters)
        {
            nameList.Add($"- {character.Name}");
        }

        await RespondAsync(
            embed: new EmbedBuilder()
                .WithTitle($"{user.Username}'s characters")
                .WithDescription(string.Join("\n", nameList))
                .WithFooter("Use /char view <character> to view more info.")
                .WithColor(Color.DarkBlue)
                .Build(), ephemeral: true
        );
    }

    [SlashCommand("edit", "Edit a character's info")]
    public async Task EditCharacter(
        [Autocomplete(typeof(CharacterAutocompleteHandler))] string characterId
    )
    {
        var character = DB.Characters.FirstOrDefault(x => x.CharacterId == characterId);

        var modal = new EditCharacterModal()
        {
            Name = character.Name,
            Description = character.Description,
            ImageUrl = character.ImageUrl
        };
        await RespondWithModalAsync<EditCharacterModal>(
            $"edit_char:{character.CharacterId}",
            modal
        );
    }

    [SlashCommand("retire", "Retire a character")]
    public async Task RetireCharacter(
        [Autocomplete(typeof(CharacterAutocompleteHandler))] string characterId,
        [Choice("Retired", "retired"), Choice("Deceased", "deceased")] string retireChoice
    )
    {
        var character = DB.Characters.FirstOrDefault(x => x.CharacterId == characterId);
        if (character is null)
        {
            await RespondAsync("Wasn't able to find that character!", ephemeral: true);
            return;
        }

        switch (retireChoice)
        {
            case "retired":
                character.ActivityStatus = CharacterActivityStatus.RETIRED;
                await RespondAsync("Your character has been retired!", ephemeral: true);
                break;
            case "deceased":
                character.ActivityStatus = CharacterActivityStatus.DECEASED;
                await RespondAsync("Your character is now considered deceased", ephemeral: true);
                break;
        }
        
        var settings = DB.PlayerSettings.Find(Context.Guild.Id, Context.User.Id);
        settings.ActiveCharacterId = null;

        DB.SaveChanges();
    }

    [ModalInteraction("edit_char:*", true)]
    public async Task EditCharacterModalResponse(string? idParam, EditCharacterModal modal)
    {
        var character = DB.Characters.FirstOrDefault(x => x.CharacterId == idParam);
        if (character is null)
        {
            await RespondAsync("Wasn't able to find that character!", ephemeral: true);
            return;
        }

        character.Name = modal.Name;
        character.Description = modal.Description;
        character.ImageUrl = modal.ImageUrl;

        await DB.SaveChangesAsync();
        await RespondAsync("Your character information has been successfully updated!", ephemeral: true);
    }

    public class EditCharacterModal : IModal
    {
        public string Title => "Character Details";

        [InputLabel("Character Name")]
        [ModalTextInput("charname")]
        public string Name { get; set; }

        [RequiredInput(false)]
        [InputLabel("Character Description")]
        [ModalTextInput("description", TextInputStyle.Paragraph)]
        public string Description { get; set; }

        [RequiredInput(false)]
        [InputLabel("Image URL")]
        [ModalTextInput(
            "imageurl",
            TextInputStyle.Short,
            "It's recommended to use Imgur for long-term image storage."
        )]
        public string ImageUrl { get; set; }
    }
}
