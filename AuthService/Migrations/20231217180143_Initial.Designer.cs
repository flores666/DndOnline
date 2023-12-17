﻿// <auto-generated />
using System;
using AuthService.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AuthService.Migrations
{
    [DbContext(typeof(AuthServiceDbContext))]
    [Migration("20231217180143_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("AuthService.DataAccess.Objects.RefreshToken", b =>
                {
                    b.Property<string>("Token")
                        .HasColumnType("text");

                    b.Property<DateTime>("ExpiryTime")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Token");

                    b.ToTable("RefreshTokens");
                });

            modelBuilder.Entity("AuthService.DataAccess.Objects.User", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("varchar(25)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("RefreshTokenToken")
                        .HasColumnType("text");

                    b.HasKey("Name");

                    b.HasIndex("RefreshTokenToken");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("AuthService.DataAccess.Objects.User", b =>
                {
                    b.HasOne("AuthService.DataAccess.Objects.RefreshToken", "RefreshToken")
                        .WithMany()
                        .HasForeignKey("RefreshTokenToken");

                    b.Navigation("RefreshToken");
                });
#pragma warning restore 612, 618
        }
    }
}
