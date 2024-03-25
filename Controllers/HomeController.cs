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

    public IActionResult chooseIdentity()
    {
        return View();
    }
}