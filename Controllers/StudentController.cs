using Microsoft.AspNetCore.Mvc;
using Darla.Models;
using Microsoft.EntityFrameworkCore;



public class StudentController : Controller
{
    private IIntexRepository _intexRepo;

    public StudentController(IIntexRepository temp)
    {
        _intexRepo = temp;
    }

        public IActionResult StudentDashboard()
        {
            var userId = 1; // Assuming you will get the user's ID from somewhere.
            var teamNumber = _intexRepo.StudentTeams
                .Where(st => st.UserId == userId)
                .Select(st => (int?)st.TeamNumber)
                .FirstOrDefault();


            List<string> teamMemberNames = new List<string>();


            RoomSchedule roomSchedule = _intexRepo.RoomSchedules
                .FirstOrDefault(rs => rs.TeamNumber == teamNumber.Value);

            // Get the list of UserIds for the team
            var userIds = _intexRepo.StudentTeams
                .Where(st => st.TeamNumber == teamNumber.Value)
                .Select(st => st.UserId)
                .ToList();

            // Retrieve the names of the Users with those Ids
            teamMemberNames = _intexRepo.Users
                .Where(u => userIds.Contains(u.UserId))
                .Select(u => u.FirstName + " " + u.LastName)
                .ToList();


            // Pass the data to the view using ViewBag
            ViewBag.TeamNumber = teamNumber;
            ViewBag.RoomSchedule = roomSchedule;
            ViewBag.TeamMemberNames = teamMemberNames;

            return View();
        }
        // [HttpGet]
        // public IActionResult StudentProgress()
        // {
        //     //get all unique class codes from the rubric table and add them to a list classed classcodes that is passed to the view
        //     //get get the userID from the session
        //     //get the TeamNumnber from StudentTeams where userID matches userID
        //     //this is the page that shows the classes
        //     // it needs to pull the classes that will be graded. possibly the classes that the students are enrolled in. just assume you are pulling all classes from the db
        //     //then from those classes it needs to dynamically pull the ruberic for each class in a list that can be clicked to take the user to that ruberic's details
        //
        //     //send submission in a viewbag to the page to dynamically appear on the submissions part of the studetn progress page
        //     //      var submissions = getSubmissions();
        //
        //    // var userId = 1; // Replace with actual user identification logic.
        //     var classes = _intexRepo.Rubrics
        //    .Select(r => r.ClassCode) // Project each Rubric to its ClassCode.
        //    .Distinct() // Ensure each class code is unique.
        //    .ToList(); // Execute the query and convert the result to a List.
        //               //var submissions = GetSubmissions(userId);
        //               //ViewBag.Submissions = submissions;
        //     return View(classes);
        // }
        private List<TeamSubmission> GetSubmissions(int userId)
        {
            var teamNumber = _context.StudentTeams
                .FirstOrDefault(st => st.UserId == userId)?.TeamNumber;

            if (!teamNumber.HasValue)
            {
                throw new Exception("User is not part of a team.");
            }

            List<TeamSubmission> submissions = _context.TeamSubmissions
                .Where(ts => ts.TeamNumber == teamNumber.Value)
                .Select(ts => new TeamSubmission
                {
                    TeamNumber = ts.TeamNumber,
                    GithubLink = ts.GithubLink,
                    VideoLink = ts.VideoLink,
                    Timestamp = ts.Timestamp
                })
                .ToList();
        //send submission in a viewbag to the page to dynamically appear on the submissions part of the studetn progress page
        //      var submissions = getSubmissions();

        // var userId = 1; // Replace with actual user identification logic.
        var classes = _intexRepo.Rubrics
       .Select(r => r.ClassCode) // Project each Rubric to its ClassCode.
       .Distinct() // Ensure each class code is unique.
       .ToList(); // Execute the query and convert the result to a List.
                  //var submissions = GetSubmissions(userId);
                  //ViewBag.Submissions = submissions;

        return View(classes);

    }
    [HttpGet]
    public IActionResult RubricDetails(int classCode)
    {
        // Retrieve all rubrics with the given classId from the repository
        List<Rubric> rubrics = _intexRepo.Rubrics.Where(r => r.ClassCode == classCode).ToList();

        // Assign the rubrics to the ViewBag
        ViewBag.Rubrics = rubrics;

        return View();
    }
            return submissions;
        }

        [HttpGet]
        public IActionResult StudentProgress()
        {
            var userId = 1; 

            var teamNumber = _context.StudentTeams
                .FirstOrDefault(st => st.UserId == userId)?.TeamNumber;

            if (!teamNumber.HasValue)
            {
                return View("Error", "User is not part of a team.");
            }

            List<int> classes = _context.Rubrics
                .Select(r => r.ClassCode)
                .Distinct()
                .ToList();

            List<TeamSubmission> submissions = GetSubmissions(userId);
            ViewBag.Submissions = submissions;
            ViewBag.Classes = classes;
            return View();
        }
    }

    public IActionResult PeerEvaluation()
    {
        //generate the peer eval quiz
        return View();
    }

