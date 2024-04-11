using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using shortid;
using shortid.Configuration;
using SimpleRPManager.Data;
using SimpleRPManager.Services;

namespace SimpleRPManager.Entities;

[Table("Characters")]
public class Character(ulong guildId, ulong ownerId, string name)
{
    [Key] public string CharacterId { get; set; } = CommonServices.GenerateSimpleUid();
    public ulong GuildId { get; set; } = guildId;
    public ulong OwnerId { get; set; } = ownerId;

    public CharacterActivityStatus ActivityStatus { get; set; } = CharacterActivityStatus.ACTIVE;

    [MaxLength(32)]
    public string Name { get; set; } = name;
    public string? ImageUrl { get; set; }
    public string? Description { get; set; }

    public List<CharacterStatusEffect> StatusEffects = new();
    public List<InventoryItem> Inventory { get; set; } = new();
}