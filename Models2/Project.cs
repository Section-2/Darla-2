using System;
using System.Collections.Generic;

namespace Darla.Models2;

public partial class Project
{
    public int ProjectID { get; set; }

    public string? ProjectName { get; set; }

    public string? ProgramName { get; set; }

    public string? ProjectType { get; set; }

    public int ProjectImpact { get; set; }

    public DateTime ProjectInstallation { get; set; }

    public string? ProjectPhase { get; set; }

    public virtual ICollection<CartLine> CartLines { get; set; } = new List<CartLine>();
}
