using System;
using System.Collections.Generic;

namespace Darla.Models;

public partial class PeerEvaluationQuestion
{
    public int QuestionId { get; set; }

    public string Question { get; set; } = null!;

    //public virtual PeerEvaluation? PeerEvaluation { get; set; }
}
