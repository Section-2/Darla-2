using System;
using System.Collections.Generic;

namespace Darla.Models;

public class MasterJudgeScheduleViewModel   
{
    public judge_room judge_room { get; set; }

    public room_schedule room_schedule { get; set; }

    public user user { get; set; }

    public permission permission { get; set; }

    public room room { get; set; }
}
