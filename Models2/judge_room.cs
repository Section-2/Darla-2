using System;
using System.Collections.Generic;

namespace Darla.Models2;

public partial class judge_room
{
    public string user_id { get; set; } = null!;

    public int room_id { get; set; }

    public virtual ICollection<presentation> presentations { get; set; } = new List<presentation>();

    public virtual room_schedule room { get; set; } = null!;
}
