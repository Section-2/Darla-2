using System;
using System.Collections.Generic;

namespace Darla.Models2.ViewModels;

public class MasterJudgeScheduleViewModel   
{
    public List<judge_room> JudgeRoom { get; set; }

    public List<room_schedule> RoomSchedule { get; set; }

    public List<user> User { get; set; }

    public List<permission> Permission { get; set; }

    public List<room> Room { get; set; }
}
