using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Darla.Models;

public partial class RoomSchedule
{
    public int EntryId { get; set; }

    public int RoomId { get; set; }

    public string Timeslot { get; set; } = null!;

    public int TeamNumber { get; set; }

    public virtual ICollection<JudgeRoom> JudgeRooms { get; set; } = new List<JudgeRoom>();

    public virtual Room Room { get; set; } = null!;

    public virtual Team TeamNumberNavigation { get; set; } = null!;
}
