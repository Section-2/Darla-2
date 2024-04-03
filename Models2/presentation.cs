using System;
using System.Collections.Generic;

namespace Darla.Models2;

public partial class presentation
{
    public int presentation_id { get; set; }

    public string? judge_id { get; set; }

    public int team_number { get; set; }

    public int communication_score { get; set; }

    public string? communication_notes { get; set; }

    public int demonstration_score { get; set; }

    public string? demonstration_notes { get; set; }

    public int client_needs_score { get; set; }

    public string? client_needs_notes { get; set; }

    public string? awards { get; set; }

    public int? team_rank { get; set; }

    public string? overall_judge_notes { get; set; }

    public virtual judge_room? judge { get; set; }

    public virtual team team_numberNavigation { get; set; } = null!;
}
