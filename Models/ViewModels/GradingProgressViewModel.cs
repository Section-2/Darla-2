namespace Darla.Models.ViewModels
{
    public class GradingProgressViewModel
    {
        public double AveragePercentage { get; set; }
        public List<ClassGradingInfo> ClassGradingInfos { get; set; } = new List<ClassGradingInfo>();
    }

    public class ClassGradingInfo
    {
        public string ClassCode { get; set; }
        public int TotalGraded { get; set; }
        public int TotalAssignments { get; set; }
        public double PercentageGraded => (double)TotalGraded / TotalAssignments * 100;
    }
}
