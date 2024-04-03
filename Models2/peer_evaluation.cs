using System;
using System.Collections.Generic;

namespace Darla.Models2;

public partial class peer_evaluation
{
    public int peer_evaluation_id { get; set; }

    public string? evaluator_id { get; set; }

    public string? subject_id { get; set; }

    public int question_id { get; set; }

    public int rating { get; set; }

    public string? comments { get; set; }

    public virtual student_team? evaluator { get; set; }

    public virtual peer_evaluation_question question { get; set; } = null!;

    public virtual student_team? subject { get; set; }
}
