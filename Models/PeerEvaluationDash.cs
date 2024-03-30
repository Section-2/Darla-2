// Models/PeerEvaluationDash.cs
namespace Darla.Models
{
    public class PeerEvaluationDash
    {
        public int GroupNumber { get; set; }
        public List<StudentEvaluation> Members { get; set; } = new List<StudentEvaluation>();
        // You can add additional properties or methods specific to the dashboard here.
    }
}

