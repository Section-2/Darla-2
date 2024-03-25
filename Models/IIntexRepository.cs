namespace Darla.Models
{
    public interface IIntexRepository
    {
        IEnumerable<Assignment> Assignments { get; }
        IEnumerable<Grade> Grades { get; }
        IEnumerable<JudgeRoom> JudgeRooms { get; }
        IEnumerable<Permission> Permissions { get; }
        IEnumerable<PresentationSurvey> PresentationSurveys { get; }
        IEnumerable<RoomSchedule> RoomSchedules { get; }
        IEnumerable<StudentTeam> StudentTeams { get; }
        IEnumerable<UserPassword> UserPasswords { get; }
        IEnumerable<User> Users { get; }
    }
}
