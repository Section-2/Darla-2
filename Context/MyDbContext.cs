using System;
using System.Collections.Generic;
using Darla.Models;
using Microsoft.EntityFrameworkCore;

namespace Darla.Context;

public partial class MyDbContext : DbContext
{
    public MyDbContext()
    {
    }

    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }


    public virtual DbSet<Grade> grades { get; set; }

    public virtual DbSet<JudgeRoom> judge_rooms { get; set; }

    public virtual DbSet<PeerEvaluation> peer_evaluations { get; set; }

    public virtual DbSet<PeerEvaluationQuestion> peer_evaluation_questions { get; set; }

    public virtual DbSet<Permission> permissions { get; set; }

    public virtual DbSet<Presentation> presentations { get; set; }

    public virtual DbSet<Room> rooms { get; set; }

    public virtual DbSet<RoomSchedule> room_schedules { get; set; }

    public virtual DbSet<Rubric> rubrics { get; set; }

    public virtual DbSet<StudentTeam> student_teams { get; set; }

    public virtual DbSet<Team> teams { get; set; }

    public virtual DbSet<TeamSubmission> team_submissions { get; set; }

    public virtual DbSet<User> users { get; set; }

    public virtual DbSet<UserPassword> user_passwords { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=tcp:michaelsdbserver.database.windows.net,1433;Initial Catalog=MichaelsDatabase1;Persist Security Info=False;User ID=michaelsdbserver;Password=Mikeyey121;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex").IsUnique();

            entity.Property(e => e.ConcurrencyStamp).HasMaxLength(450);
            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetRoleClaim>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

            entity.Property(e => e.ClaimType).HasMaxLength(450);
            entity.Property(e => e.ClaimValue).HasMaxLength(450);

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex").IsUnique();

            entity.Property(e => e.ConcurrencyStamp).HasMaxLength(450);
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.FirstName).HasMaxLength(256);
            entity.Property(e => e.LastName).HasMaxLength(256);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.PasswordHash).HasMaxLength(450);
            entity.Property(e => e.PhoneNumber).HasMaxLength(450);
            entity.Property(e => e.SecurityStamp).HasMaxLength(450);
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("AspNetUserRoles");
                        j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                    });
        });

        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

            entity.Property(e => e.ClaimType).HasMaxLength(450);
            entity.Property(e => e.ClaimValue).HasMaxLength(450);

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

            entity.Property(e => e.ProviderDisplayName).HasMaxLength(450);

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

            entity.Property(e => e.Value).HasMaxLength(450);

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens).HasForeignKey(d => d.UserId);
        });




        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => e.GradeId);

            entity.ToTable("grade");

            entity.Property(e => e.Comments).HasMaxLength(450);
            entity.Property(e => e.PointsEarned).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UserId).HasMaxLength(450);

            entity.HasOne(d => d.Assignment).WithMany(p => p.grades)
                .HasForeignKey(d => d.assignment_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_grade_assignment");

            entity.HasOne(d => d.TeamNumber).WithMany(p => p.grades)
                .HasForeignKey(d => d.team_number)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_grade_team");

            entity.HasOne(d => d.User).WithMany(p => p.Grades)
                .HasForeignKey(d => d.user_id)
                .HasConstraintName("FK_grade_student_team");
        });

        modelBuilder.Entity<JudgeRoom>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.ToTable("judge_room");

            entity.HasOne(d => d.Room).WithMany(p => p.JudgeRooms)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_judge_room_room_schedule");
        });

        modelBuilder.Entity<PeerEvaluation>(entity =>
        {
            entity.HasKey(e => e.PeerEvaluationId);

            entity.ToTable("peer_evaluation");

            entity.Property(e => e.Comments).HasMaxLength(450);
            entity.Property(e => e.EvaluatorId).HasMaxLength(450);
            entity.Property(e => e.SubjectId).HasMaxLength(450);

            entity.HasOne(d => d.evaluator).WithMany(p => p.peer_evaluationevaluators)
                .HasForeignKey(d => d.evaluator_id)
                .HasConstraintName("FK_peer_evaluator_student_team_evaluator");

            entity.HasOne(d => d.question).WithMany(p => p.peer_evaluations)
                .HasForeignKey(d => d.question_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_peer_evaluation_peer_evaluation_question");

            entity.HasOne(d => d.subject).WithMany(p => p.peer_evaluationsubjects)
                .HasForeignKey(d => d.subject_id)
                .HasConstraintName("FK_peer_subject_student_team_subject");
        });

        modelBuilder.Entity<peer_evaluation_question>(entity =>
        {
            entity.HasKey(e => e.question_id);

            entity.ToTable("peer_evaluation_question");

            entity.Property(e => e.question_id).ValueGeneratedNever();
            entity.Property(e => e.question).HasMaxLength(450);
        });

        modelBuilder.Entity<permission>(entity =>
        {
            entity.HasKey(e => e.permission_type);

            entity.ToTable("permission");

            entity.Property(e => e.permission_type).ValueGeneratedNever();
            entity.Property(e => e.permission_description).HasMaxLength(450);
        });

        modelBuilder.Entity<presentation>(entity =>
        {
            entity.HasKey(e => e.presentation_id);

            entity.ToTable("presentation");

            entity.Property(e => e.awards).HasMaxLength(450);
            entity.Property(e => e.client_needs_notes).HasMaxLength(450);
            entity.Property(e => e.communication_notes).HasMaxLength(450);
            entity.Property(e => e.demonstration_notes).HasMaxLength(450);
            entity.Property(e => e.judge_id).HasMaxLength(450);

            entity.HasOne(d => d.judge).WithMany(p => p.presentations)
                .HasForeignKey(d => d.judge_id)
                .HasConstraintName("FK_presentation_judge_room");

            entity.HasOne(d => d.team_numberNavigation).WithMany(p => p.presentations)
                .HasForeignKey(d => d.team_number)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_presentation_team");
        });

        modelBuilder.Entity<room>(entity =>
        {
            entity.HasKey(e => e.room_id);

            entity.ToTable("room");

            entity.Property(e => e.room_id).ValueGeneratedNever();
            entity.Property(e => e.room_name).HasMaxLength(450);
        });

        modelBuilder.Entity<room_schedule>(entity =>
        {
            entity.HasKey(e => e.entry_id);

            entity.ToTable("room_schedule");

            entity.Property(e => e.entry_id).ValueGeneratedNever();
            entity.Property(e => e.timeslot).HasMaxLength(450);

            entity.HasOne(d => d.entry).WithOne(p => p.room_schedule)
                .HasForeignKey<room_schedule>(d => d.entry_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_room_schedule_room");

            entity.HasOne(d => d.team_numberNavigation).WithMany(p => p.room_schedules)
                .HasForeignKey(d => d.team_number)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_room_schedule_team");
        });

        modelBuilder.Entity<rubric>(entity =>
        {
            entity.HasKey(e => e.assignment_id);

            entity.ToTable("rubric");

            entity.Property(e => e.description).HasMaxLength(450);
            entity.Property(e => e.instructor_notes).HasMaxLength(450);
            entity.Property(e => e.subcategory).HasMaxLength(450);
        });

        modelBuilder.Entity<student_team>(entity =>
        {
            entity.HasKey(e => e.user_id);

            entity.ToTable("student_team");

            entity.HasOne(d => d.team_numberNavigation).WithMany(p => p.student_teams)
                .HasForeignKey(d => d.team_number)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_student_team_team");

            entity.HasOne(d => d.user).WithOne(p => p.student_team)
                .HasForeignKey<student_team>(d => d.user_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_student_team_AspNetUsers");
        });

        modelBuilder.Entity<team>(entity =>
        {
            entity.HasKey(e => e.team_number);

            entity.ToTable("team");

            entity.Property(e => e.team_number).ValueGeneratedNever();
        });

        modelBuilder.Entity<team_submission>(entity =>
        {
            entity.HasKey(e => e.team_number);

            entity.ToTable("team_submission");

            entity.Property(e => e.team_number).ValueGeneratedNever();
            entity.Property(e => e.github_link).HasMaxLength(450);
            entity.Property(e => e.google_doc_link).HasMaxLength(450);
            entity.Property(e => e.video_link).HasMaxLength(450);

            entity.HasOne(d => d.team_numberNavigation).WithOne(p => p.team_submission)
                .HasForeignKey<team_submission>(d => d.team_number)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_team_submission_team");
        });

        modelBuilder.Entity<user>(entity =>
        {
            entity.HasKey(e => e.user_id);

            entity.ToTable("user");

            entity.Property(e => e.first_name).HasMaxLength(450);
            entity.Property(e => e.last_name).HasMaxLength(450);
            entity.Property(e => e.net_id).HasMaxLength(450);

            entity.HasOne(d => d.permission_typeNavigation).WithMany(p => p.users)
                .HasForeignKey(d => d.permission_type)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_user_permission");
        });

        modelBuilder.Entity<user_password>(entity =>
        {
            entity.HasKey(e => e.user_id);

            entity.ToTable("user_password");

            entity.Property(e => e.user_id).ValueGeneratedNever();
            entity.Property(e => e.user_password1)
                .HasMaxLength(450)
                .HasColumnName("user_password");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
