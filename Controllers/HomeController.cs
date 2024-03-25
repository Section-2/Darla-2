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

    public IActionResult OpeningPage()
    {
        return View();
    }

}