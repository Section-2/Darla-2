


namespace Darla.Models
{
    public interface IIntexRepository
    {
        IEnumerable<Rubric> Rubrics { get; }
        IEnumerable<Grade> Grades { get; }
        IEnumerable<JudgeRoom> JudgeRooms { get; }
        IEnumerable<Permission> Permissions { get; }
        IEnumerable<Presentation> Presentations { get; }
        IEnumerable<RoomSchedule> RoomSchedules { get; }
        IEnumerable<StudentTeam> StudentTeams { get; }
        IEnumerable<UserPassword> UserPasswords { get; }
        IEnumerable<User> Users { get; }
        IEnumerable<PeerEvaluationQuestion> PeerEvaluationQuestions { get; }
        IEnumerable<PeerEvaluation> PeerEvaluations { get; }
        IEnumerable<Team> Teams { get; }
        IEnumerable<Room> Rooms { get; }
        IEnumerable<TeamSubmission> TeamSubmissions { get; }

        void AddPeerEvaluation(PeerEvaluation evaluation);
        void SaveChanges();
        Task SaveChangesAsync();
    }
}
