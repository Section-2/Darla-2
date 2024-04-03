using System;
using System.Collections.Generic;

namespace Darla.Models2;

public partial class grade
{
    public int assignment_id { get; set; }

    public string? user_id { get; set; }

    public int team_number { get; set; }

    public decimal points_earned { get; set; }

    public string? comments { get; set; }

    public int grade_id { get; set; }

    public virtual rubric assignment { get; set; } = null!;

    public virtual team team_numberNavigation { get; set; } = null!;

    public virtual student_team? user { get; set; }
}
