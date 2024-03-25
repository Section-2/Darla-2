using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Darla.Models;

namespace Darla.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
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

    }
}