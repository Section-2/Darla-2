using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Darla.Models;

public partial class StudentTeam
{
    
    public string UserId { get; set; }

    public int TeamNumber { get; set; }

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();

    public virtual ICollection<PeerEvaluation> PeerEvaluationEvaluators { get; set; } = new List<PeerEvaluation>();

    public virtual ICollection<PeerEvaluation> PeerEvaluationSubjects { get; set; } = new List<PeerEvaluation>();

    public virtual User User { get; set; } = null!;

    public virtual ApplicationUser ApplicationUser { get; set; }
}
