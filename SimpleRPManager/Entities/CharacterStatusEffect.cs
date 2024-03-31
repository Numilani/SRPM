using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SimpleRPManager.Services;

namespace SimpleRPManager.Entities;

[Table("CharacterStatusEffects")]
public class CharacterStatusEffect(ulong guildId, string effectName)
{
    [Key] public string EffectId { get; set; } = CommonServices.GenerateSimpleUid();
    public ulong GuildId { get; set; } = guildId;
    
    public string EffectName { get; set; }
    public string Description { get; set; }
}