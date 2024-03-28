using System;
using System.Collections.Generic;

namespace Darla.Models;

public class MasterJudgeScheduleViewModel   
{
    public List<JudgeRoom> JudgeRoom { get; set; }

    public List<RoomSchedule> RoomSchedule { get; set; }

    public List<User> User { get; set; }

    public List<Permission> Permission { get; set; }

    public List<Room> Room { get; set; }
}
