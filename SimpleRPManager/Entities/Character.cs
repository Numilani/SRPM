using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using shortid;
using shortid.Configuration;
using SimpleRPManager.Data;
using SimpleRPManager.Services;

namespace SimpleRPManager.Entities;

[Table("Characters")]
public class Character(ulong guildId, string name)
{
    [Key] public string CharacterId { get; set; } = CommonServices.GenerateSimpleUid();

    public ulong GuildId { get; set; } = guildId;

    [MaxLength(255)]
    public string Name { get; set; } = name;

    public CharacterStatus Status { get; set; } = CharacterStatus.ACTIVE;

    public List<InventoryItem> Inventory { get; set; } = new();
}