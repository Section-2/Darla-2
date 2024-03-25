namespace Darla.Models;

public class EfJudgeRoomsRepository : IJudgeRoomsRepository
{
    private GradingContext _context;
    public EfJudgeRoomsRepository(GradingContext temp)
    {
        _context = temp;
    }
    
    public IQueryable<JudgeRoom> JudgeRooms => _context.JudgeRooms;
}