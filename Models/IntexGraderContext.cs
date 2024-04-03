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

    public virtual DbSet<Grade> Grades { get; set; }

    public virtual DbSet<JudgeRoom> JudgeRooms { get; set; }

    public virtual DbSet<PeerEvaluation> PeerEvaluations { get; set; }

    public virtual DbSet<PeerEvaluationQuestion> PeerEvaluationQuestions { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Presentation> Presentations { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<RoomSchedule> RoomSchedules { get; set; }

    public virtual DbSet<Rubric> Rubrics { get; set; }

    public virtual DbSet<StudentTeam> StudentTeams { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    public virtual DbSet<TeamSubmission> TeamSubmissions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserPassword> UserPasswords { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlite("Data Source=intex_grader.sqlite");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Grade>(entity =>
        {
            entity.ToTable("grade");

            entity.Property(e => e.GradeId).HasColumnName("grade_id");
            entity.Property(e => e.AssignmentId).HasColumnName("assignment_id");
            entity.Property(e => e.Comments).HasColumnName("comments");
            entity.Property(e => e.PointsEarned)
                .HasColumnType("NUMERIC")
                .HasColumnName("points_earned");
            entity.Property(e => e.TeamNumber).HasColumnName("team_number");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Assignment).WithMany(p => p.Grades)
                .HasForeignKey(d => d.AssignmentId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.User).WithMany(p => p.Grades)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<JudgeRoom>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.ToTable("judge_room");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");
            entity.Property(e => e.RoomId).HasColumnName("room_id");

            entity.HasOne(d => d.Room).WithMany(p => p.JudgeRooms)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.User).WithOne(p => p.JudgeRoom)
                .HasForeignKey<JudgeRoom>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<PeerEvaluation>(entity =>
        {
            entity.ToTable("peer_evaluation");

            entity.Property(e => e.PeerEvaluationId)
                .ValueGeneratedOnAdd()
                .HasColumnName("peer_evaluation_id");
            entity.Property(e => e.EvaluatorId).HasColumnName("evaluator_id");
            entity.Property(e => e.QuestionId).HasColumnName("question_id");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.SubjectId).HasColumnName("subject_id");

            entity.HasOne(d => d.Evaluator).WithMany(p => p.PeerEvaluationEvaluators)
                .HasForeignKey(d => d.EvaluatorId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.PeerEvaluationNavigation).WithOne(p => p.PeerEvaluation)
                .HasForeignKey<PeerEvaluation>(d => d.PeerEvaluationId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Subject).WithMany(p => p.PeerEvaluationSubjects)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<PeerEvaluationQuestion>(entity =>
        {
            entity.HasKey(e => e.QuestionId);

            entity.ToTable("peer_evaluation_question");

            entity.Property(e => e.QuestionId)
                .ValueGeneratedNever()
                .HasColumnName("question_id");
            entity.Property(e => e.Question).HasColumnName("question");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.PermissionType);

            entity.ToTable("permission");

            entity.Property(e => e.PermissionType)
                .ValueGeneratedNever()
                .HasColumnName("permission_type");
            entity.Property(e => e.PermissionDescription).HasColumnName("permission_description");
        });

        modelBuilder.Entity<Presentation>(entity =>
        {
            entity.ToTable("presentation");

            entity.Property(e => e.PresentationId)
                .ValueGeneratedNever()
                .HasColumnName("presentation_id");
            entity.Property(e => e.Awards).HasColumnName("awards");
            entity.Property(e => e.ClientNeedsNotes).HasColumnName("client_needs_notes");
            entity.Property(e => e.ClientNeedsScore).HasColumnName("client_needs_score");
            entity.Property(e => e.CommunicationNotes).HasColumnName("communication_notes");
            entity.Property(e => e.CommunicationScore).HasColumnName("communication_score");
            entity.Property(e => e.DemonstrationNotes).HasColumnName("demonstration_notes");
            entity.Property(e => e.DemonstrationScore).HasColumnName("demonstration_score");
            entity.Property(e => e.JudgeId)
                .HasColumnType("NUMERIC")
                .HasColumnName("judge_id");
            entity.Property(e => e.TeamNumber).HasColumnName("team_number");
            entity.Property(e => e.TeamRank).HasColumnName("team_rank");

            entity.HasOne(d => d.Judge).WithMany(p => p.Presentations)
                .HasForeignKey(d => d.JudgeId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.TeamNumberNavigation).WithMany(p => p.Presentations)
                .HasForeignKey(d => d.TeamNumber)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.ToTable("room");

            entity.Property(e => e.RoomId)
                .ValueGeneratedNever()
                .HasColumnName("room_id");
            entity.Property(e => e.RoomName).HasColumnName("room_name");
        });

        modelBuilder.Entity<RoomSchedule>(entity =>
        {
            entity.HasKey(e => e.RoomId);

            entity.ToTable("room_schedule");

            entity.Property(e => e.RoomId)
                .ValueGeneratedNever()
                .HasColumnName("room_id");
            entity.Property(e => e.TeamNumber).HasColumnName("team_number");
            entity.Property(e => e.Timeslot).HasColumnName("timeslot");

            entity.HasOne(d => d.Room).WithOne(p => p.RoomSchedule)
                .HasForeignKey<RoomSchedule>(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.TeamNumberNavigation).WithMany(p => p.RoomSchedules)
                .HasForeignKey(d => d.TeamNumber)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Rubric>(entity =>
        {
            entity.HasKey(e => e.AssignmentId);

            entity.ToTable("rubric");

            entity.Property(e => e.AssignmentId).HasColumnName("assignment_id");
            entity.Property(e => e.ClassCode).HasColumnName("class_code");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.InstructorNotes).HasColumnName("instructor_notes");
            entity.Property(e => e.MaxPoints).HasColumnName("max_points");
            entity.Property(e => e.Subcategory).HasColumnName("subcategory");
        });

        modelBuilder.Entity<StudentTeam>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.ToTable("student_team");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");
            entity.Property(e => e.TeamNumber).HasColumnName("team_number");

            entity.HasOne(d => d.User).WithOne(p => p.StudentTeam)
                .HasForeignKey<StudentTeam>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.TeamNumber);

            entity.ToTable("team");

            entity.Property(e => e.TeamNumber)
                .ValueGeneratedNever()
                .HasColumnName("team_number");
        });

        modelBuilder.Entity<TeamSubmission>(entity =>
        {
            entity.HasKey(e => e.TeamNumber);

            entity.ToTable("team_submission");

            entity.Property(e => e.TeamNumber)
                .ValueGeneratedNever()
                .HasColumnName("team_number");
            entity.Property(e => e.GithubLink).HasColumnName("github_link");
            entity.Property(e => e.GoogleDocLink).HasColumnName("google_doc_link");
            entity.Property(e => e.VideoLink).HasColumnName("video_link");

            entity.HasOne(d => d.TeamNumberNavigation).WithOne(p => p.TeamSubmission)
                .HasForeignKey<TeamSubmission>(d => d.TeamNumber)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("user");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.FirstName).HasColumnName("first_name");
            entity.Property(e => e.LastName).HasColumnName("last_name");
            entity.Property(e => e.NetId).HasColumnName("net_id");
            entity.Property(e => e.PermissionType).HasColumnName("permission_type");

            entity.HasOne(d => d.PermissionTypeNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.PermissionType)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<UserPassword>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.ToTable("user_password");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");
            entity.Property(e => e.UserPassword1).HasColumnName("user_password");

            entity.HasOne(d => d.User).WithOne(p => p.UserPassword)
                .HasForeignKey<UserPassword>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
