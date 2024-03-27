using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Darla.Models;

namespace Darla.Controllers;

public class StudentController : Controller
{
    private readonly IntexGraderContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public StudentController(IntexGraderContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public class DashboardViewModel
    {
        public TimeSpan Countdown { get; set; }
        public string RoomNumber { get; set; }
        public string PresentationTime { get; set; }
        public int TeamNumber { get; set; }
        public List<string> TeamMembers { get; set; }
    }

    private DashboardViewModel GetDashboardData(int userId)
    {
        var teamNumber = _context.StudentTeams
            .FirstOrDefault(st => st.UserId == userId)?.TeamNumber ?? throw new Exception("User is not part of a team.");

        var roomSchedule = _context.RoomSchedules
            .Include(rs => rs.Room)
            .FirstOrDefault(rs => rs.TeamNumber == teamNumber) ?? throw new Exception("No room schedule found for the team.");

        var teamMembers = _context.StudentTeams
            .Where(st => st.TeamNumber == teamNumber)
            .Select(st => st.User.FirstName + " " + st.User.LastName)
            .ToList();

        var presentationTime = DateTime.Parse(roomSchedule.Timeslot);
        var countdown = presentationTime - DateTime.Now;

        return new DashboardViewModel
        {
            Countdown = countdown,
            RoomNumber = roomSchedule.Room.RoomId.ToString(),
            PresentationTime = roomSchedule.Timeslot,
            TeamNumber = teamNumber,
            TeamMembers = teamMembers
        };
    }

    public IActionResult StudentDashboard()
    {
        var userId = _httpContextAccessor.HttpContext.User.Identity.GetUserId();
        var dashboardData = GetDashboardData(userId);
        return View(dashboardData);
    }
}

 public IActionResult StudentProgress()
    {
        var userId = User.Identity.GetUserId(); // Replace with actual user identification logic.
        var classes = _context.Classes
            .Include(c => c.Rubrics) // Assuming each Class entity has a navigation property to its Rubrics.
            .ToList();
        
        var submissions = GetSubmissions(userId);
        ViewBag.Submissions = submissions;

        return View(classes);
    }

    public IActionResult RubericDetails(int rubricId)
    {
        var rubricDetails = _context.Rubrics
            .Where(r => r.RubricId == rubricId)
            .Include(r => r.Assignments)
            .FirstOrDefault();

        return View(rubricDetails);
    }

    [HttpPost]
    public IActionResult UpdateCompleteStatus(int assignmentId, bool completed)
    {
        var assignment = _context.Assignments
            .FirstOrDefault(a => a.AssignmentId == assignmentId);
        
        if (assignment != null)
        {
            assignment.Completed = completed;
            _context.SaveChanges();
        }

        // Redirect back to the RubericDetails or another appropriate page.
        return RedirectToAction("RubericDetails", new { rubricId = assignment?.RubricId });
    }

    private List<TeamSubmission> GetSubmissions(int userId)
    {
        var groupId = _context.Groups
            .Where(g => g.Students.Any(s => s.UserId == userId))
            .Select(g => g.GroupId)
            .FirstOrDefault();

        var submissions = _context.Submissions
            .Where(s => s.GroupId == groupId && s.Assignment.IsDeliverable)
            .ToList();

        return submissions;
    }

    [HttpPost]
    public IActionResult Submit(int groupId, int assignmentId, IFormFile file)
    {
        // Handle file upload logic here.
        
        var submission = _context.Submissions
            .FirstOrDefault(s => s.GroupId == groupId && s.AssignmentId == assignmentId);

        if (submission != null)
        {
            submission.Complete = true;
            // Handle versioning logic if necessary.
            _context.SaveChanges();
        }
        
        // Redirect to an appropriate page after the submission.
        return RedirectToAction("StudentProgress");
    }


    // public IActionResult StudentDashboard()
    // {
    //
    //     //this has to load a count down timer based the present time and the appointment time
    //
    //     //It has to pull the appointment information assosiated with the student's group.
    //     //       this includes group number, locaiton/roomnumber, time of appointment, 
    //     
    //     //list the group number and the group members
    //     //  
    //
    //
    //
    //
    //     return View();
    // }

    // public IActionResult StudentProgress()
    // {
    //     //this is the page that shows the ruberic
    //     // it needs to pull the classes that will be graded. possibly the classes that the students are enrolled in. just assume you are pulling all classes from the db
    //     //then from those classes it needs to dynamically pull the ruberic for each class in a list that can be clicked to take the user to that ruberic's details
    //    
    //     //send submission in a viewbag to the page to dynamically appear on the submissions part of the studetn progress page
    //     //      var submissions = getSubmissions();
    //      return View();
    // }
    //
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
    //
    // public IActionResult getSubmissions()
    // {
    //
    //     //this action will check all assignmetns across all ruberics to and get the addignmetns id of those that have a isDeliverable peramiter of True
    //     // so it should take the Group ID as a peramiter so that it can add submisssions to the submissison table the are assosiated with that group
    //
    //     // reference the draw.io for what the submission table looks like
    //     //it should then return a list of submission. this function will be called on the Student progress page
    //     var submissions = [];
    //
    //     return submissions;
    // }
    //  
    // public IActionResult submit(groupID, assignmentID ,file)
    // {
    //     //this function needs to be able to receive the group ID, the assignmentID, and the file and add those to the submission that matches the groupID and assignmetnID
    //     //then it updates the compelete status of the submission to true, 
    //     
    //     //optional:
    //     //if the complete status is true then make a copy of that submission and incremetn the submissionVersion value by so that multiple same submissions can be differentiated by submissionVersion 
    //     return View();
    // }

