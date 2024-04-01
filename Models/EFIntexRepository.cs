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

        public IEnumerable<Rubric> Rubrics => _context.Rubrics;
        public IEnumerable<Grade> Grades => _context.Grades;
        public IEnumerable<JudgeRoom> JudgeRooms => _context.JudgeRooms;
        public IEnumerable<Permission> Permissions => _context.Permissions;
        public IEnumerable<Presentation> Presentations => _context.Presentations;
        public IEnumerable<RoomSchedule> RoomSchedules => _context.RoomSchedules;
        public IEnumerable<StudentTeam> StudentTeams => _context.StudentTeams;
        public IEnumerable<UserPassword> UserPasswords => _context.UserPasswords;
        public IEnumerable<User> Users => _context.Users;
        public IEnumerable<PeerEvaluationQuestion> PeerEvaluationQuestions => _context.PeerEvaluationQuestions;
        public IEnumerable<PeerEvaluation> PeerEvaluations => _context.PeerEvaluations;
        public IEnumerable<Team> Teams => _context.Teams;
        public IEnumerable<Room> Rooms => _context.Rooms;
        public IEnumerable<TeamSubmission> TeamSubmissions => _context.TeamSubmissions;
        public void AddTeamSubmission(TeamSubmission submission)
        {
            _context.TeamSubmissions.Add(submission);
        }
        public void AddPeerEvaluation(PeerEvaluation evaluation)
        {
            _context.PeerEvaluations.Add(evaluation);
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
       

       
    }


}
