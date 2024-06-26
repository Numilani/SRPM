﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SimpleRPManager.Context;

#nullable disable

namespace SimpleRPManager.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240416023639_updates")]
    partial class updates
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("SimpleRPManager.Entities.Character", b =>
                {
                    b.Property<string>("CharacterId")
                        .HasColumnType("text");

                    b.Property<int>("ActivityStatus")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.Property<decimal>("OwnerId")
                        .HasColumnType("numeric(20,0)");

                    b.HasKey("CharacterId");

                    b.ToTable("Characters");
                });

            modelBuilder.Entity("SimpleRPManager.Entities.GuildSettings", b =>
                {
                    b.Property<decimal>("GuildId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal[]>("RoleplayChannels")
                        .HasColumnType("numeric(20,0)[]");

                    b.HasKey("GuildId");

                    b.ToTable("GuildSettings");
                });

            modelBuilder.Entity("SimpleRPManager.Entities.InventoryItem", b =>
                {
                    b.Property<string>("ItemId")
                        .HasColumnType("text");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("OwnerCharacterId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("ItemId");

                    b.HasIndex("OwnerCharacterId");

                    b.ToTable("InventoryItems");
                });

            modelBuilder.Entity("SimpleRPManager.Entities.PlayerSettings", b =>
                {
                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal>("PlayerId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("ActiveCharacterId")
                        .HasColumnType("text");

                    b.Property<bool>("AutoSpeakAsActiveCharacter")
                        .HasColumnType("boolean");

                    b.HasKey("GuildId", "PlayerId");

                    b.ToTable("PlayerSettings");
                });

            modelBuilder.Entity("SimpleRPManager.Entities.InventoryItem", b =>
                {
                    b.HasOne("SimpleRPManager.Entities.Character", "Owner")
                        .WithMany("Inventory")
                        .HasForeignKey("OwnerCharacterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("SimpleRPManager.Entities.Character", b =>
                {
                    b.Navigation("Inventory");
                });
#pragma warning restore 612, 618
        }
    }
}
