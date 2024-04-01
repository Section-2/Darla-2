namespace Darla.Models
{
    public class IntexViewModel
    {
        public IQueryable<Grade>? Grades { get; set; }
        public IQueryable<Rubric>? Rubrics { get; set; }
        public IQueryable<JudgeRoom> JudgeRooms { get; set; }
        public IQueryable<PeerEvaluation> PeerEvaluations { get; set; }
        public IQueryable<PeerEvaluationQuestion> PeerEvaluationsQuestions { get; set;}
        public IQueryable<Permission> Permissions { get; set; }
        public IQueryable<Presentation> Presentations { get; set; }
        public IQueryable<Room> Rooms { get; set; }
        public IQueryable<RoomSchedule> RoomSchedules { get; set; }
        public IQueryable<StudentTeam> StudentTeams { get; set; }
        public IQueryable<Team> Teams { get; set; }
        public IQueryable<TeamSubmission> TeamSubmissions { get; set; }
        public IQueryable<User> Users { get; set; }
        public IQueryable<UserPassword> UserPasswords { get; set; }


    }
}
