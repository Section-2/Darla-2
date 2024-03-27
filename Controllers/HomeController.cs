using System.Diagnostics;
using Darla.Models;
using Microsoft.AspNetCore.Mvc;
using Darla.Models;

namespace Darla.Controllers;

public class HomeController : Controller
{
    private IIntexRepository _repo;
    
    private readonly IIntexRepository _intexRepo;
    
    public HomeController(IIntexRepository intexRepo)
    {
        _intexRepo = intexRepo;
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
        return View("Judge/judge_survey");
    }

    [HttpGet]
    public IActionResult AddPresentationScore()
    {
        var presentationData = _repo.Presentations.ToArray();

        return View(presentationData);
    }

    [HttpPost]
    public IActionResult AddPresentationScore(Presentation p)
    {
        if (ModelState.IsValid)
        {
            _repo.AddPresentationScore(p);
        }

        return RedirectToAction("ScheduleView", new Presentation());
    }

    // Action to open judge schedule
    public IActionResult ScheduleView()
    {
        return View("Judge/ScheduleView");
        var roomSchedules = _intexRepo.RoomSchedulesWithRooms;
        return View("Judge/ScheduleView", roomSchedules);
    }

    public IActionResult OpeningPage()
    {
        return View("Judge/ScheduleView");
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