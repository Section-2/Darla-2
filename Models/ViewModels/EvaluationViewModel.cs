using System.Globalization;

namespace Darla.Models.ViewModels
{
    public class EvaluationViewModel
    {
/*        public IQueryable<PeerEvaluation> PeerEvaluations { get; set; }*/
        public string EvaluatorId { get; set; }
        public string SubjectId { get; set; }
        public string UserId { get; set; }
        public string NetId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SubjFName { get; set; }
        public string SubjLName { get; set; }
        public int QuestionId { get; set; }
        public string Question { get; set; }
    }
}
