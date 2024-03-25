using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

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

    public IActionResult AllGrades()
    {
        return View();
    }
    public IActionResult ClassRubric()
    {
        return View();
    }
    public IActionResult MissingReq()
    {
        return View();
    }
    public IActionResult TAGradingProgress()
    {
        return View();
    }
}