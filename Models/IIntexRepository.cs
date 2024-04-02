namespace Darla.Models
{
    public interface IIntexRepository
    {
        IEnumerable<Rubric> Rubrics { get; }
        IEnumerable<Grade> Grades { get; }
        IEnumerable<JudgeRoom> JudgeRooms { get; }
        IEnumerable<Permission> Permissions { get; }
        IEnumerable<Presentation> Presentations { get; }
        public void AddPresentationScore(Presentation presentation);
        public void UpdateTeamRanks(Dictionary<int, int> teamRanks);
        IEnumerable<RoomSchedule> RoomSchedules { get; }
        IQueryable<RoomSchedule> RoomSchedulesWithRooms { get; }
        IEnumerable<StudentTeam> StudentTeams { get; }
        IEnumerable<UserPassword> UserPasswords { get; }
        IEnumerable<User> Users { get; }
        IEnumerable<PeerEvaluationQuestion> PeerEvaluationQuestions { get; }
        IEnumerable<PeerEvaluation> PeerEvaluations { get; }
        IEnumerable<Team> Teams { get; }
        IEnumerable<Room> Rooms { get; }
        IEnumerable<TeamSubmission> TeamSubmissions { get; }
        void AddTeamSubmission(TeamSubmission submission);
        void AddPeerEvaluation(PeerEvaluation evaluation);
        Task SaveChangesAsync();
        IQueryable<StudentTeam> GetQueryableStudentTeams();
        public void AddRubric();
        public void EditRubric(Rubric rubric);
        public void DeleteRubric(int assignmentId);

        public void EditJudge(User updatedInfo);
        public void DeleteJudge(User removedUser);
        IQueryable<RoomSchedule> GetRoomSchedulesByRoomId(int roomId);


    }
}
