using System;
using System.Collections.Generic;

namespace Darla.Models;

public partial class TeamSubmission
{
    public int TeamNumber { get; set; }

    public string? GithubLink { get; set; }

    public string? VideoLink { get; set; }

    public string? GoogleDocLink { get; set; }

    public string? Timestamp { get; set; }

    public virtual Team TeamNumberNavigation { get; set; } = null!;
}
