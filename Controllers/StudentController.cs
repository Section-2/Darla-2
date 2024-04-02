using Darla.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// StudentSubmission view

namespace Darla.Controllers
{
    public class StudentController : Controller
    {
        private IIntexRepository _intexRepo;

        public StudentController(IIntexRepository temp)
        {
            _intexRepo = temp;
        }

        public IActionResult StudentDashboard()
        {
            string userId = "7"; // Assuming you will get the user's ID from somewhere.
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
                .Select(st => st.UserId.ToString())
                .ToList();

            // Retrieve the names of the Users with those Ids
            teamMemberNames = _intexRepo.Users
                .Where(u => userIds.Contains(u.UserId.ToString()))
                .Select(u => u.FirstName + " " + u.LastName)
                .ToList();


            // Pass the data to the view using ViewBag
            ViewBag.TeamNumber = teamNumber;
            ViewBag.RoomSchedule = roomSchedule;
            ViewBag.TeamMemberNames = teamMemberNames;

            return View();
        }

        private List<TeamSubmission> GetSubmissions(string userId)
        {
            var teamNumber = _intexRepo.StudentTeams
                .FirstOrDefault(st => st.UserId == userId)?.TeamNumber;

            if (!teamNumber.HasValue)
            {
                throw new Exception("User is not part of a team.");
            }

            List<TeamSubmission> submissions = _intexRepo.TeamSubmissions
                .Where(ts => ts.TeamNumber == teamNumber.Value)
                .Select(ts => new TeamSubmission
                {
                    TeamNumber = ts.TeamNumber,
                    GithubLink = ts.GithubLink,
                    VideoLink = ts.VideoLink,
                    GoogleDocLink = ts.GoogleDocLink,
                })
                .ToList();

            return submissions;
        }

    [HttpGet]
    public IActionResult StudentRubricDetails(int classCode)
    {
        // Retrieve all rubrics with the given classId from the repository
        List<Rubric> rubrics = _intexRepo.Rubrics.Where(r => r.ClassCode == classCode).ToList();

            // Assign the rubrics to the ViewBag
            ViewBag.Rubrics = rubrics;

            return View("StudentRubricDetails");
        }


        [HttpGet]
        public IActionResult StudentProgress()
        {
            string userId = "7";
            var teamNumber = _intexRepo.StudentTeams
                .FirstOrDefault(st => st.UserId == userId)?.TeamNumber;

            if (!teamNumber.HasValue)
            {
                return View("Error", "User is not part of a team.");
            }

            // Throws error: no such column: r.decsription
            List<int> classes = _intexRepo.Rubrics
                .Select(r => r.ClassCode)
                .Distinct()
                .ToList();

            List<TeamSubmission> submissions = GetSubmissions(userId);

            // Retrieve rubrics for each class
            Dictionary<int, List<Rubric>> rubricsByClass = classes.ToDictionary(
                classCode => classCode,
                classCode => _intexRepo.Rubrics.Where(r => r.ClassCode == classCode).ToList()
            );

            ViewBag.Submissions = submissions;
            ViewBag.Classes = classes;
            ViewBag.RubricsByClass = rubricsByClass;

            return View();
        }

        public IActionResult updateCompleteStatus()
        {
            //when an assigments completed check box is click and is emplt or False 
            //then change it to checked and True and vis versa
            //possibly if this is a stylized radio button have this action happen every time the button is clicked uing an event listener of some sort
            //using the assignmentID update the value of the complete attibute of that assignment
            return View("StudentProgress");
        }

        //public IActionResult getSubmissions()
        //{

        //    //this action will check all assignmetns across all ruberics to and get the addignmetns id of those that have a isDeliverable peramiter of True
        //    // so it should take the Group ID as a peramiter so that it can add submisssions to the submissison table the are assosiated with that group

        //    // reference the draw.io for what the submission table looks like
        //    //it should then return a list of submission. this function will be called on the Student progress page
        //    var submissions = [];

        //    return submissions;
        //}

        [HttpPost]
        public async Task<IActionResult> Submit(string githubLink, string videoLink)
        {
            string userId = "7";
            var teamNumber = _intexRepo.StudentTeams
                                 .FirstOrDefault(st => st.UserId == userId)?.TeamNumber ??
                             0; // Provide a default value of 0 if TeamNumber is null

            var submission = _intexRepo.TeamSubmissions
                .FirstOrDefault(s => s.TeamNumber == teamNumber);

            if (submission == null)
            {
                submission = new TeamSubmission
                {
                    TeamNumber = teamNumber,
                    GithubLink = githubLink,
                    VideoLink = videoLink,

                };
                _intexRepo.AddTeamSubmission(submission);
            }
            else
            {
                submission.GithubLink = githubLink;
                submission.VideoLink = videoLink;

            }

            await _intexRepo.SaveChangesAsync();

            TempData["SuccessMessage"] = "Submission updated successfully!";
            return View("StudentProgress");
        }


        public IActionResult GroupPeerEvals()
        {
            string userId = "7"; // Hardcoded userId

            // Find the team number associated with this user
            var teamNumber = _intexRepo.StudentTeams
                .Where(st => st.UserId == userId)
                .Select(st => st.TeamNumber)
                .FirstOrDefault();

            if (teamNumber == 0)
            {
                return View("Error");
            }

            // Get all user IDs that are part of the team, excluding the current user
            var teamMemberIds = _intexRepo.StudentTeams
                .Where(st => st.TeamNumber == teamNumber && st.UserId != userId)
                .Select(st => st.UserId)
                .ToList();

            // Retrieve User objects that match the team member IDs
            var teamMemberUsers = _intexRepo.Users
                .Where(u => teamMemberIds.Contains(u.UserId.ToString()))
                .ToList();

            // Assign the list of User objects to the ViewBag
            ViewBag.TeamMembers = teamMemberUsers;

            return View("StudentGroupPeerEvals");
        }


        [HttpGet]
        public IActionResult StudentPeerReview(string subjectId)
        {
            var userId = 7;
            // Retrieve the User object (subject) with the given ID
            var subject = _intexRepo.Users.FirstOrDefault(u => u.UserId == subjectId.ToString());


            // Retrieve a list of all PeerEvaluationQuestions from the repository or context
            var questions = _intexRepo.PeerEvaluationQuestions.ToList();

            // Pass the subject User object and the list of questions to the view using ViewBag
            ViewBag.Subject = subject;
            ViewBag.PeerEvaluationQuestions = questions;
            ViewBag.evaluatorId = userId;
            return View();
        }




        public IActionResult PeerEvaluation()
        {
            //generate the peer eval quiz
            return View("StudentPeerReview");
        }

        //if (ModelState.IsValid)
        //{ 
        //    foreach (var evaluation in peerEvaluations)
        //    {
        //        _intexRepo.AddPeerEvaluation(evaluation);
        //    }

        //    await _intexRepo.SaveChangesAsync();

        //    // Redirect to a confirmation page or back to the form
        //    return RedirectToAction("GroupPeerEvals"); // Adjust your redirection as necessary
        //}
        //Console.WriteLine("status invalid");

        [HttpPost]
        public async Task<IActionResult> SubmitPeerEvaluation(List<PeerEvaluation> peerEvaluations, int subjectId)
        {
            string evaluatorId = "7"; // Hardcoded evaluatorId for testing

            if (peerEvaluations != null && peerEvaluations.Any())
            {
                foreach (var evaluation in peerEvaluations)
                {
                    var newEvaluation = new PeerEvaluation
                    {
                        EvaluatorId = evaluatorId, // Use the hardcoded evaluatorId
                        SubjectId = evaluation.SubjectId,
                        QuestionId = evaluation.QuestionId,
                        Rating = evaluation.Rating
                    };
                    _intexRepo.AddPeerEvaluation(newEvaluation);
                }

                await _intexRepo.SaveChangesAsync();
                TempData["SuccessMessage"] = "Peer evaluations submitted successfully!";
                return RedirectToAction("StudentDashboard");
            }
            else
            {
                ModelState.AddModelError("", "No evaluations provided.");
            }

            // Redirect to StudentPeerReview action to repopulate ViewBag if there are validation errors
            return RedirectToAction("StudentPeerReview", new { subjectId = subjectId });

        }


    }
}
