using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SimpleRPManager.Entities;

namespace SimpleRPManager.Context;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<GuildSettings> GuildSettings { get; set; }
    public virtual DbSet<PlayerSettings> PlayerSettings { get; set; }
    public virtual DbSet<Character> Characters { get; set; }

}