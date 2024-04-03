using Darla.Models2;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Darla.Models.ViewModels;
using IntexGraderContext = Darla.Models.IntexGraderContext;
using MasterJudgeScheduleViewModel = Darla.Models2.ViewModels.MasterJudgeScheduleViewModel;

namespace Darla.Controllers;

public class HomeController : Controller
{
    private IIntexRepository _repo;
    
    public HomeController(IIntexRepository repo)
    {
        _repo = repo;
    }

    // START HERE!
    public IActionResult OpeningPage()
    {
        if (User.Identity.IsAuthenticated == true)
        {
            // User is logged in, proceed with your logic
                
            // Change this logic later
            return RedirectToAction("StudentDashboard", "Student");
        }
        else
        {
            // User is not logged in, redirect to the login page
            return View();
        }
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
        var presentation = new presentation();

        if (teamNumber.HasValue)
        {
            presentation.team_number = teamNumber.Value;
            // If JudgeId is needed from the logged-in user, assign it similarly
            // presentation.JudgeId = ...;
        }

        return View("Judge/judge_survey", presentation);
    }

    [HttpPost]
    public IActionResult judge_survey(presentation p)
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

        return RedirectToAction("JudgeDashboard", new presentation());
    }

    // Action to open judge schedule
    // public IActionResult JudgeDashboard()
    // {
    //     var roomSchedules = _repo.RoomSchedulesWithRooms;
    //     return View("Judge/JudgeDashboard", roomSchedules);
    // }
    
    public IActionResult ScheduleByRoomId(int roomId)
    {
        var roomSchedules = _repo.GetRoomSchedulesByRoomId(roomId).ToList();
    
        // Assuming RoomSchedules includes Room, fetch the first Room's details if available
        // var roomDetails = roomSchedules.Select(rs => rs.Room).FirstOrDefault();
        // var roomDetails = roomSchedules = roomSchedules.SelectMany(rs => rs.judge_rooms).Select(jr => jr.room).FirstOrDefault();
        var roomDetails = roomSchedules
            .SelectMany(rs => rs.judge_rooms)
            .Select(jr => jr.room)
            .FirstOrDefault();

        var viewModel = new Models2.ViewModels.MasterJudgeScheduleViewModel()
        {
            RoomSchedule = roomSchedules,
            Room = roomDetails != null ? new List<room> { roomDetails } : new List<room>()
            // Initialize other properties as needed
        };

        return View("Judge/JudgeDashboard", viewModel);
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
        var query = _repo.Users.Where(x => x.permission_type == 4);
        return View("AdminRubricEdit");
    }

    public async Task<IActionResult> AdminPeerEvalDashboard()
    {
        var viewModel = await _repo.GetPeerEvaluationInfo();
        return View(viewModel);
    }

    public IActionResult AdminProfIndex()
    {
        // Calculate the number of teams with at least one grade entry
        var gradedTeamsCount = _repo.Grades
            .Select(g => g.team_number)
            .Distinct()
            .Count();

        // Calculate the total number of teams
        var totalTeamsCount = _repo.Teams
            .Select(t => t.team_number)
            .Distinct()
            .Count();

        // Calculate the total number of assignments
        var totalAssignmentsCount = _repo.Rubrics
            .Select(r => r.assignment_id)
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

        var model = new AdminGradeProgressBarComposite()
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
            .Join(_repo.StudentTeams, pe => pe.evaluator_id, st => st.user_id, (pe, st) => new { pe, st })
            .Join(_repo.Users, temp1 => temp1.st.user_id, u => u.user_id, (temp1, u) => new { temp1.pe, temp1.st, u })
            .Join(_repo.PeerEvaluationQuestions, temp2 => temp2.pe.question_id, pq => pq.question_id, (temp2, pq) => new { temp2.pe, temp2.st, temp2.u, pq })
            .Join(
                (from peInner in _repo.PeerEvaluations
                 join stInner in _repo.StudentTeams on peInner.subject_id equals stInner.user_id
                 join uInner in _repo.Users on stInner.user_id equals uInner.user_id
                 select new
                 {
                     SubjFName = uInner.first_name,
                     SubjLName = uInner.last_name,
                     peInner.subject_id,
                     uInner.user_id
                 }),
                temp3 => temp3.pe.subject_id,
                subj => subj.user_id,
                (temp3, subj) => new { temp3.pe, temp3.st, temp3.u, temp3.pq, subj }
            )
            .GroupBy(x => x.pe.peer_evaluation_id)
            .Select(group => group.First()) // Selecting the first element from each group
            .Select(result => new EvaluationViewModel
            {
                EvaluatorId = result.pe.evaluator_id,
                SubjectId = result.pe.subject_id,
                UserId = result.st.user_id,
                NetId = result.u.net_id,
                FirstName = result.u.first_name,
                LastName = result.u.last_name,
                SubjFName = result.subj.SubjFName,
                SubjLName = result.subj.SubjLName,
                Question = result.pq.question,
                QuestionId = result.pq.question_id,
            })
            .ToList();

        return View(evaluationData);
    }

    public IActionResult AdminViewPeerEvalReceived(int evaluatorId)
    {
        var evaluationData = _repo.PeerEvaluations
            .Join(_repo.StudentTeams, pe => pe.evaluator_id, st => st.user_id, (pe, st) => new { pe, st })
            .Join(_repo.Users, temp1 => temp1.st.user_id, u => u.user_id, (temp1, u) => new { temp1.pe, temp1.st, u })
            .Join(_repo.PeerEvaluationQuestions, temp2 => temp2.pe.question_id, pq => pq.question_id, (temp2, pq) => new { temp2.pe, temp2.st, temp2.u, pq })
            .Join(
                (from peInner in _repo.PeerEvaluations
                 join stInner in _repo.StudentTeams on peInner.subject_id equals stInner.user_id
                 join uInner in _repo.Users on stInner.user_id equals uInner.user_id
                 select new
                 {
                     SubjFName = uInner.first_name,
                     SubjLName = uInner.last_name,
                     peInner.subject_id,
                     uInner.user_id
                 }),
                temp3 => temp3.pe.subject_id,
                subj => subj.user_id,
                (temp3, subj) => new { temp3.pe, temp3.st, temp3.u, temp3.pq, subj }
            )
            .GroupBy(x => x.pe.peer_evaluation_id)
            .Select(group => group.First()) // Selecting the first element from each group
            .Select(result => new EvaluationViewModel
            {
                EvaluatorId = result.pe.evaluator_id,
                SubjectId = result.pe.subject_id,
                UserId = result.st.user_id,
                NetId = result.u.net_id,
                FirstName = result.u.first_name,
                LastName = result.u.last_name,
                SubjFName = result.subj.SubjFName,
                SubjLName = result.subj.SubjLName,
                Question = result.pq.question,
                QuestionId = result.pq.question_id,
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
            .Where(u => u.permission_type == 4 && judgeRooms.Any(jr => jr.user_id == u.user_id))
            .ToList();

        var judgeSchedule = new MasterJudgeScheduleViewModel()
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
            .Single(x => x.user_id == id);
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
    public IActionResult AdminRubricEdit(int assignmentId)
    {
        var rubricItem = _repo.Rubrics
            .Single(x => x.AssignmentId == assignmentId);

        return View("AdminRubricAdd", rubricItem);
    }

    [HttpPost]
    public IActionResult AdminRubricEdit(Rubric updatedRubric)
    {
        if (ModelState.IsValid)
        {
            _repo.EditRubric(updatedRubric);

            return RedirectToAction("AdminRubricFull");
        }
        else
        {
            return View("AdminRubricAdd", updatedRubric);
        }
    }

    [HttpGet]
    public IActionResult AdminRubricAdd(int classCode)
    {
        var newItem = new Rubric { ClassCode = classCode };

        return View(newItem);
    }

    [HttpPost]
    public IActionResult AdminRubricAdd(Rubric response)
    {
        if (ModelState.IsValid)
        {
            _repo.AddRubric(response);
            return RedirectToAction("AdminRubricFull");
        }
        else
        {
            return View(response);
        }
    }

    [HttpGet]
    public IActionResult AdminRubricDelete(int assignmentId)
    {
        var itemToDelete = _repo.Rubrics
            .Single(x => x.AssignmentId == assignmentId);

        return View(itemToDelete);
    }

    [HttpPost]
    public IActionResult AdminRubricDelete(Rubric taskToDelete)
    {
        var itemToDelete = _repo.Rubrics
            .Single(x => x.AssignmentId == taskToDelete.AssignmentId);

        _repo.DeleteRubric(itemToDelete);

        return RedirectToAction("AdminRubricFull");
    }

    [HttpGet]
    public IActionResult AdminJudgeListView()
    {
        ViewBag.Permissions = _repo.Permissions.ToList()
            .OrderBy(x => x.PermissionDescription)
            .Where(x => x.PermissionType == 4)
            .ToList();
        
        List<User> users = new List<User> { new User() };
        
        return View("AdminJudgeListView", users);
    }
    [HttpPost]
    public IActionResult AdminJudgeListView(User response)
    {
        if (ModelState.IsValid)
        {
            _repo.AddJudge(response);
            return View("AdminAddJudge", response);
        }
        else
        {
            ViewBag.Permissions = _repo.Permissions.ToList()
                .OrderBy(x => x.PermissionDescription)
                .ToList();
            
            List<User> users = new List<User> { new User() };

            return View("AdminJudgeListView", users);
        }
    }

    [HttpGet]
    public IActionResult AdminTAListView()
    {
        ViewBag.Permissions = _repo.Permissions.ToList()
            .OrderBy(x => x.PermissionDescription)
            .Where(x => x.PermissionType == 3)
            .ToList();

        List<User> users = new List<User> { new User() };

        return View("AdminTAListView", users);
    }

    [HttpPost]
    public IActionResult AdminTAListView(User addTAResponse)
    {
        if (ModelState.IsValid)
        {
            _repo.AddTA(addTAResponse);
            return View("AdminAddTA", addTAResponse);
        }
        else
        {
            ViewBag.Permissions = _repo.Permissions.ToList()
                .OrderBy(x => x.PermissionDescription)
                .ToList();

            List<User> users = new List<User> { new User() };

            return View("AdminTAListView", users);
        }
    }

    //[HttpGet]
    //public IActionResult EditTA(string id)
    //{
    //    var recordToEdit = _repo.Users
    //        .Single(x => x.UserId == id);
    //    return View("AdminAddTA", recordToEdit);
    //}

    //[HttpPost]
    //public IActionResult Edit(User updatedInfo)
    //{
    //    _repo.EditJudge(updatedInfo);
    //    return RedirectToAction("");
    //    /*return RedirectToAction("AdminJudgeListView");*/

    //}


    [HttpGet]
    public IActionResult DeleteTA(string id)
    {
        var recordToDelete = _repo.Users
            .Single(x => x.UserId == id);
        return View("AdminDeleteTA", recordToDelete);
    }

    [HttpPost]
    public IActionResult DeleteTA(User removedTAUser)
    {
        _repo.DeleteTA(removedTAUser);
        return RedirectToAction("");
    }


    /* Potential missing actions for views: TeacherViewPeerEvalSingle, ListTA, adminPeerEvalDashboard, 
     * AdminJudgeListView, AdminDeleteJudge
     */
}
