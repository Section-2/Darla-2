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

        public IEnumerable<Presentation> Presentations =>
            _context.Presentations.Include(x => x.Judge).Include(x => x.TeamNumberNavigation);
        public void AddPresentationScore(Presentation presentation)
        {
            _context.Update(presentation);
            _context.SaveChanges();
        }

        public void UpdateTeamRanks(Dictionary<int, int> teamRanks)
        {
            foreach (var teamRank in teamRanks)
            {
                var teamNumber = teamRank.Key;
                var rank = teamRank.Value;

                var presToUpdate = _context.Presentations.SingleOrDefault(x => x.TeamNumber == teamNumber);

                if (presToUpdate == null)
                {
                    throw new Exception($"Team not found for team number: {teamNumber}.");
                }
                // Update the property
                presToUpdate.TeamRank = rank;
            }

            // Save changes outside the loop
            _context.SaveChanges();
        }


        public IEnumerable<RoomSchedule> RoomSchedules => _context.RoomSchedules;
        public IQueryable<RoomSchedule> RoomSchedulesWithRooms => _context.RoomSchedules.Include(rs => rs.Room);
        public IEnumerable<StudentTeam> StudentTeams => _context.StudentTeams;
        public IEnumerable<UserPassword> UserPasswords => _context.UserPasswords;
        public IEnumerable<User> Users => _context.Users;
        public IEnumerable<PeerEvaluationQuestion> PeerEvaluationQuestions => _context.PeerEvaluationQuestions;
        public IEnumerable<PeerEvaluation> PeerEvaluations => _context.PeerEvaluations;
        public IEnumerable<Team> Teams => _context.Teams;
        public IEnumerable<Room> Rooms => _context.Rooms;
        public IEnumerable<TeamSubmission> TeamSubmissions => _context.TeamSubmissions;


    }


}
