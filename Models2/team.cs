using System;
using System.Collections.Generic;

namespace Darla.Models2;

public partial class team
{
    public int team_number { get; set; }

    public virtual ICollection<grade> grades { get; set; } = new List<grade>();

    public virtual ICollection<presentation> presentations { get; set; } = new List<presentation>();

    public virtual ICollection<room_schedule> room_schedules { get; set; } = new List<room_schedule>();

    public virtual ICollection<student_team> student_teams { get; set; } = new List<student_team>();

    public virtual team_submission? team_submission { get; set; }
}
