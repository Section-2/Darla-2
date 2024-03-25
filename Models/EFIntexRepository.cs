namespace Darla.Models
{
    public class EFIntexRepository : IIntexRepository
    {
        private IntexGraderContext _context;
        public EFIntexRepository(IntexGraderContext temp)
        {
            _context = temp;
        }
        public IEnumerable<Assignment> Assignments => _context.Assignments;
        public IEnumerable<Grade> Grades => _context.Grades;
        public IEnumerable<JudgeRoom> JudgeRooms => _context.JudgeRooms;
        public IEnumerable<Permission> Permissions => _context.Permissions;
        public IEnumerable<PresentationSurvey> PresentationSurveys => _context.PresentationSurveys;
        public IEnumerable<RoomSchedule> RoomSchedules => _context.RoomSchedules;
        public IEnumerable<StudentTeam> StudentTeams => _context.StudentTeams;
        public IEnumerable<UserPassword> UserPasswords => _context.UserPasswords;
        public IEnumerable<User> Users => _context.Users;

    }
}
