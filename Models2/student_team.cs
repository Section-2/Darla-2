using System;
using System.Collections.Generic;

namespace Darla.Models2;

public partial class student_team
{
    public string user_id { get; set; } = null!;

    public int team_number { get; set; }

    public virtual ICollection<grade> grades { get; set; } = new List<grade>();

    public virtual ICollection<peer_evaluation> peer_evaluationevaluators { get; set; } = new List<peer_evaluation>();

    public virtual ICollection<peer_evaluation> peer_evaluationsubjects { get; set; } = new List<peer_evaluation>();

    public virtual team team_numberNavigation { get; set; } = null!;

    public virtual AspNetUser user { get; set; } = null!;
}
