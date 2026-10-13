using Microsoft.EntityFrameworkCore;
using SmartRecruitment.Domain.Entities;
using SmartRecruitment.Domain.Entities.Auth;
using System;
using System.Collections.Generic;
using HrApplication = SmartRecruitment.Domain.Entities.Application;

namespace SmartRecruitment.Infrastructure.Presistence.Data;

public partial class SmartRecruitmentDbContext : DbContext
{
    public SmartRecruitmentDbContext()
    {
    }

    public SmartRecruitmentDbContext(DbContextOptions<SmartRecruitmentDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
    public DbSet<Hr> Hrs { get; set; }
    public DbSet<Job> Jobs { get; set; }
    public DbSet<Candidate> Candidates { get; set; }
    public DbSet<CandidateUserLink> CandidateUserLinks { get; set; }
    public DbSet<HrApplication> Applications { get; set; }
    public DbSet<Interview> Interviews { get; set; }
    public DbSet<ApplicationStatusHistory> ApplicationStatusHistories { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Only fall back to the hardcoded string when DI did not configure
        // a connection (e.g. design-time tools). Never override AddDbContext.
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server=.;Database=SmartRecruitmentDB;Trusted_Connection=True;TrustServerCertificate=True;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RefreshT__3214EC07E4574E9E");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Token).HasMaxLength(500);
        });
        modelBuilder.Entity<CandidateUserLink>(entity =>
        {
            entity.HasKey(cul => cul.UserId);

            entity.HasIndex(cul => cul.CandidateId).IsUnique();

            entity.HasOne(cul => cul.User)
                  .WithOne()
                  .HasForeignKey<CandidateUserLink>(cul => cul.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(cul => cul.Candidate)
                  .WithOne(c => c.UserLink)
                  .HasForeignKey<CandidateUserLink>(cul => cul.CandidateId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<HrApplication>(entity =>
        {
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.MatchScore).HasPrecision(18, 2);

            entity.HasOne(e => e.Candidate)
                .WithMany(c => c.Applications)
                .HasForeignKey(e => e.CandidateId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Job)
                .WithMany(j => j.Applications)
                .HasForeignKey(e => e.JobId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Job>(entity =>
        {
            entity.HasOne(e => e.Hr)
                .WithMany(h => h.Jobs)
                .HasForeignKey(e => e.Hrid)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Interview>(entity =>
        {
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Result).HasMaxLength(50);
            entity.Property(e => e.Location).HasMaxLength(200);

            entity.HasOne(e => e.Application)
                .WithMany(a => a.Interviews)
                .HasForeignKey(e => e.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ApplicationStatusHistory>(entity =>
        {
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.ChangedBy).HasMaxLength(150);

            entity.HasOne(e => e.Application)
                .WithMany(a => a.StatusHistory)
                .HasForeignKey(e => e.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Candidate>(entity =>
        {
            entity.Property(e => e.Skills).HasMaxLength(1000);
            entity.Property(e => e.CvFileName).HasMaxLength(300);
            entity.Property(e => e.CvContentType).HasMaxLength(150);
            entity.Property(e => e.CvFilePath).HasMaxLength(500);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
