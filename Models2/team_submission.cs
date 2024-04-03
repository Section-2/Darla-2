using System;
using System.Collections.Generic;

namespace Darla.Models2;

public partial class team_submission
{
    public int team_number { get; set; }

    public string? github_link { get; set; }

    public string? video_link { get; set; }

    public string? google_doc_link { get; set; }

    public virtual team team_numberNavigation { get; set; } = null!;
}
