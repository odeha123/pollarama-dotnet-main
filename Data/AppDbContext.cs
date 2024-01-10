using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DotNetAPI;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ArtistAwards;

namespace ArtistAwards.Data
{
  public class AppDbContext : DbContext
  {
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<UserRole> Userroles { get; set; }
    public virtual DbSet<User> Users { get; set; }
    //public DbSet<Artist> Artists { get; set; }

    public virtual DbSet<PollOption> PollOptions { get; set; }
    public virtual DbSet<PollStatus> PollStatuses { get; set; }
    public virtual DbSet<Poll> Polls { get; set; }
    public virtual DbSet<UserVotes> UserVotes { get; set; }
    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    //  modelBuilder.Entity<Artist>().ToTable("Artists");
    //  modelBuilder.Entity<Artist>()
    //.Property(e => e.Id)
    //.ValueGeneratedOnAdd();
      //  modelBuilder.Entity<User>()
      //.Property(u => u.Id)
      //.ValueGeneratedOnAdd();
      modelBuilder.Entity<Role>(entity =>
      {
        entity.ToTable("roles");

        entity.Property(e => e.Id).HasColumnName("id");

        entity.Property(e => e.Name)
            .HasColumnName("name")
            .HasMaxLength(50);
      });

      modelBuilder.Entity<UserRole>(entity =>
      {
        entity.Property(e => e.Id).HasColumnName("id");

        entity.ToTable("userroles");

        entity.Property(e => e.Roleid).HasColumnName("roleid");

        entity.Property(e => e.Userid).HasColumnName("userid");

        entity.HasOne(d => d.Role)
            .WithMany()
            .HasForeignKey(d => d.Roleid)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("userroles_roleid_fkey");

        entity.HasOne(d => d.User)
            .WithMany()
            .HasForeignKey(d => d.Userid)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("userroles_userid_fkey");
      });

      modelBuilder.Entity<User>(entity =>
      {
        entity.ToTable("users");

        entity.HasIndex(e => e.Email)
            .HasName("users_email_key")
            .IsUnique();

        entity.Property(e => e.Id).HasColumnName("id");

        entity.Property(e => e.Email)
            .IsRequired()
            .HasColumnName("email")
            .HasMaxLength(255);

        entity.Property(e => e.Name)
            .IsRequired()
            .HasColumnName("name")
            .HasMaxLength(50);

        entity.Property(e => e.Passwordhash)
            .IsRequired()
            .HasColumnName("passwordhash")
            .HasMaxLength(100);
      });

      modelBuilder.HasPostgresExtension("uuid-ossp");

      modelBuilder.Entity<PollOption>(entity =>
      {
        entity.ToTable("poll_options");

        entity.Property(e => e.Id)
            .HasColumnName("id")
            .UseIdentityAlwaysColumn();

        entity.Property(e => e.Content)
            .IsRequired()
            .HasColumnName("content")
            .HasColumnType("character varying");

        entity.Property(e => e.PollId).HasColumnName("poll_id");

        entity.Property(e => e.Votes)
            .HasColumnName("votes")
            .HasDefaultValueSql("0");

        entity.HasOne(d => d.Poll)
            .WithMany(p => p.PollOptions)
            .HasForeignKey(d => d.PollId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("poll_options_poll_id_fkey");
      });

      modelBuilder.Entity<PollStatus>(entity =>
      {
        entity.ToTable("poll_statuses");

        entity.Property(e => e.Id)
            .HasColumnName("id")
            .UseIdentityAlwaysColumn();

        entity.Property(e => e.Name)
            .IsRequired()
            .HasColumnName("name")
            .HasColumnType("character varying");
      });

      modelBuilder.Entity<Poll>(entity =>
      {
        entity.ToTable("polls");

        entity.Property(e => e.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("uuid_generate_v4()");

        entity.Property(e => e.CreatorId).HasColumnName("creator_id");

        entity.Property(e => e.StatusId).HasColumnName("status_id");

        entity.Property(e => e.Title)
            .IsRequired()
            .HasColumnName("title")
            .HasColumnType("character varying");

        entity.HasOne(d => d.Status)
            .WithMany(p => p.Polls)
            .HasForeignKey(d => d.StatusId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("polls_status_id_fkey");
      });

      modelBuilder.Entity<UserVotes>(entity =>
      {
        entity.ToTable("user_votes");

        entity.Property(e => e.Id)
            .HasColumnName("id")
            .UseIdentityAlwaysColumn();

        entity.Property(e => e.PollId).HasColumnName("poll_id");

        entity.Property(e => e.PollOptionId).HasColumnName("poll_option_id");

        entity.Property(e => e.UserId).HasColumnName("user_id");

        entity.HasOne(d => d.Poll)
            .WithMany(p => p.UserVotes)
            .HasForeignKey(d => d.PollId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("user_votes_poll_id_fkey");

        entity.HasOne(d => d.PollOption)
            .WithMany(p => p.UserVotes)
            .HasForeignKey(d => d.PollOptionId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("user_votes_poll_option_id_fkey");
      });

      modelBuilder.Entity<RefreshToken>(entity =>
      {
        entity.ToTable("refresh_tokens");

        entity.Property(e => e.Id)
            .HasColumnName("id")
            .UseIdentityAlwaysColumn();

        entity.Property(e => e.Expires).HasColumnName("expires");

        entity.Property(e => e.IsActive).HasColumnName("is_active");

        entity.Property(e => e.Token)
            .IsRequired()
            .HasColumnName("token")
            .HasColumnType("character varying");

        entity.Property(e => e.UserId).HasColumnName("user_id");

        entity.HasOne(d => d.User)
            .WithMany(p => p.RefreshTokens)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("refreshtokens_users_fkey");
      });


      //OnModelCreatingPartial(modelBuilder);
    }
    //partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
  }





}
