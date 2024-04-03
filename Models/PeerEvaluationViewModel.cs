namespace Darla.Models
{
    public class PeerEvaluationViewModel
    {
        public int TeamNumber { get; set; }
        public List<MemberEvaluationInfo> Members { get; set; } = new List<MemberEvaluationInfo>();
    }
}
