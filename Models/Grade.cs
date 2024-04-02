using System;
using System.Collections.Generic;

namespace Darla.Models;

public partial class Grade
{
    public int AssignmentId { get; set; }

    public string UserId { get; set; }

    public int TeamNumber { get; set; }

    public int PointsEarned { get; set; }

    public string? Comments { get; set; }

    public int GradeId { get; set; }

    public virtual Rubric Assignment { get; set; } = null!;

    public virtual StudentTeam User { get; set; } = null!;
}
