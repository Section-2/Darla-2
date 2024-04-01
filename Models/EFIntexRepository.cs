using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace Darla.Models
{
    public class EFIntexRepository : IIntexRepository
    {
        private IntexGraderContext _context;

        public EFIntexRepository(IntexGraderContext temp)
        {
            _context = temp;
        }

        public IEnumerable<Rubric> Rubrics => _context.Rubrics.ToList();
        public IEnumerable<Grade> Grades => _context.Grades;
        public IEnumerable<JudgeRoom> JudgeRooms => _context.JudgeRooms;
        public IEnumerable<Permission> Permissions => _context.Permissions;

        public IEnumerable<Presentation> Presentations =>
            _context.Presentations.Include(x => x.Judge).Include(x => x.TeamNumberNavigation);
        public void AddPresentationScore(Presentation presentation)
        {
            _context.Update(presentation);
            _context.SaveChanges();
        }
        public IEnumerable<RoomSchedule> RoomSchedules => _context.RoomSchedules;
        public IEnumerable<StudentTeam> StudentTeams => _context.StudentTeams;
        public IEnumerable<UserPassword> UserPasswords => _context.UserPasswords;
        public IEnumerable<User> Users => _context.Users;
        public IEnumerable<PeerEvaluationQuestion> PeerEvaluationQuestions => _context.PeerEvaluationQuestions;
        public IEnumerable<PeerEvaluation> PeerEvaluations => _context.PeerEvaluations;
        public IEnumerable<Team> Teams => _context.Teams;
        public IEnumerable<Room> Rooms => _context.Rooms;
        public IEnumerable<TeamSubmission> TeamSubmissions => _context.TeamSubmissions;

        public IQueryable<StudentTeam> GetQueryableStudentTeams()
        {
            return _context.StudentTeams;
        }

        public void AddRubric(Rubric rubric)
        {
            _context.Rubrics.Add(rubric);
            _context.SaveChanges();
        }

        public void DeleteRubric(Rubric rubric)
        {
            _context.Rubrics.Remove(rubric);
            _context.SaveChanges();

        }

        public void EditRubric(Rubric rubric)
        {
            _context.Rubrics.Update(rubric);
            _context.SaveChanges();
        }
    }


}
