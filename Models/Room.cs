using System;
using System.Collections.Generic;

namespace Darla.Models;

public partial class Room
{
    public int RoomId { get; set; }

    public string RoomName { get; set; } = null!;

    public virtual RoomSchedule? RoomSchedule { get; set; }
}
