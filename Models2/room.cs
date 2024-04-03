using System;
using System.Collections.Generic;

namespace Darla.Models2;

public partial class room
{
    public int room_id { get; set; }

    public string? room_name { get; set; }

    public virtual room_schedule? room_schedule { get; set; }
}
