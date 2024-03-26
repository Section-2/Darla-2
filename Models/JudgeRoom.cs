using System;
using System.Collections.Generic;

namespace Darla.Models;

public partial class JudgeRoom
{
    public int UserId { get; set; }

    public int RoomId { get; set; }

    public virtual ICollection<Presentation> Presentations { get; set; } = new List<Presentation>();

    public virtual RoomSchedule Room { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
