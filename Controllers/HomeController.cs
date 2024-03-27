using System.Diagnostics;
using Darla.Models;
using Microsoft.AspNetCore.Mvc;
using Darla.Models;

namespace Darla.Controllers;

public class HomeController : Controller
{
    private IIntexRepository _repo;
<<<<<<< HEAD
    
    private readonly IIntexRepository _intexRepo;
=======
>>>>>>> 23b18754b08ecc48104e7f7f992947954ecbcf84
    
    public HomeController(IIntexRepository intexRepo)
    {
        _repo = intexRepo;
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
<<<<<<< HEAD
        return View("Judge/ScheduleView");
        var roomSchedules = _intexRepo.RoomSchedulesWithRooms;
=======
        var roomSchedules = _repo.RoomSchedulesWithRooms;
>>>>>>> 23b18754b08ecc48104e7f7f992947954ecbcf84
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