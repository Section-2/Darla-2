using Darla.Models;
using Microsoft.AspNetCore.Mvc;

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
   
    private List<TeamSubmission> GetSubmissions(int userId)
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
                Timestamp = ts.Timestamp
            })
            .ToList();

        return submissions;
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
        return View();
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

    public IActionResult submit()
    {
        //this function needs to be able to receive the group ID, the assignmentID, and the file and add those to the submission that matches the groupID and assignmetnID
        //then it updates the compelete status of the submission to true, 

        //optional:
        //if the complete status is true then make a copy of that submission and incremetn the submissionVersion value by so that multiple same submissions can be differentiated by submissionVersion 
        return View();
    }

    public IActionResult GroupPeerEvals()
    {
        int userId = 7; // Hardcoded userId

        // Find the team number associated with this user
        var teamNumber = _intexRepo.StudentTeams
            .Where(st => st.UserId == userId)
            .Select(st => st.TeamNumber)
            .FirstOrDefault();

        if (teamNumber == 0)
        {
            return View("Error", new { message = "User is not part of a team." });
        }

        // Get all user IDs that are part of the team, excluding the current user
        var teamMemberIds = _intexRepo.StudentTeams
            .Where(st => st.TeamNumber == teamNumber && st.UserId != userId)
            .Select(st => st.UserId)
            .ToList();

        // Retrieve User objects that match the team member IDs
        var teamMemberUsers = _intexRepo.Users
            .Where(u => teamMemberIds.Contains(u.UserId))
            .ToList();

        // Assign the list of User objects to the ViewBag
        ViewBag.TeamMembers = teamMemberUsers;

        return View();
    }


    [HttpGet]
    public IActionResult StudentPeerReview(int subjectId)
    {
        var userId = 7;
        // Retrieve the User object (subject) with the given ID
        var subject = _intexRepo.Users.FirstOrDefault(u => u.UserId == subjectId);
       

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
        return View();
    }

    //[HttpPost]
    //public IActionResult SubmitPeerEvaluation(List<PeerEvaluation> peerEvaluations)
    //{


    //    foreach (var evaluation in peerEvaluations)
    //    {
    //        _intexRepo.AddPeerEvaluation(evaluation);


    //        _intexRepo.SaveChanges(); // Or await _intexRepo.SaveChangesAsync(); for async
    //        Console.WriteLine("one eval submit");
    //    }
    //        // Redirect to a confirmation page or back to the form
    //        return RedirectToAction("GroupPeerEvals"); // Adjust your redirection as necessary

    //}

    [HttpPost]
    public async Task<IActionResult> AddHardcodedPeerEvaluation()
    {
        // Create the PeerEvaluation object with hardcoded values
        var hardcodedEvaluation = new PeerEvaluation
        {
            EvaluatorId = 1,
            SubjectId = 7,
            QuestionId = 1,
            Rating = 1
        };

        // Check if the hardcoded foreign keys are valid before adding
        bool evaluatorExists = _intexRepo.StudentTeams.Any(st => st.UserId == hardcodedEvaluation.EvaluatorId);
        Console.WriteLine(evaluatorExists);
        bool subjectExists = _intexRepo.StudentTeams.Any(st => st.UserId == hardcodedEvaluation.SubjectId);
        Console.WriteLine(subjectExists);
        bool questionExists = _intexRepo.PeerEvaluationQuestions.Any(q => q.QuestionId == hardcodedEvaluation.QuestionId);
        Console.WriteLine(questionExists);

        if (evaluatorExists && subjectExists && questionExists)
        {
            // Add the new PeerEvaluation to the repository
            _intexRepo.AddPeerEvaluation(hardcodedEvaluation);

            // Save changes in the repository
            await _intexRepo.SaveChangesAsync();

            // Redirect to a success page or another action method as required
            return RedirectToAction("GroupPeerEvals"); // Adjust as necessary
        }
        else
        {
            // Handle the error if foreign keys are not valid (e.g., return an error view)
            return View("Error", new { message = "Invalid foreign key values." });
        }
    }



}
