using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Darla.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Darla.Controllers;

public class HomeController : Controller
{

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

    Action to open judge schedule
    public IActionResult ScheduleView()
    {
        return View("Judge/ScheduleView");
    }
    public IActionResult OpeningPage()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult ProfIndex()
    {
        ViewData["GradingProgress"] = 70;
        return View();
    }
    public IActionResult ProfFullRubric()
    {
        ViewData["GradingProgress"] = 70;
        return View();
    }
    public IActionResult ProfEditRubric()
    {
        ViewData["GradingProgress"] = 70;
        return View();
    }
}