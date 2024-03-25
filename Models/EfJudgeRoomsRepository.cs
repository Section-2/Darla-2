namespace Darla.Models;

public class EfJudgeRoomsRepository : IJudgeRoomsRepository
{
    private RatingContext _context;
    public EfJudgeRoomsRepository(RatingContext temp)
    {
        _context = temp;
    }
}