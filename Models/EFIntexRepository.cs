using Darla.Models2;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace Darla.Models2
{
    public class EFIntexRepository : IIntexRepository
    {
        private IntexGraderContext _context;

        public EFIntexRepository(IntexGraderContext temp)
        {
            _context = temp;
        }

        public List<rubric> Rubrics => _context.Rubrics.ToList();
        public IEnumerable<grade> Grades => _context.Grades;
        public IEnumerable<judge_room> JudgeRooms => _context.JudgeRooms;
        public IEnumerable<permission> Permissions => _context.Permissions;
        public IEnumerable<presentation> Presentations =>
            _context.Presentations.Include(x => x.judge).Include(x => x.team_numberNavigation);
        public void AddPresentationScore(presentation presentation)
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
                var presToUpdate = _context.Presentations.SingleOrDefault(x => x.team_number == teamNumber);

                if (presToUpdate == null)
                {
                    // If no presentation exists, create a new one.
                    presToUpdate = new presentation()
                    {
                        team_number = teamNumber, // Make sure to set the TeamNumber too.
                        team_rank = rank
                    };
                    // Add the new presentation to the context.
                    _context.Presentations.Add(presToUpdate);
                }
                else
                {
                    // If a presentation is found, just update its rank.
                    presToUpdate.team_rank = rank;
                }
            }

            // Save changes after processing all team ranks.
            _context.SaveChanges();
        }

        public IEnumerable<room_schedule> RoomSchedules => _context.RoomSchedules;
        public IQueryable<room_schedule> RoomSchedulesWithRooms => _context.RoomSchedules.Include(rs => rs.judge_rooms).ThenInclude(room => room.room);
        public IEnumerable<student_team> StudentTeams => _context.StudentTeams.ToList();
        public IEnumerable<user_password> UserPasswords => _context.UserPasswords;
        public IEnumerable<user> Users => _context.Users;
        public IEnumerable<peer_evaluation_question> PeerEvaluationQuestions => _context.PeerEvaluationQuestions;
        public IEnumerable<peer_evaluation> PeerEvaluations => _context.PeerEvaluations;
        public IEnumerable<team> Teams => _context.Teams;
        public IEnumerable<room> Rooms => _context.Rooms;
        public IEnumerable<team_submission> TeamSubmissions => _context.TeamSubmissions;
        public void AddTeamSubmission(team_submission submission)
        {
            _context.TeamSubmissions.Add(submission);
        }
        public void AddPeerEvaluation(peer_evaluation evaluation)
        {
            _context.PeerEvaluations.Add(evaluation);
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }


        public IQueryable<student_team> GetQueryableStudentTeams()
        {
            return _context.StudentTeams;
        }


        public IQueryable<room_schedule> GetRoomSchedulesByRoomId(int roomId)
        {
            return _context.RoomSchedules.AsNoTracking().Include(rs => rs.judge_rooms).ThenInclude(room => room.room)
                .Where(rs => rs.room_id == roomId);

        }
        public void AddRubric(rubric rubric)
        {
            _context.Rubrics.Add(rubric);
            _context.SaveChanges();
        }

        public void DeleteRubric(rubric rubric)
        {
            _context.Rubrics.Remove(rubric);
            _context.SaveChanges();
        }

        public void EditRubric(rubric rubric)
        {
            _context.Rubrics.Update(rubric);
            _context.SaveChanges();
        }

        public void EditJudge(user updatedInfo)
        {
            _context.Update(updatedInfo);
            _context.SaveChanges();
        }

        public void DeleteJudge(user removedUser)
        {
            _context.Users.Remove(removedUser);
            _context.SaveChanges();
        }
        public void AddJudge(user response)
        {
            _context.Users.Add(response);
            _context.SaveChanges();
        }

        public void EditTA(user updatedTAInfo)
        {
            _context.Update(updatedTAInfo);
            _context.SaveChanges();
        }

        public void DeleteTA(user removedTAUser)
        {
            _context.Users.Remove(removedTAUser);
            _context.SaveChanges();
        }
        public void AddTA(user addTAResponse)
        {
            _context.Users.Add(addTAResponse);
            _context.SaveChanges();
        }

        public async Task<List<PeerEvaluationViewModel>> GetPeerEvaluationInfo()
        {
            var peerEvaluations = _context.PeerEvaluations
                .Include(pe => pe.subject)
                .ThenInclude(st => st.user)
                .AsNoTracking();
            var groupedEvaluations = await peerEvaluations
                .GroupBy(pe => pe.subject.team_number)
                .Select(group => new PeerEvaluationViewModel
                {
                    TeamNumber = group.Key,
                    Members = group
                        .GroupBy(g => g.subject_id)
                        .Select(m => new MemberEvaluationInfo
                        {
                            UserId = m.Key,
                            FirstName = m.First().subject.user.FirstName,
                            LastName = m.First().subject.user.LastName,
                            TotalScore = m.Sum(x => x.rating)
                        }).ToList()
                }).ToListAsync();
            return groupedEvaluations;
        }

    }
}
