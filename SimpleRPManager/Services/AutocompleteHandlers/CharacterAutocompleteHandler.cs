using Discord;
using Discord.Interactions;
using SimpleRPManager.Context;

namespace SimpleRPManager.Services.AutocompleteHandlers;

public class CharacterAutocompleteHandler : AutocompleteHandler
{
    private AppDbContext Db;

    public CharacterAutocompleteHandler(AppDbContext db)
    {
        Db = db;
    }

    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction,
        IParameterInfo parameter, IServiceProvider services)
    {
        var results = new List<AutocompleteResult>();
        Db.Characters.ToList().ForEach(x => results.Add(new AutocompleteResult(x.Name, x.CharacterId)));
        return AutocompletionResult.FromSuccess(results.Take(25));
    }
}