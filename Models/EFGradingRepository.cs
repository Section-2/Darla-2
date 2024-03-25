namespace Darla.Models;

public class EFGradingRepository : IGradingRepository
{
    private GradingContext _context;

    public EFGradingRepository(GradingContext temp)
    {
        _context = temp;
    }
    public IQueryable<Survey> Surveys => _context.Surveys;
}