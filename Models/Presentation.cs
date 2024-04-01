using System;
using System.Collections.Generic;

namespace Darla.Models;

public partial class Presentation
{
    public int PresentationId { get; set; }

    public int JudgeId { get; set; }

    public int TeamNumber { get; set; }

    public int CommunicationScore { get; set; }

    public string? CommunicationNotes { get; set; }

    public int DemonstrationScore { get; set; }

    public string? DemonstrationNotes { get; set; }

    public int ClientNeedsScore { get; set; }

    public string? ClientNeedsNotes { get; set; }

    public string? Awards { get; set; }

    public int? TeamRank { get; set; }

    public virtual JudgeRoom Judge { get; set; } = null!;
    
    public virtual Team TeamNumberNavigation { get; set; } = null!;
}
