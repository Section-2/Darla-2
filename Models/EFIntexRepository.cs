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
        public void UpdateTeamRanks(Dictionary<int, int> teamRanks)
        {
            foreach (var teamRank in teamRanks)
            {
                var teamNumber = teamRank.Key;
                var rank = teamRank.Value;

                // Attempt to find an existing presentation for the team.
                var presToUpdate = _context.Presentations.SingleOrDefault(x => x.TeamNumber == teamNumber);

                if (presToUpdate == null)
                {
                    // If no presentation exists, create a new one.
                    presToUpdate = new Presentation()
                    {
                        TeamNumber = teamNumber, // Make sure to set the TeamNumber too.
                        TeamRank = rank
                    };
                    // Add the new presentation to the context.
                    _context.Presentations.Add(presToUpdate);
                }
                else
                {
                    // If a presentation is found, just update its rank.
                    presToUpdate.TeamRank = rank;
                }
            }

            // Save changes after processing all team ranks.
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


        public IQueryable<StudentTeam> GetQueryableStudentTeams()
        {
            return _context.StudentTeams;
        }


        public IQueryable<RoomSchedule> GetRoomSchedulesByRoomId(int roomId)
        {
            return _context.RoomSchedules.AsNoTracking().Include(rs => rs.Room)
                .Where(rs => rs.RoomId == roomId);

        }
        public void AddRubric()
        {
            var toAdd = _context.Rubrics
                .FirstOrDefault();

            _context.Rubrics.Add(toAdd);
            _context.SaveChanges();
        }

        public void DeleteRubric(int assignmentId)
        {
            var toDelete = _context.Rubrics
                .Where(x => x.AssignmentId == assignmentId)
                .FirstOrDefault();

            _context.Rubrics.Remove(toDelete);
            _context.SaveChanges();
        }

        public void EditRubric(Rubric rubric)
        {
            _context.Rubrics.Update(rubric);
            _context.SaveChanges();
        }

        public void EditJudge(User updatedInfo)
        {
            _context.Update(updatedInfo);
            _context.SaveChanges();
        }

        public void DeleteJudge(User removedUser)
        {
            _context.Users.Remove(removedUser);
            _context.SaveChanges();
        }
    }
}
