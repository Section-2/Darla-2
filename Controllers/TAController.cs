using Darla.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Darla.Controllers
{
    public class TAController : Controller
    {

        //Controller
        private IIntexRepository _repo;

        public TAController(IIntexRepository temp)
        {
            _repo = temp;
        }

        //Views 
        /*
                public IActionResult AllGrades()
                {
                    var TeamInfo = _repo.Teams.ToList();

                    return View("~/Views/Home/TA/AllGrades.cshtml", TeamInfo);
                }

                public IActionResult TaGradingProgress()
                {
                    return View("~/Views/Home/TA/TaGradingProgress.cshtml");
                }

                public IActionResult ClassRubric()
                {
                    return View("~/Views/Home/TA/ClassRubric.cshtml");
                }*/

        // This action retrieves the data and returns it to the AllGrades view
        public IActionResult AllGrades()
        {
            var viewModel = new IntexViewModel
            {
                Grades = (IQueryable<Grade>)_repo.Grades,
                Rubrics = (IQueryable<Rubric>)_repo.Rubrics
            };

            return View("~/Views/Home/TA/AllGrades.cshtml", viewModel);
          
        }

        public IActionResult TaTesting()
        {
            var viewModel = new IntexViewModel
            {
                Grades = (IQueryable<Grade>)_repo.Grades,
                Rubrics = (IQueryable<Rubric>)_repo.Rubrics,
                JudgeRooms = (IQueryable<JudgeRoom>)_repo.JudgeRooms,
                PeerEvaluations = (IQueryable<PeerEvaluation>)_repo.PeerEvaluations,
                PeerEvaluationsQuestions = (IQueryable<PeerEvaluationQuestion>)_repo.PeerEvaluationQuestions,
                Permissions = (IQueryable<Permission>)_repo.Permissions,
                Presentations = (IQueryable<Presentation>)_repo.Presentations,
                Rooms = (IQueryable<Room>)_repo.Rooms,
                RoomSchedules = (IQueryable<RoomSchedule>)_repo.RoomSchedules,
                StudentTeams = (IQueryable<StudentTeam>)_repo.StudentTeams,
                Teams = (IQueryable<Team>)_repo.Teams,
                TeamSubmissions = (IQueryable<TeamSubmission>)_repo.TeamSubmissions,
                Users = (IQueryable<User>)_repo.Users,
                UserPasswords = (IQueryable<UserPassword>)_repo.UserPasswords
            };

            return View("~/Views/Home/TA/TATesting.cshtml", viewModel);
        }
    }
}
