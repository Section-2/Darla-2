//using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Darla.Models;
using Microsoft.EntityFrameworkCore;


namespace Darla.Controllers;

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

        // Retrieve the team number associated with the user.
        var teamNumber = _intexRepo.StudentTeams
            .Where(st => st.UserId == userId)
            .Select(st => (int?)st.TeamNumber)
            .FirstOrDefault(); // Synchronous version of FirstOrDefaultAsync

        // ... your additional logic to get the appointment information

        // Pass the data to the view if necessary.
        // Pass the team number to the view using ViewBag
        ViewBag.TeamNumber = teamNumber;


        return View();
    }

    public IActionResult StudentProgress()
    {
        //get all unique class codes from the rubric table and add them to a list classed classcodes that is passed to the view
        //get get the userID from the session
        //get the TeamNumnber from StudentTeams where userID matches userID
        //this is the page that shows the classes
        // it needs to pull the classes that will be graded. possibly the classes that the students are enrolled in. jsut assume you are pulling all classes from the db
        //then from those classes it needs to dynamically pull the ruberic for each class in a list that can be clicked to take the user to that ruberic's details

        //send submission in a viewbag to the page to dynamically appear on the submissions part of the studetn progress page
        //      var submissions = getSubmissions();

        var userId = 1; // Replace with actual user identification logic.
        var classes = _intexRepo.Rubrics
       .Select(r => r.ClassCode) // Project each Rubric to its ClassCode.
       .Distinct() // Ensure each class code is unique.
       .ToList(); // Execute the query and convert the result to a List.
                  //var submissions = GetSubmissions(userId);
                  //ViewBag.Submissions = submissions;

        return View(classes);
        
    }

    public IActionResult RubericDetails()
    {
        //when you click on a class ruberic
        // then it needs to dynamically pull all assignments asssosiated with the ruberic id
        //          each assignment has the attributes assignmentID:int, rubericID:int, completed: bool, pintsOnGrade: int, isDeliverable:bool, description:string
        //          the description, points, and complete need to be displayed for each assignment with the complete states bring determind by a chekc box. this may be another action called updateCompletedStatus


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
