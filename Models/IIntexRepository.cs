namespace Darla.Models
{
    public interface IIntexRepository
    {
        List<Rubric> Rubrics { get; }
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
        public void AddRubric(Rubric rubric);
        public void EditRubric(Rubric rubric);

        public void DeleteRubric(Rubric rubric);
        public void AddAwards(List<Presentation> presentation);
        public void EditAwards(Presentation presentation);

        public void EditJudge(User updatedInfo);
        public void DeleteJudge(User removedUser);
        public void AddJudge(User response);
        public void EditTA(User updatedTAInfo);
        public void DeleteTA(User removedTAUser);
        public void AddTA(User addTAResponse);
        IQueryable<RoomSchedule> GetRoomSchedulesByRoomId(int roomId);

        Task<List<PeerEvaluationViewModel>> GetPeerEvaluationInfo();


    }
}
