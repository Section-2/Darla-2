using System;
using System.Collections.Generic;

namespace Darla.Models2;

public partial class rubric
{
    public int assignment_id { get; set; }

    public int class_code { get; set; }

    public string? subcategory { get; set; }

    public string? description { get; set; }

    public int? max_points { get; set; }

    public string? instructor_notes { get; set; }

    public virtual ICollection<grade> grades { get; set; } = new List<grade>();
}
