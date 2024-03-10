using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleRPManager.Entities;

[Table("GuildSettings")]
public class GuildSettings(ulong guildId)
{
    [Key] public ulong GuildId { get; set; } = guildId;

    public ulong[]? RoleplayChannels { get; set; }
}