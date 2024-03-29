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

    // Initial Login page: links to Judge Page! Should redirect to Student OR Admin OR TA views after login... how? 
    public IActionResult BYULogin()
    {
        return View();
    }

    // Who should have access to this page?
    public IActionResult CreateAccount()
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
    public IActionResult judge_survey(int? teamNumber)
    {
        var presentation = new Presentation();

        if (teamNumber.HasValue)
        {
            presentation.TeamNumber = teamNumber.Value;
            // If JudgeId is needed from the logged-in user, assign it similarly
            // presentation.JudgeId = ...;
        }

        return View("Judge/judge_survey", presentation);
    }

    [HttpPost]
    public IActionResult judge_survey(Presentation p)
    {
        if (!ModelState.IsValid)
        {
            // Log each model error
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    // Log the error to your logging framework; for example:
                    Console.WriteLine(error.ErrorMessage); // Replace with your actual logging mechanism
                }
            }

            // Return to the view with the current Presentation model to display errors
            return View("Judge/judge_survey", p);
        }

        // If the model is valid, proceed with adding the presentation score
        _repo.AddPresentationScore(p);

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

    public IActionResult RubricDetails()
    {
        return View();
    }

    /* Potential missing actions for views: TeacherViewPeerEvalSingle, ListTA, adminPeerEvalDashboard, 
     * AdminJudgeListView, AdminDeleteJudge
     */
}
