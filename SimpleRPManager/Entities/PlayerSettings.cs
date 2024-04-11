using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SimpleRPManager.Entities;

[Table("PlayerSettings")]
[PrimaryKey(nameof(GuildId), nameof(PlayerId))]
public class PlayerSettings(ulong guildId, ulong playerId)
{
    public ulong GuildId { get; set; } = guildId;
    public ulong PlayerId { get; set; } = playerId;

    public string? ActiveCharacterId { get; set; }
    public bool AutoSpeakAsActiveCharacter { get; set; }
    
}