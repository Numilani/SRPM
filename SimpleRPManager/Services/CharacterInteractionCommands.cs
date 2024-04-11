using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.Interactions;
using Serilog;
using SimpleRPManager.Context;
using SimpleRPManager.Data;
using SimpleRPManager.Entities;
using SimpleRPManager.Services.AutocompleteHandlers;

namespace SimpleRPManager.Services;

[Discord.Interactions.Group("char", "Manage your characters")]
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

    [ModalInteraction("create_char")]
    public async Task CreateCharacterModalResponse(EditCharacterModal modal)
    {
        var character = new Character(Context.Guild.Id, Context.User.Id, modal.Name);
        if (DB.Characters.Where(x => x.Name == character.Name).Any())
        {
            await RespondAsync("A character with this name already exists!");
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
            DB.Characters.Update(character);
            await DB.SaveChangesAsync();
            await RespondAsync($"{character.Name} has been created!");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to save new character");
            await RespondAsync($"Something went wrong: {ex.Message}");
        }
    }

    [SlashCommand("setactive", "Set a character as your active character")]
    public async Task SetActiveCharacter([Autocomplete(typeof(CharacterAutocompleteHandler))] string characterId)
    {
        var userSettings = DB.PlayerSettings.FirstOrDefault(x => x.PlayerId == Context.User.Id);
        var character = DB.Characters.FirstOrDefault(x => x.CharacterId == characterId);
        if (userSettings is null)
        {
            var newSettings = new PlayerSettings(Context.Guild.Id, Context.User.Id);
            DB.PlayerSettings.Update(newSettings);
            userSettings = newSettings;
            await DB.SaveChangesAsync();
        }

        if (character is null)
        {
            await RespondAsync("Wasn't able to find that character!");
            return;
        }

        userSettings.ActiveCharacterId = character.CharacterId;
    }

    [SlashCommand("retire", "Retire a character")]
    public async Task RetireCharacter([Autocomplete(typeof(CharacterAutocompleteHandler))] string characterId,
        [Choice("Retired", "retired"), Choice("Deceased", "deceased")] string retireChoice)
    {
        var character = DB.Characters.FirstOrDefault(x => x.CharacterId == characterId);
        if (character is null)
        {
            await RespondAsync("Wasn't able to find that character!");
            return;
        }
        
        switch (retireChoice)
        {
            case "retired":
                character.ActivityStatus = CharacterActivityStatus.RETIRED;
                await RespondAsync("Your character has been retired!");
                break;
            case "deceased":
                character.ActivityStatus = CharacterActivityStatus.DECEASED;
                await RespondAsync("Your character is now considered deceased");
                break;
        }

        DB.SaveChanges();
        
    }

    [SlashCommand("edit", "Edit a character's info")]
    public async Task EditCharacter([Autocomplete(typeof(CharacterAutocompleteHandler))] string characterId)
    {
        var character = DB.Characters.FirstOrDefault(x => x.CharacterId == characterId);

        var modal = new EditCharacterModal(){Name = character.Name, Description = character.Description, ImageUrl = character.ImageUrl, Id = character.CharacterId};
        await RespondWithModalAsync<EditCharacterModal>("edit_char", modal);
    }

    [ModalInteraction("edit_char")]
    public async Task EditCharacterModalResponse(EditCharacterModal modal)
    {
        var character = DB.Characters.FirstOrDefault(x => x.CharacterId == modal.Id);
        if (character is null)
        {
            await RespondAsync("Wasn't able to find that character!");
            return;
        }

        character.Name = modal.Name;
        character.Description = modal.Description;
        character.ImageUrl = modal.ImageUrl;

        await DB.SaveChangesAsync();
        await RespondAsync("Your character information has been successfully updated!");
    }

    public class EditCharacterModal : IModal
    {
        public string Title => "Character Details";

        [RequiredInput(true)]
        [InputLabel("Character Name")]
        [ModalTextInput("charname", TextInputStyle.Short)]
        public string Name { get; set; }

        [RequiredInput(false)]
        [InputLabel("Character Description")]
        [ModalTextInput("description", TextInputStyle.Paragraph)]
        public string Description { get; set; }

        [RequiredInput(false)]
        [InputLabel("Image URL")]
        [ModalTextInput("imageurl", TextInputStyle.Short, "It's recommended to use Imgur for long-term image storage.")]
        public string ImageUrl { get; set; }

        public string Id;
    }
    
    
}