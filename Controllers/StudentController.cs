using Darla.Models;
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

    public IActionResult StudentPeerReview()
    {

        //This view needs to pull the group info and so that each team memer can be seen and selected to be peer reviewd by the user.
        // so just return a variable to the view that holds the student info where group ID matches the group ID of the user
        return View();
    }

    public IActionResult PeerEvaluation()
    {
        //generate the peer eval quiz
        return View();
    }

    public IActionResult SubmitPeerEval()
    {
        //submit the eval, update the data base
        //retrun to the StudentPeerReview view
        return View();
    }

}
