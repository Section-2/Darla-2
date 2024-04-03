using Darla.Models;
using Darla.Models.ViewModels;
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

        /* is the Grading summary page supposed to go here? */

        //Views
        public IActionResult AllGrades()
        {
            var TeamInfo = _repo.Teams.ToList();

            return View(TeamInfo);
        }

        public IActionResult TaGradingProgress()
        {
            var GradeInfo = _repo.Grades.ToList();

            return View(GradeInfo);
        }

        public ActionResult TADashboard()
        {
            var assignmentToClassMap = new Dictionary<int, string>
            {
                {1, "IS 401"}, {2, "IS 413"}, {3, "IS 414"}, {4, "IS 455"}, {5, "IS 401"} // and so on...
            };

            // Assuming 'Model' is your data source
            var classGrades = _repo.Grades.ToList()
                .Where(g => assignmentToClassMap.ContainsKey(g.AssignmentId))
                .GroupBy(g => assignmentToClassMap[g.AssignmentId])
                .Select(group => new ClassGradingInfo
                {
                    ClassCode = group.Key,
                    TotalGraded = group.Count(),
                    TotalAssignments = 240 // Assuming each class has 240 total assignments
                })
                .ToList();

            var viewModel = new GradingProgressViewModel
            {
                ClassGradingInfos = classGrades,
                AveragePercentage = classGrades.Average(x => x.PercentageGraded)
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult ClassRubric()
        {
            var rubricRequirements = _repo.GetRubricRequirements().ToList(); // This will execute the query and convert to List<Rubric>

            var grades = rubricRequirements.Select(r => new Grade
            {
                AssignmentId = r.AssignmentId,
                // Initialize other properties of Grade if necessary
            }).ToList();

            var viewModel = new GradeViewModel
            {
                RubricRequirements = rubricRequirements,
                Grades = grades
            };

            return View(viewModel);
        }


        [HttpPost]
        public IActionResult ClassRubric(GradeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                foreach (var grade in viewModel.Grades)
                {
                    // Validate each grade if necessary
                    if (TryValidateModel(grade))
                    {
                        _repo.AddGrade(grade);
                    }
                    else
                    {
                        // Handle the case where individual grades might not be valid
                        // You may want to return immediately or collect all errors before returning
                    }
                }

                // Save changes if using an ORM like Entity Framework
                // _context.SaveChanges();

                // If all grades are valid, or after handling invalid grades
                return RedirectToAction("ClassRubric");
            }
            else
            {
                // ViewBag.categories = _repo.Categories.ToList(); // Uncomment if needed
                return View(viewModel);
            }
        }
    }

}
