namespace Darla.Models
{
    public class AdminGradeProgressBarComposite
    {
        public IEnumerable<Grade>? Grades { get; set; }
        public IEnumerable<Team>? Teams { get; set; }
        public IEnumerable<Rubric>? Rubrics { get; set; }
        public double GradingProgress { get; set; }
        // Add other properties you need for your view here
    }
}
