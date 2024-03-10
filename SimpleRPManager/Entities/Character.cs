using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using SimpleRPManager.Data;

namespace SimpleRPManager.Entities;

[Table("Characters")]
public class Character(ulong guildId, string name)
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public uint CharacterId { get; set; }

    public ulong GuildId { get; set; } = guildId;

    [MaxLength(255)]
    public string Name { get; set; } = name;

    public CharacterStatus Status { get; set; } = CharacterStatus.ACTIVE;
}