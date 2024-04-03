using System;
using System.Collections.Generic;

namespace Darla.Models2;

public partial class peer_evaluation_question
{
    public int question_id { get; set; }

    public string? question { get; set; }

    public virtual ICollection<peer_evaluation> peer_evaluations { get; set; } = new List<peer_evaluation>();
}
