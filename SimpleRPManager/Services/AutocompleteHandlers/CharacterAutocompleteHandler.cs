using Discord;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using SimpleRPManager.Context;

namespace SimpleRPManager.Services.AutocompleteHandlers;

public class CharacterAutocompleteHandler : AutocompleteHandler
{
    
    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction,
        IParameterInfo parameter, IServiceProvider services)
    {
        var Db = services.GetRequiredService<AppDbContext>();
        
        var results = new List<AutocompleteResult>();
        Db.Characters.Where(x => context.User.Id == x.OwnerId).ToList().ForEach(x => results.Add(new AutocompleteResult(x.Name, x.CharacterId)));
        return AutocompletionResult.FromSuccess(results.Take(25));
    }
}