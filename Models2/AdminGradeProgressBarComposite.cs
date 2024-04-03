namespace Darla.Models2
{
    public class AdminGradeProgressBarComposite
    {
        public IEnumerable<grade>? Grades { get; set; }
        public IEnumerable<team>? Teams { get; set; }
        public IEnumerable<rubric>? Rubrics { get; set; }
        public double GradingProgress { get; set; }
        // Add other properties you need for your view here
    }
}
