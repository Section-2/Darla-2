using System.Diagnostics;
using Darla.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SQLitePCL;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

// test 2
namespace Darla.Controllers;

public class HomeController : Controller
{
    private IIntexRepository _repo;
    
    public HomeController(IIntexRepository Repo)
    {
        _repo = Repo;
    }

    // START HERE!
    public IActionResult OpeningPage()
    {
        return View();
    }

    // JUDGES SECTION
    public IActionResult JudgePage()
    {
        return View();
    }

    public IActionResult JudgeSignedIn()
    {
        return View("Judge/JudgeSignedIn");
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

    // END JUDGES SECTION



    // Grading Summary Page for TAs
    public IActionResult Index()
    {
        return View();
    }
    

    // ADMINS SECTION
    // Landing page for Admins
    public IActionResult AdminIndex()
    {
        ViewData["GradingProgress"] = 70;
        return View("AdminIndexDashboard");
    }

    public IActionResult ProfAddJudge()
    {
        return View("AdminAddJudge");
    }

    public IActionResult ProfFullRubric()
    {
        return View();
    }
    
    public IActionResult ProfEditRubric()
    {
    var query = _repo.Users.Where(x => x.PermissionType == 4);
    return View();
    }
    
    public IActionResult StudentProgress()
    {
        return View();
    }

    public IActionResult StudentDashboard()
    {
        return View();
    }

    public IActionResult RubricDetails()
    {
        return View();
    }

    /* Potential missing actions for views: TeacherViewPeerEvalSingle, ListTA, adminPeerEvalDashboard, 
     * AdminJudgeListView, AdminDeleteJudge
     */
}
