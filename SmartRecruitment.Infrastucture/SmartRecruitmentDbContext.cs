using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SmartRecruitment.Domain.Entities;

namespace SmartRecruitment.Infrastructure;

public partial class SmartRecruitmentDbContext : DbContext
{
    public SmartRecruitmentDbContext()
    {
    }

    public SmartRecruitmentDbContext(DbContextOptions<SmartRecruitmentDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Domain.Entities.Application> Applications { get; set; }

    public virtual DbSet<Candidate> Candidates { get; set; }

    public virtual DbSet<Hr> Hrs { get; set; }

    public virtual DbSet<Job> Jobs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.;Database=SmartRecruitmentDb;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Domain.Entities.Application>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Applicat__3214EC07422A3E93");

            entity.HasIndex(e => new { e.CandidateId, e.JobId }, "UQ_Applications_Candidate_Job").IsUnique();

            entity.Property(e => e.AppliedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.MatchScore).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.Candidate).WithMany(p => p.Applications)
                .HasForeignKey(d => d.CandidateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Applications_Candidates");

            entity.HasOne(d => d.Job).WithMany(p => p.Applications)
                .HasForeignKey(d => d.JobId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Applications_Jobs");
        });

        modelBuilder.Entity<Candidate>(entity =>
        {
            entity.Property(e => e.CandidateId)
                .ValueGeneratedNever()
                .HasColumnName("candidate_id");
            entity.Property(e => e.Age).HasColumnName("age");
            entity.Property(e => e.Certifications).HasColumnName("certifications");
            entity.Property(e => e.Cgpa).HasColumnName("cgpa");
            entity.Property(e => e.CompanyType)
                .HasMaxLength(50)
                .HasColumnName("company_type");
            entity.Property(e => e.EducationLevel)
                .HasMaxLength(50)
                .HasColumnName("education_level");
            entity.Property(e => e.ExperienceYears).HasColumnName("experience_years");
            entity.Property(e => e.Hackathons).HasColumnName("hackathons");
            entity.Property(e => e.Hired).HasColumnName("hired");
            entity.Property(e => e.Internships).HasColumnName("internships");
            entity.Property(e => e.ProgrammingLanguages).HasColumnName("programming_languages");
            entity.Property(e => e.Projects).HasColumnName("projects");
            entity.Property(e => e.ResearchPapers).HasColumnName("research_papers");
            entity.Property(e => e.ResumeLengthWords).HasColumnName("resume_length_words");
            entity.Property(e => e.SkillsScore).HasColumnName("skills_score");
            entity.Property(e => e.SoftSkillsScore).HasColumnName("soft_skills_score");
            entity.Property(e => e.UniversityTier)
                .HasMaxLength(50)
                .HasColumnName("university_tier");
        });

        modelBuilder.Entity<Hr>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__HRs__3214EC07D864F97E");

            entity.ToTable("HRs");

            entity.Property(e => e.CompanyName).HasMaxLength(150);

            entity.HasOne(d => d.User).WithMany(p => p.Hrs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HRs_Users");
        });

        modelBuilder.Entity<Job>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Jobs__3214EC070CA26EAE");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Hrid).HasColumnName("HRId");
            entity.Property(e => e.RequiredSkills).HasMaxLength(1000);
            entity.Property(e => e.Title).HasMaxLength(150);

            entity.HasOne(d => d.Hr).WithMany(p => p.Jobs)
                .HasForeignKey(d => d.Hrid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Jobs_HRs");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07B0DA18A5");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105347772B9CB").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Role).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
