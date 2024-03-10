using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SimpleRPManager.Services;

namespace SimpleRPManager.Entities;

[Table("InventoryItems")]
public class InventoryItem(ulong guildId, string name)
{
    [Key] public string ItemId { get; set; } = CommonServices.GenerateSimpleUid();

    public ulong GuildId { get; set; } = guildId;

    public string Name { get; set; } = name;
    public string Description = "This item has no description set!";
    
    public Character Owner { get; set; }
}