using System;
using System.Collections.Generic;

namespace Darla.Models;

public partial class Team
{
    public int TeamNumber { get; set; }

    public virtual ICollection<Presentation> Presentations { get; set; } = new List<Presentation>();

    public virtual ICollection<RoomSchedule> RoomSchedules { get; set; } = new List<RoomSchedule>();

    public virtual TeamSubmission? TeamSubmission { get; set; }
}
