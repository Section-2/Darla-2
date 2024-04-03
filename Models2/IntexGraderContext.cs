using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Darla.Models2;

public partial class IntexGraderContext : DbContext

{
    public IntexGraderContext()
    {
    }

    public IntexGraderContext(DbContextOptions<IntexGraderContext> options)
        : base(options)
    {
    }

    public virtual DbSet<grade> Grades { get; set; }

    public virtual DbSet<judge_room> JudgeRooms { get; set; }

    public virtual DbSet<peer_evaluation> PeerEvaluations { get; set; }

    public virtual DbSet<peer_evaluation_question> PeerEvaluationQuestions { get; set; }

    public virtual DbSet<permission> Permissions { get; set; }

    public virtual DbSet<presentation> Presentations { get; set; }

    public virtual DbSet<room> Rooms { get; set; }

    public virtual DbSet<room_schedule> RoomSchedules { get; set; }

    public virtual DbSet<rubric> Rubrics { get; set; }

    public virtual DbSet<student_team> StudentTeams { get; set; }

    public virtual DbSet<team> Teams { get; set; }

    public virtual DbSet<team_submission> TeamSubmissions { get; set; }

    public virtual DbSet<user> Users { get; set; }

    public virtual DbSet<user_password> UserPasswords { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=tcp:michaelsdbserver.database.windows.net,1433;Initial Catalog=Michael's Database 1;Persist Security Info=False;User ID=michaelsdbserver;Password=Mikeyey121;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<grade>(entity =>
        {
            entity.ToTable("grade");

            entity.Property(e => e.grade_id).HasColumnName("grade_id");
            entity.Property(e => e.assignment_id).HasColumnName("assignment_id");
            entity.Property(e => e.comments).HasColumnName("comments");
            entity.Property(e => e.points_earned)
                .HasColumnType("NUMERIC")
                .HasColumnName("points_earned");
            entity.Property(e => e.team_number).HasColumnName("team_number");
            entity.Property(e => e.user_id).HasColumnName("user_id");

            entity.HasOne(d => d.assignment).WithMany(p => p.grades)
                .HasForeignKey(d => d.assignment_id)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.user).WithMany(p => p.grades)
                .HasForeignKey(d => d.user_id)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        // modelBuilder.Entity<judge_room>(entity =>
        // {
        //     entity.HasKey(e => e.user_id);
        //
        //     entity.ToTable("judge_room");
        //
        //     entity.Property(e => e.user_id)
        //         .ValueGeneratedNever()
        //         .HasColumnName("user_id");
        //     entity.Property(e => e.room_id).HasColumnName("room_id");
        //
        //     entity.HasOne(d => d.room).WithMany(p => p.judge_rooms)
        //         .HasForeignKey(d => d.room_id)
        //         .OnDelete(DeleteBehavior.ClientSetNull);
        //
        //     entity.HasOne(d => d.user).WithOne(p => p.JudgeRoom)
        //         .HasForeignKey<judge_room>(d => d.user_id)
        //         .OnDelete(DeleteBehavior.ClientSetNull);
        // });
        
        modelBuilder.Entity<judge_room>(entity =>
        {
            entity.HasKey(e => new { e.user_id, e.room_id }); // If judge_room is uniquely identified by both user_id and room_id

            entity.ToTable("judge_room");

            entity.Property(e => e.user_id).HasColumnName("user_id");
            entity.Property(e => e.room_id).HasColumnName("room_id");

            // Configure the many-to-one relationship to room
            entity.HasOne(d => d.room)
                .WithMany(p => p.judge_rooms)
                .HasForeignKey(d => d.room_id)
                .OnDelete(DeleteBehavior.ClientSetNull);

            // Assuming user_id references a user entity not shown, configure that relationship as needed
        });


        // modelBuilder.Entity<peer_evaluation>(entity =>
        // {
        //     entity.ToTable("peer_evaluation");
        //
        //     entity.Property(e => e.peer_evaluation_id)
        //         .ValueGeneratedOnAdd()
        //         .HasColumnName("peer_evaluation_id");
        //     entity.Property(e => e.evaluator_id).HasColumnName("evaluator_id");
        //     entity.Property(e => e.question_id).HasColumnName("question_id");
        //     entity.Property(e => e.rating).HasColumnName("rating");
        //     entity.Property(e => e.subject_id).HasColumnName("subject_id");
        //
        //     entity.HasOne(d => d.evaluator).WithMany(p => p.peer_evaluationevaluators)
        //         .HasForeignKey(d => d.evaluator_id)
        //         .OnDelete(DeleteBehavior.ClientSetNull);
        //
        //     entity.HasOne(d => d.question).WithOne(p => p.peer_evaluations)
        //         .HasForeignKey<peer_evaluation>(d => d.peer_evaluation_id)
        //         .OnDelete(DeleteBehavior.ClientSetNull);
        //
        //     entity.HasOne(d => d.subject).WithMany(p => p.peer_evaluationsubjects)
        //         .HasForeignKey(d => d.subject_id)
        //         .OnDelete(DeleteBehavior.ClientSetNull);
        // });
        
        modelBuilder.Entity<peer_evaluation>(entity =>
        {
            entity.ToTable("peer_evaluation");

            entity.Property(e => e.peer_evaluation_id)
                .ValueGeneratedOnAdd()
                .HasColumnName("peer_evaluation_id");

            entity.Property(e => e.evaluator_id).HasColumnName("evaluator_id");
            entity.Property(e => e.question_id).HasColumnName("question_id");
            entity.Property(e => e.rating).HasColumnName("rating");
            entity.Property(e => e.subject_id).HasColumnName("subject_id");

            // Correct relationship for evaluator
            entity.HasOne(d => d.evaluator).WithMany(p => p.peer_evaluationevaluators)
                .HasForeignKey(d => d.evaluator_id)
                .OnDelete(DeleteBehavior.ClientSetNull);

            // Correct relationship for question (One-to-Many)
            entity.HasOne(d => d.question).WithMany(p => p.peer_evaluations)
                .HasForeignKey(d => d.question_id)
                .OnDelete(DeleteBehavior.ClientSetNull);

            // Correct relationship for subject
            entity.HasOne(d => d.subject).WithMany(p => p.peer_evaluationsubjects)
                .HasForeignKey(d => d.subject_id)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });


        modelBuilder.Entity<peer_evaluation_question>(entity =>
        {
            entity.HasKey(e => e.question_id);

            entity.ToTable("peer_evaluation_question");

            entity.Property(e => e.question_id)
                .ValueGeneratedNever()
                .HasColumnName("question_id");
            entity.Property(e => e.question).HasColumnName("question");
        });

        modelBuilder.Entity<permission>(entity =>
        {
            entity.HasKey(e => e.permission_type);

            entity.ToTable("permission");

            entity.Property(e => e.permission_type)
                .ValueGeneratedNever()
                .HasColumnName("permission_type");
            entity.Property(e => e.permission_description).HasColumnName("permission_description");
        });

        modelBuilder.Entity<presentation>(entity =>
        {
            entity.ToTable("presentation");

            entity.Property(e => e.presentation_id)
                .ValueGeneratedNever()
                .HasColumnName("presentation_id");
            entity.Property(e => e.awards).HasColumnName("awards");
            entity.Property(e => e.client_needs_notes).HasColumnName("client_needs_notes");
            entity.Property(e => e.client_needs_score).HasColumnName("client_needs_score");
            entity.Property(e => e.communication_notes).HasColumnName("communication_notes");
            entity.Property(e => e.communication_score).HasColumnName("communication_score");
            entity.Property(e => e.demonstration_notes).HasColumnName("demonstration_notes");
            entity.Property(e => e.demonstration_score).HasColumnName("demonstration_score");
            entity.Property(e => e.judge_id)
                .HasColumnType("NVARCHAR")
                .HasColumnName("judge_id");
            entity.Property(e => e.team_number).HasColumnName("team_number");
            entity.Property(e => e.team_rank).HasColumnName("team_rank");

            entity.HasOne(d => d.judge).WithMany(p => p.presentations)
                .HasForeignKey(d => d.judge_id)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.team_numberNavigation).WithMany(p => p.presentations)
                .HasForeignKey(d => d.team_number)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<room>(entity =>
        {
            entity.ToTable("room");

            entity.Property(e => e.room_id)
                .ValueGeneratedNever()
                .HasColumnName("room_id");
            entity.Property(e => e.room_name).HasColumnName("room_name");
        });

        // modelBuilder.Entity<room_schedule>(entity =>
        // {
        //     entity.HasKey(e => e.room_id);
        //
        //     entity.ToTable("room_schedule");
        //
        //     entity.Property(e => e.room_id)
        //         .ValueGeneratedNever()
        //         .HasColumnName("room_id");
        //     entity.Property(e => e.team_number).HasColumnName("team_number");
        //     entity.Property(e => e.timeslot).HasColumnName("timeslot");
        //
        //     entity.HasOne(d => d.Room).WithOne(p => p.RoomSchedule)
        //         .HasForeignKey<room_schedule>(d => d.room_id)
        //         .OnDelete(DeleteBehavior.ClientSetNull);
        //
        //     entity.HasOne(d => d.team_numberNavigation).WithMany(p => p.room_schedules)
        //         .HasForeignKey(d => d.team_number)
        //         .OnDelete(DeleteBehavior.ClientSetNull);
        // });
        
        modelBuilder.Entity<room_schedule>(entity =>
        {
            entity.HasKey(e => e.entry_id); // Corrected to entry_id as primary key

            entity.ToTable("room_schedule");

            // Correctly configure room_id as a foreign key, not as a primary key
            entity.Property(e => e.room_id).HasColumnName("room_id");

            entity.Property(e => e.team_number).HasColumnName("team_number");
            entity.Property(e => e.timeslot).HasColumnName("timeslot");

            // Assuming a one-to-many relationship between room_schedule and room
            // This seems to be a mistake given your model. A correction is needed
            // based on the accurate relationship nature between room_schedule and room.

            // Correctly configure the relationship with team, based on your model
            entity.HasOne(d => d.team_numberNavigation)
                .WithMany(p => p.room_schedules)
                .HasForeignKey(d => d.team_number)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });


        modelBuilder.Entity<rubric>(entity =>
        {
            entity.HasKey(e => e.assignment_id);

            entity.ToTable("rubric");

            entity.Property(e => e.assignment_id).HasColumnName("assignment_id");
            entity.Property(e => e.class_code).HasColumnName("class_code");
            entity.Property(e => e.description).HasColumnName("description");
            entity.Property(e => e.instructor_notes).HasColumnName("instructor_notes");
            entity.Property(e => e.max_points).HasColumnName("max_points");
            entity.Property(e => e.subcategory).HasColumnName("subcategory");
        });

        modelBuilder.Entity<student_team>(entity =>
        {
            entity.HasKey(e => e.user_id);

            entity.ToTable("student_team");

            entity.Property(e => e.user_id)
                .ValueGeneratedNever()
                .HasColumnName("user_id");
            entity.Property(e => e.team_number).HasColumnName("team_number");

            entity.HasOne(d => d.user).WithOne(p => p.student_team)
                .HasForeignKey<student_team>(d => d.user_id)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<team>(entity =>
        {
            entity.HasKey(e => e.team_number);

            entity.ToTable("team");

            entity.Property(e => e.team_number)
                .ValueGeneratedNever()
                .HasColumnName("team_number");
        });

        modelBuilder.Entity<team_submission>(entity =>
        {
            entity.HasKey(e => e.team_number);

            entity.ToTable("team_submission");

            entity.Property(e => e.team_number)
                .ValueGeneratedNever()
                .HasColumnName("team_number");
            entity.Property(e => e.github_link).HasColumnName("github_link");
            entity.Property(e => e.google_doc_link).HasColumnName("google_doc_link");
            entity.Property(e => e.video_link).HasColumnName("video_link");

            entity.HasOne(d => d.team_numberNavigation).WithOne(p => p.team_submission)
                .HasForeignKey<team_submission>(d => d.team_number)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<user>(entity =>
        {
            entity.ToTable("user");

            entity.Property(e => e.user_id).HasColumnName("user_id");
            entity.Property(e => e.first_name).HasColumnName("first_name");
            entity.Property(e => e.last_name).HasColumnName("last_name");
            entity.Property(e => e.net_id).HasColumnName("net_id");
            entity.Property(e => e.permission_type).HasColumnName("permission_type");

            entity.HasOne(d => d.permission_typeNavigation).WithMany(p => p.users)
                .HasForeignKey(d => d.permission_type)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        // modelBuilder.Entity<user_password>(entity =>
        // {
        //     entity.HasKey(e => e.user_id);
        //
        //     entity.ToTable("user_password");
        //
        //     entity.Property(e => e.user_id)
        //         .ValueGeneratedNever()
        //         .HasColumnName("user_id");
        //     entity.Property(e => e.user_password1).HasColumnName("user_password");
        //
        //     entity.HasOne(d => d.User).WithOne(p => p.UserPassword)
        //         .HasForeignKey<user_password>(d => d.user_id)
        //         .OnDelete(DeleteBehavior.ClientSetNull);
        // });
        
        // modelBuilder.Entity<user_password>(entity =>
        // {
        //     entity.HasKey(e => e.user_id); // Ensures user_id is the primary key for user_password
        //
        //     entity.ToTable("user_password");
        //
        //     // Configures user_id as the primary key and specifies it's not auto-generated
        //     entity.Property(e => e.user_id)
        //         .ValueGeneratedNever()
        //         .HasColumnName("user_id");
        //
        //     // Configures the user_password1 field and maps it to the "user_password" column
        //     entity.Property(e => e.user_password1)
        //         .HasColumnName("user_password");
        //
        //     // Establishes a one-to-one relationship between user_password and user
        //     entity.HasOne(d => d.User)
        //         .WithOne(p => p.UserPassword)
        //         .HasForeignKey<user_password>(d => d.user_id)
        //         .OnDelete(DeleteBehavior.ClientSetNull);
        // });
        
        modelBuilder.Entity<user_password>(entity =>
        {
            entity.HasKey(e => e.user_id); // Ensures user_id is the primary key for user_password

            entity.ToTable("user_password");

            // Configures user_id as the primary key and specifies it's not auto-generated
            entity.Property(e => e.user_id)
                .ValueGeneratedNever()
                .HasColumnName("user_id");

            // Configures the user_password1 field and maps it to the "user_password" column
            entity.Property(e => e.user_password1)
                .HasColumnName("user_password");

            // Establishes a one-to-one relationship between user_password and user
            entity.HasOne(d => d.user_navigation) // Use the correct navigation property name
                .WithOne(p => p.userpassword_typeNavigation) // Reflects the corrected navigation property in 'user'
                .HasForeignKey<user_password>(d => d.user_id) // FK is in user_password
                .OnDelete(DeleteBehavior.ClientSetNull);
        });



        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
