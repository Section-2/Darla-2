using System.Diagnostics;
using Darla.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SQLitePCL;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Darla.Models.ViewModels;

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
    // public IActionResult JudgeDashboard()
    // {
    //     var roomSchedules = _repo.RoomSchedulesWithRooms;
    //     return View("Judge/JudgeDashboard", roomSchedules);
    // }
    
    public IActionResult ScheduleByRoomId(int roomId)
    {
        var roomSchedules = _repo.GetRoomSchedulesByRoomId(roomId);
        return View("Judge/JudgeDashboard", roomSchedules);
    }
    [HttpPost]
    public IActionResult UpdateRanks(Dictionary<int,int>teamRanks)
    {
        _repo.UpdateTeamRanks(teamRanks);
        return RedirectToAction("JudgeDashboard");
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
        return View("AdminRubricFull");
    }
    
    public IActionResult ProfEditRubric()
    {
        var query = _repo.Users.Where(x => x.PermissionType == 4);
        return View("AdminRubricEdit");
    }

    public IActionResult AdminPeerEvalDashboard()
    {
        // Include necessary navigation properties to access related data
        var studentTeams = _repo.GetQueryableStudentTeams()
            .Include(st => st.User)
            .Include(st => st.PeerEvaluationSubjects)
            .ThenInclude(pe => pe.PeerEvaluationNavigation)
            .ToList();

        // Group by TeamNumber
        var groupedByTeam = studentTeams.GroupBy(st => st.TeamNumber);

        // Prepare a list of PeerEvaluationDash to hold the dashboard data
        var peerEvaluationDashes = new List<PeerEvaluationDash>();
        foreach (var group in groupedByTeam)
        {
            var dash = new PeerEvaluationDash
            {
                GroupNumber = group.Key,
                Members = group.Select(member =>
                {
                    var evaluations = _repo.PeerEvaluations
                                           .Where(pe => pe.SubjectId == member.UserId)
                                           .ToList();
                    return new StudentEvaluation
                    {
                        User = member.User,
                        PeerEvaluations = evaluations,
                        // No need to calculate Score here, since it's already a computed property
                    };
                }).ToList()
            };
            peerEvaluationDashes.Add(dash);
        }

        // Then, pass this data to the view (if needed)
        return View(peerEvaluationDashes); // Make sure you have a view named "adminPeerEvalDashboard.cshtml" under Views/Home/
    }

    public IActionResult AdminProfIndex()
    {
        // Calculate the number of teams with at least one grade entry
        var gradedTeamsCount = _repo.Grades
            .Select(g => g.TeamNumber)
            .Distinct()
            .Count();

        // Calculate the total number of teams
        var totalTeamsCount = _repo.Teams
            .Select(t => t.TeamNumber)
            .Distinct()
            .Count();

        // Calculate the total number of assignments
        var totalAssignmentsCount = _repo.Rubrics
            .Select(r => r.AssignmentId)
            .Distinct()
            .Count();

        // Avoid division by zero
        if (totalTeamsCount == 0 || totalAssignmentsCount == 0)
        {
            ViewData["GradingProgress"] = 0;
        }
        else
        {
            // Calculate the percentage
            var percentageOfTeamsGraded = (double)gradedTeamsCount / (totalTeamsCount * totalAssignmentsCount) * 100;
            ViewData["GradingProgress"] = Math.Round(percentageOfTeamsGraded, 2); // Round to two decimal places
        }

        var model = new AdminGradeProgressBarComposite
        {
            // If there are other properties to set, set them here
            GradingProgress = ViewData["GradingProgress"] != null ? Convert.ToDouble(ViewData["GradingProgress"]) : 0,
        };

        // Pass the model to the view
        return View(model);
    }

    public IActionResult AdminViewPeerEvalGiven(int evaluatorId)
    {
        var evaluationData = _repo.PeerEvaluations
            .Join(_repo.StudentTeams, pe => pe.EvaluatorId, st => st.UserId, (pe, st) => new { pe, st })
            .Join(_repo.Users, temp1 => temp1.st.UserId, u => u.UserId, (temp1, u) => new { temp1.pe, temp1.st, u })
            .Join(_repo.PeerEvaluationQuestions, temp2 => temp2.pe.QuestionId, pq => pq.QuestionId, (temp2, pq) => new { temp2.pe, temp2.st, temp2.u, pq })
            .Join(
                (from peInner in _repo.PeerEvaluations
                 join stInner in _repo.StudentTeams on peInner.SubjectId equals stInner.UserId
                 join uInner in _repo.Users on stInner.UserId equals uInner.UserId
                 select new
                 {
                     SubjFName = uInner.FirstName,
                     SubjLName = uInner.LastName,
                     peInner.SubjectId,
                     uInner.UserId
                 }),
                temp3 => temp3.pe.SubjectId,
                subj => subj.UserId,
                (temp3, subj) => new { temp3.pe, temp3.st, temp3.u, temp3.pq, subj }
            )
            .GroupBy(x => x.pe.PeerEvaluationId)
            .Select(group => group.First()) // Selecting the first element from each group
            .Select(result => new EvaluationViewModel
            {
                EvaluatorId = result.pe.EvaluatorId,
                SubjectId = result.pe.SubjectId,
                UserId = result.st.UserId,
                NetId = result.u.NetId,
                FirstName = result.u.FirstName,
                LastName = result.u.LastName,
                SubjFName = result.subj.SubjFName,
                SubjLName = result.subj.SubjLName,
                Question = result.pq.Question,
                QuestionId = result.pq.QuestionId,
            })
            .ToList();

        return View(evaluationData);
    }

    public IActionResult AdminViewPeerEvalReceived(int evaluatorId)
    {
        var evaluationData = _repo.PeerEvaluations
            .Join(_repo.StudentTeams, pe => pe.EvaluatorId, st => st.UserId, (pe, st) => new { pe, st })
            .Join(_repo.Users, temp1 => temp1.st.UserId, u => u.UserId, (temp1, u) => new { temp1.pe, temp1.st, u })
            .Join(_repo.PeerEvaluationQuestions, temp2 => temp2.pe.QuestionId, pq => pq.QuestionId, (temp2, pq) => new { temp2.pe, temp2.st, temp2.u, pq })
            .Join(
                (from peInner in _repo.PeerEvaluations
                 join stInner in _repo.StudentTeams on peInner.SubjectId equals stInner.UserId
                 join uInner in _repo.Users on stInner.UserId equals uInner.UserId
                 select new
                 {
                     SubjFName = uInner.FirstName,
                     SubjLName = uInner.LastName,
                     peInner.SubjectId,
                     uInner.UserId
                 }),
                temp3 => temp3.pe.SubjectId,
                subj => subj.UserId,
                (temp3, subj) => new { temp3.pe, temp3.st, temp3.u, temp3.pq, subj }
            )
            .GroupBy(x => x.pe.PeerEvaluationId)
            .Select(group => group.First()) // Selecting the first element from each group
            .Select(result => new EvaluationViewModel
            {
                EvaluatorId = result.pe.EvaluatorId,
                SubjectId = result.pe.SubjectId,
                UserId = result.st.UserId,
                NetId = result.u.NetId,
                FirstName = result.u.FirstName,
                LastName = result.u.LastName,
                SubjFName = result.subj.SubjFName,
                SubjLName = result.subj.SubjLName,
                Question = result.pq.Question,
                QuestionId = result.pq.QuestionId,
            })
            .ToList();

        return View(evaluationData);
    }
    // return View(evaluationData);
    // }

    //    return View(evaluationData);
    //}

    public IActionResult MasterJudgeSchedule()
    {
        var judgeRooms = _repo.JudgeRooms.ToList();
        var roomSchedules = _repo.RoomSchedules.ToList();
        var permissions = _repo.Permissions.ToList();
        var rooms = _repo.Rooms.ToList();

        var users = _repo.Users
            .Where(u => u.PermissionType == 4 && judgeRooms.Any(jr => jr.UserId == u.UserId))
            .ToList();

        var judgeSchedule = new MasterJudgeScheduleViewModel
        {
            JudgeRoom = judgeRooms,
            RoomSchedule = roomSchedules,
            User = users,
            Permission = permissions,
            Room = rooms
        };

        return View(judgeSchedule);
    }

    public IActionResult StudentProgress()
    {
        return View("Index");
    }

    public IActionResult RubricDetails()
    {
        return View();
    }
    
    public IActionResult JudgeDashboard()
    {
        return View("Judge/JudgeDashboard");
    }

    [HttpGet]
    public IActionResult EditJudge(string id)
    {
        var recordToEdit = _repo.Users
            .Single(x => x.UserId == id);
        return View("AdminAddJudge", recordToEdit);
    }

    [HttpPost]
    public IActionResult Edit(User updatedInfo)
    {
        _repo.EditJudge(updatedInfo);
    return RedirectToAction("");
    /*return RedirectToAction("AdminJudgeListView");*/

    }


    [HttpGet]
    public IActionResult DeleteJudge(string id)
    {
        var recordToDelete = _repo.Users
            .Single(x => x.UserId == id);
        return View("AdminDeleteJudge",recordToDelete);
    }

    [HttpPost]
    public IActionResult DeleteJudge(User removedUser)
    {
        _repo.DeleteJudge(removedUser);
        return RedirectToAction("");
        /*return RedirectToAction("AdminJudgeListView");*/
    }


    public IActionResult AdminRubricFull()
    {
        var rubrics = _repo.Rubrics.ToList();

        return View(rubrics);
    }

    [HttpGet]
    public IActionResult AdminRubricEdit(int classCode)
    {
        var rubrics = _repo.Rubrics
            .Where(x => x.ClassCode == classCode).ToList();

        return View("AdminRubricEdit", rubrics);
    }

    [HttpPost]
    public IActionResult AdminRubricEdit(Rubric updatedRubric)
    {
        if (ModelState.IsValid)
        {
            _repo.EditRubric(updatedRubric);
            return RedirectToAction("AdminRubricEdit");
        }
        else
        {
            return View("AdminRubricAdd", updatedRubric);
        }
    }

    [HttpGet]
    public IActionResult AdminRubricAdd()
    {
        return View(new Rubric());
    }
    
    [HttpPost]
    public IActionResult AdminRubricAdd(Rubric rubric)
    {
        if (ModelState.IsValid)
        {
            _repo.AddRubric(rubric);
            return View("AdminRubricEdit", rubric);
        }
        else
        {
            return View("AdminRubricEdit", rubric);
        }
    }

    [HttpGet]
    public IActionResult AdminRubricDelete(int assignmentId)
    {
        var delete = _repo.Rubrics.Single(x => x.ClassCode == assignmentId);
        return View("AdminRubricEdit", delete);
    }

    [HttpPost]
    public IActionResult AdminRubricDelete(Rubric rubric)
    {
        _repo.DeleteRubric(rubric);
        return View("AdminRubricEdit");
    }

    [HttpGet]
    public IActionResult AdminJudgeListView()
    {
        ViewBag.Permissions = _repo.Permissions.ToList()
            .OrderBy(x => x.PermissionDescription)
            .Where(x => x.PermissionType == 4)
            .ToList();
        return View("AdminJudgeListView", new User());
    }
    [HttpPost]
    public IActionResult AdminJudgeListView(User response)
    {
        if (ModelState.IsValid)
        {
            _repo.AddJudge(response);
            return View("AdminAddJudgeConfirmation", response);
        }
        else
        {
            ViewBag.Permissions = _repo.Permissions.ToList()
                .OrderBy(x => x.PermissionDescription)
                .ToList();
            return View("AdminJudgeListView", new User());
        }
    }


    /* Potential missing actions for views: TeacherViewPeerEvalSingle, ListTA, adminPeerEvalDashboard, 
     * AdminJudgeListView, AdminDeleteJudge
     */
}
