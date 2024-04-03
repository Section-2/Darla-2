using System;
using System.Collections.Generic;

namespace Darla.Models2;

public partial class room_schedule
{
    public int entry_id { get; set; }

    public string? timeslot { get; set; }

    public int team_number { get; set; }

    public int? room_id { get; set; }

    public virtual room entry { get; set; } = null!;

    public virtual ICollection<judge_room> judge_rooms { get; set; } = new List<judge_room>();

    public virtual team team_numberNavigation { get; set; } = null!;
}
