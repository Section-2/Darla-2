using System;
using System.Collections.Generic;

namespace Darla.Models;

public partial class Rubric
{
    public int AssignmentId { get; set; }
    public int ClassCode { get; set; }
    public string? Subcategory { get; set; }
    public string? Description { get; set; }
    public int? MaxPoints { get; set; }
    public string? InstructorNotes { get; set; }
    //public bool IsCompleted { get; set; } //added by student team view.

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();
}