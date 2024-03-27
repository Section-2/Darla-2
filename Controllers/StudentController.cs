using Microsoft.AspNetCore.Mvc;
using Darla.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;



namespace Darla.Controllers
{
    public class StudentController : Controller
    {
        private readonly IIntexRepository _intexRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IntexGraderContext _context;

        public StudentController(IIntexRepository intexRepo, IHttpContextAccessor httpContextAccessor,
            IntexGraderContext context)
        {
            _intexRepo = intexRepo;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
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

            return submissions;
        }

        [HttpGet]
        public IActionResult StudentProgress()
        {
            var userId = 1;

            var teamNumber = _intexRepo.StudentTeams
                .FirstOrDefault(st => st.UserId == userId)?.TeamNumber;

            if (!teamNumber.HasValue)
            {
                return View("Error", "User is not part of a team.");
            }

            List<int> classes = _intexRepo.Rubrics
                .Select(r => r.ClassCode)
                .Distinct()
                .ToList();

            List<TeamSubmission> submissions = GetSubmissions(userId);
            ViewBag.Submissions = submissions;
            ViewBag.Classes = classes;
            return View();
        }

        [HttpGet]
        public IActionResult StudentPeerReview()
        {
            var userId = 1; 
            // Get the team number for the current user
            var teamNumber = _intexRepo.StudentTeams
                .FirstOrDefault(st => st.UserId == userId)?.TeamNumber;

            if (!teamNumber.HasValue)
            {
                return View("Error", "User is not part of a team.");
            }

            // Get the peer evaluation questions
            List<PeerEvaluationQuestion> questions = _intexRepo.PeerEvaluationQuestions
                .ToList(); 

            // Get the team members for the user's team
            var teamMembers = _intexRepo.StudentTeams
                .Where(st => st.TeamNumber == teamNumber.Value)
                .Select(st => st.User) // Assuming there is a navigation property 'User' in 'StudentTeam'
                .ToList();

            return View(viewModel);
        }

    }

    // public IActionResult PeerEvaluation()
    // {
    //     //generate the peer eval quiz
    //     return View();
    // }
    //
    // public IActionResult SubmitPeerEval()
    // {
    //     //submit the eval, update the data base
    //     //retrun to the StudentPeerReview view
    //     return View();
    // }


        // public IActionResult RubericDetails()
        // {
        //     //when you click on a class ruberic
        //     // then it needs to dynamically pull all assignments asssosiated with the ruberic id
        //     //          each assignment has the attributes assignmentID:int, rubericID:int, completed: bool, pintsOnGrade: int, isDeliverable:bool, description:string
        //     //          the description, points, and complete need to be displayed for each assignment with the complete states bring determind by a chekc box. this may be another action called updateCompletedStatus
        //
        //
        //     return View();
        // }
        //
        // public IActionResult updateCompleteStatus()
        // {
        //     //when an assigments completed check box is click and is emplt or False 
        //     //then change it to checked and True and vis versa
        //     //possibly if this is a stylized radio button have this action happen every time the button is clicked uing an event listener of some sort
        //     //using the assignmentID update the value of the complete attibute of that assignment
        //     return View();
        // }
        //
        // //public IActionResult getSubmissions()
        // //{
        //
        // //    //this action will check all assignmetns across all ruberics to and get the addignmetns id of those that have a isDeliverable peramiter of True
        // //    // so it should take the Group ID as a peramiter so that it can add submisssions to the submissison table the are assosiated with that group
        //
        // //    // reference the draw.io for what the submission table looks like
        // //    //it should then return a list of submission. this function will be called on the Student progress page
        // //    var submissions = [];
        //
        // //    return submissions;
        // //}
        //
        // public IActionResult submit()
        // {
        //     //this function needs to be able to receive the group ID, the assignmentID, and the file and add those to the submission that matches the groupID and assignmetnID
        //     //then it updates the compelete status of the submission to true, 
        //
        //     //optional:
        //     //if the complete status is true then make a copy of that submission and incremetn the submissionVersion value by so that multiple same submissions can be differentiated by submissionVersion 
        //     return View();
        // }
        //
        
    }

