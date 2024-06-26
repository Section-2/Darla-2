﻿using System;
using System.Collections.Generic;

namespace Darla.Models;

public partial class PeerEvaluation
{
    public int PeerEvaluationId { get; set; }

    public string EvaluatorId { get; set; }

    public string SubjectId { get; set; }

    public int QuestionId { get; set; }

    public int Rating { get; set; }

    public string? Comments { get; set; }


    public virtual StudentTeam Evaluator { get; set; } = null!;

    public virtual PeerEvaluationQuestion PeerEvaluationNavigation { get; set; } = null!;

    public virtual StudentTeam Subject { get; set; } = null!;
}
