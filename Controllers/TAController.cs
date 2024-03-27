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
        }

    }
}
