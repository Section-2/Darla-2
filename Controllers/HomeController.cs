using System.Diagnostics;
using Darla.Models;
using Microsoft.AspNetCore.Mvc;
using Darla.Models;

namespace Darla.Controllers;

public class HomeController : Controller
{
    private IIntexRepository _repo;
    
    public HomeController(IIntexRepository Repo)
    {
        _repo = Repo;
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

    [HttpGet]
    public IActionResult judge_survey()
    {
        return View("Judge/judge_survey",new Presentation());
    }

    [HttpPost]
    public IActionResult judge_survey(Presentation p)
    {
        if (ModelState.IsValid)
        {
            _repo.AddPresentationScore(p);
        }

        return RedirectToAction("JudgeDashboard", new Presentation());
    }

    // Action to open judge schedule
    public IActionResult JudgeDashboard()
    {
        var roomSchedules = _repo.RoomSchedulesWithRooms;
        return View("Judge/JudgeDashboard", roomSchedules);
    }

    public IActionResult OpeningPage()
    {
        return View("Judge/JudgeDashboard");
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

}