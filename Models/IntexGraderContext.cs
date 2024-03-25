using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Darla.Models;

public partial class IntexGraderContext : DbContext
{
    public IntexGraderContext()
    {
    }

    public IntexGraderContext(DbContextOptions<IntexGraderContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Assignment> Assignments { get; set; }

    public virtual DbSet<Grade> Grades { get; set; }

    public virtual DbSet<JudgeRoom> JudgeRooms { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<PresentationSurvey> PresentationSurveys { get; set; }

    public virtual DbSet<RoomSchedule> RoomSchedules { get; set; }

    public virtual DbSet<StudentTeam> StudentTeams { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserPassword> UserPasswords { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlite("Data Source=intex_grader.sqlite");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.ToTable("assignments");

            entity.Property(e => e.AssignmentId)
                .ValueGeneratedNever()
                .HasColumnType("INT")
                .HasColumnName("assignment_id");
            entity.Property(e => e.AssignmentDescription)
                .HasColumnType("VARCHAR(45)")
                .HasColumnName("assignment_description");
            entity.Property(e => e.ClassCode)
                .HasColumnType("INT")
                .HasColumnName("class_code");
            entity.Property(e => e.MaxPoints)
                .HasColumnType("INT")
                .HasColumnName("max_points");
            entity.Property(e => e.Subcategory)
                .HasColumnType("VARCHAR(45)")
                .HasColumnName("subcategory");
        });

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => new { e.AssignmentId, e.StudentGradesNetId });

            entity.ToTable("grades");

            entity.Property(e => e.AssignmentId)
                .HasColumnType("INT")
                .HasColumnName("assignment_id");
            entity.Property(e => e.StudentGradesNetId)
                .HasColumnType("INT")
                .HasColumnName("student_grades_net_id");
            entity.Property(e => e.ClassCode)
                .HasColumnType("INT")
                .HasColumnName("class_code");
            entity.Property(e => e.Comments)
                .HasColumnType("VARCHAR(45)")
                .HasColumnName("comments");
            entity.Property(e => e.Points)
                .HasColumnType("INT")
                .HasColumnName("points");

            entity.HasOne(d => d.Assignment).WithMany(p => p.Grades)
                .HasForeignKey(d => d.AssignmentId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.StudentGradesNet).WithMany(p => p.Grades)
                .HasForeignKey(d => d.StudentGradesNetId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<JudgeRoom>(entity =>
        {
            entity.HasKey(e => new { e.JudgeCode, e.RoomCode });

            entity.ToTable("judge_rooms");

            entity.Property(e => e.JudgeCode)
                .HasColumnType("INT")
                .HasColumnName("judge_code");
            entity.Property(e => e.RoomCode)
                .HasColumnType("VARCHAR(45)")
                .HasColumnName("room_code");

            entity.HasOne(d => d.JudgeCodeNavigation).WithMany(p => p.JudgeRooms)
                .HasForeignKey(d => d.JudgeCode)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.PermissionType);

            entity.ToTable("permissions");

            entity.Property(e => e.PermissionType)
                .ValueGeneratedNever()
                .HasColumnType("INT")
                .HasColumnName("permission_type");
            entity.Property(e => e.PermissionDescription)
                .HasColumnType("VARCHAR(45)")
                .HasColumnName("permission_description");
        });

        modelBuilder.Entity<PresentationSurvey>(entity =>
        {
            entity.HasKey(e => new { e.JudgeId, e.TeamNumber });

            entity.ToTable("presentation_surveys");

            entity.Property(e => e.JudgeId)
                .HasColumnType("INT")
                .HasColumnName("judge_id");
            entity.Property(e => e.TeamNumber)
                .HasColumnType("INT")
                .HasColumnName("team_number");
            entity.Property(e => e.ClientNeedsNotes)
                .HasColumnType("VARCHAR(45)")
                .HasColumnName("client_needs_notes");
            entity.Property(e => e.ClientNeedsScore)
                .HasColumnType("VARCHAR(45)")
                .HasColumnName("client_needs_score");
            entity.Property(e => e.CommunicationNotes)
                .HasColumnType("VARCHAR(45)")
                .HasColumnName("communication_notes");
            entity.Property(e => e.CommunicationScore)
                .HasColumnType("VARCHAR(45)")
                .HasColumnName("communication_score");
            entity.Property(e => e.ConsideredAwards)
                .HasColumnType("VARCHAR(45)")
                .HasColumnName("considered_awards");
            entity.Property(e => e.DemonstrationNotes)
                .HasColumnType("VARCHAR(45)")
                .HasColumnName("demonstration_notes");
            entity.Property(e => e.DemonstrationScore)
                .HasColumnType("VARCHAR(45)")
                .HasColumnName("demonstration_score");
            entity.Property(e => e.TeamRank)
                .HasColumnType("VARCHAR(45)")
                .HasColumnName("team_rank");
        });

        modelBuilder.Entity<RoomSchedule>(entity =>
        {
            entity.HasKey(e => new { e.RoomCode, e.Timeslot });

            entity.ToTable("room_schedule");

            entity.Property(e => e.RoomCode)
                .HasColumnType("VARCHAR(45)")
                .HasColumnName("room_code");
            entity.Property(e => e.Timeslot)
                .HasColumnType("VARCHAR(45)")
                .HasColumnName("timeslot");
            entity.Property(e => e.TeamNumber)
                .HasColumnType("VARCHAR(45)")
                .HasColumnName("team_number");
            entity.Property(e => e.TeamSection)
                .HasColumnType("VARCHAR(45)")
                .HasColumnName("team_section");
        });

        modelBuilder.Entity<StudentTeam>(entity =>
        {
            entity.HasKey(e => new { e.StudentNetId, e.TeamNumber });

            entity.ToTable("student_teams");

            entity.Property(e => e.StudentNetId)
                .HasColumnType("INT")
                .HasColumnName("student_net_id");
            entity.Property(e => e.TeamNumber)
                .HasColumnType("INT")
                .HasColumnName("team_number");

            entity.HasOne(d => d.StudentNet).WithMany(p => p.StudentTeams)
                .HasForeignKey(d => d.StudentNetId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.NetId);

            entity.ToTable("users");

            entity.Property(e => e.NetId)
                .ValueGeneratedNever()
                .HasColumnType("INT")
                .HasColumnName("net_id");
            entity.Property(e => e.FirstName)
                .HasColumnType("VARCHAR(45)")
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasColumnType("VARCHAR(45)")
                .HasColumnName("last_name");
            entity.Property(e => e.PermissionsType)
                .HasColumnType("INT")
                .HasColumnName("permissions_type");

            entity.HasOne(d => d.PermissionsTypeNavigation).WithMany(p => p.Users).HasForeignKey(d => d.PermissionsType);
        });

        modelBuilder.Entity<UserPassword>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.ToTable("user_passwords");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnType("INT")
                .HasColumnName("user_id");
            entity.Property(e => e.UserPassword1)
                .HasColumnType("VARCHAR(45)")
                .HasColumnName("user_password");

            entity.HasOne(d => d.User).WithOne(p => p.UserPassword)
                .HasForeignKey<UserPassword>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
