// Models/StudentEvaluation.cs
using System.Collections.Generic;
using System.Linq;

namespace Darla.Models
{
    public class StudentEvaluation
    {
        public User User { get; set; }
        public List<PeerEvaluation> PeerEvaluations { get; set; }

        public int Score
        {
            get { return PeerEvaluations.Sum(e => e.Rating); }
        }

        public string EvaluationLink
        {
            // Placeholder for the logic to generate the link to view evaluations
            // You'll need to replace "Link_To_Evaluation" with actual logic to generate the link
            get { return PeerEvaluations.Any() ? "Link_To_Evaluation" : string.Empty; }
        }
    }
}

