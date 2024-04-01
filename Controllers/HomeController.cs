using System.Diagnostics;
using Darla.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SQLitePCL;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

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

        /*[HttpGet]
        public IActionResult Edit(int id)
        {
            var recordToEdit = _context.Users
                .Single(x => x.UserId == id);
            return View("AdminAddJudge", recordToEdit);
        }

        [HttpPost]
        public IActionResult Edit(User updatedInfo)
        {
            _context.Update(updatedInfo);
            _context.SaveChanges();
            return RedirectToAction("AdminJudgeListView");
        }


        [HttpGet]
        public IActionResult Delete(int id)
        {
            var recordToDelete = _context.Users
                .Single(x => x.UserId == id);
            return View(recordToDelete);
        }  

        [HttpPost]
        public IActionResult Delete(User removedUser)
        {
            _context.Users.Remove(removedUser);
            _context.SaveChanges();

            return RedirectToAction("AdminJudgeListView");
        }*/

    }

    /* Potential missing actions for views: TeacherViewPeerEvalSingle, ListTA, adminPeerEvalDashboard, 
     * AdminJudgeListView, AdminDeleteJudge
     */
}
