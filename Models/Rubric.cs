using System;
using System.Collections.Generic;

namespace Darla.Models;

public partial class Rubric
{
    public int AssignmentId { get; set; }

    public int ClassCode { get; set; }

    public string? Subcategory { get; set; }

    public string? Decsription { get; set; }

    public int? MaxPoints { get; set; }

    public string? InstructorNotes { get; set; }

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();
}
