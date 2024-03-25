using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
//using Darla.Models;

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

    public IActionResult JudgePage()
    {
        return View();
    }

    public IActionResult judge_survey()
    {
        return View("Judge/judge_survey");
    }

    // Action to open judge schedule
    public IActionResult ScheduleView()
    {
        return View("Judge/ScheduleView");

    public IActionResult OpeningPage()
    {
        return View();
    }

}