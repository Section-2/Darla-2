using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Darla.Models;
using Darla.Models.ViewModels;

namespace Darla.Controllers;

public class HomeController : Controller
{

    private IIntexRepository _repo;

    public HomeController(IIntexRepository temp)
    {
        _repo = temp;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult BYULogin()
    {
        return View();
    }

    public IActionResult AllGrades()
    {
        return View();
    }
    public IActionResult ClassRubric()
    {
        return View();
    }
    public IActionResult TaGradingProgress()
    {
        return View();
    }
    
    public IActionResult JudgePage()
    {
        return View();
    }

    public IActionResult judge_survey()
    {

        return View();
    }

    // Action to open judge schedule
    public IActionResult ScheduleView()
    {
        return View("Judge/ScheduleView");
    }


    public IActionResult OpeningPage()
    {
        return View();
    }

    //Allowing access to StudentSubmission
    public IActionResult StudentProgress()
    {
        return View();
    }

    public IActionResult ProfIndex()
    {
        ViewData["GradingProgress"] = 70;
        return View();
    }

    public IActionResult TeacherViewPeerEvalSingle(int evaluatorId)
    {
        var evaluationData = _repo.PeerEvaluations
            .Join(_repo.StudentTeams, pe => pe.EvaluatorId, st => st.UserId, (pe, st) => new { pe, st })
            .Join(_repo.Users, temp1 => temp1.st.UserId, u => u.UserId, (temp1, u) => new { temp1.pe, temp1.st, u })
            .Join(_repo.PeerEvaluationQuestions, temp2 => temp2.pe.QuestionId, pq => pq.QuestionId, (temp2, pq) => new { temp2.pe, temp2.st, temp2.u, pq })
            .Join(
                (from peInner in _repo.PeerEvaluations
                 join stInner in _repo.StudentTeams on peInner.SubjectId equals stInner.UserId
                 join uInner in _repo.Users on stInner.UserId equals uInner.UserId
                 select new
                 {
                     SubjFName = uInner.FirstName,
                     SubjLName = uInner.LastName,
                     peInner.SubjectId,
                     uInner.UserId
                 }),
                temp3 => temp3.pe.SubjectId,
                subj => subj.UserId,
                (temp3, subj) => new { temp3.pe, temp3.st, temp3.u, temp3.pq, subj }
            )
            .GroupBy(x => x.pe.PeerEvaluationId)
            .Select(group => group.First()) // Selecting the first element from each group
            .Select(result => new EvaluationViewModel
            {
                EvaluatorId = result.pe.EvaluatorId,
                SubjectId = result.pe.SubjectId,
                UserId = result.st.UserId,
                NetId = result.u.NetId,
                FirstName = result.u.FirstName,
                LastName = result.u.LastName,
                SubjFName = result.subj.SubjFName,
                SubjLName = result.subj.SubjLName,
                Question = result.pq.Question,
                QuestionId = result.pq.QuestionId,
            })
            .ToList();

        return View(evaluationData);
    }


}