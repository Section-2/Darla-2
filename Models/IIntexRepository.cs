using Darla.Models;
using Darla.Models2;

namespace Darla.Models2
{
    public interface IIntexRepository
    {
        List<rubric> Rubrics { get; }
        IEnumerable<grade> Grades { get; }
        IEnumerable<judge_room> JudgeRooms { get; }
        IEnumerable<permission> Permissions { get; }
        IEnumerable<presentation> Presentations { get; }
        public void AddPresentationScore(presentation presentation);
        public void UpdateTeamRanks(Dictionary<int, int> teamRanks);
        IEnumerable<room_schedule> RoomSchedules { get; }
        IQueryable<room_schedule> RoomSchedulesWithRooms { get; }
        IEnumerable<student_team> StudentTeams { get; }
        IEnumerable<user_password> UserPasswords { get; }
        IEnumerable<user> Users { get; }
        IEnumerable<peer_evaluation_question> PeerEvaluationQuestions { get; }
        IEnumerable<peer_evaluation> PeerEvaluations { get; }
        IEnumerable<team> Teams { get; }
        IEnumerable<room> Rooms { get; }
        IEnumerable<team_submission> TeamSubmissions { get; }
        void AddTeamSubmission(team_submission submission);
        void AddPeerEvaluation(peer_evaluation evaluation);
        Task SaveChangesAsync();
        IQueryable<student_team> GetQueryableStudentTeams();
        public void AddRubric(rubric rubric);
        public void EditRubric(rubric rubric);
        public void DeleteRubric(rubric rubric);

        public void EditJudge(user updatedInfo);
        public void DeleteJudge(user removedUser);
        public void AddJudge(user response);
        public void EditTA(user updatedTAInfo);
        public void DeleteTA(user removedTAUser);
        public void AddTA(user addTAResponse);
        IQueryable<room_schedule> GetRoomSchedulesByRoomId(int roomId);

        Task<List<PeerEvaluationViewModel>> GetPeerEvaluationInfo();


    }
}
