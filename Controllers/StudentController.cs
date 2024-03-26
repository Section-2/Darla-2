using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Darla.Models;

namespace Darla.Controllers;

public class StudentController : Controller
{
  

    public IActionResult StudentDashboard()
    {

        //this has to load a count down timer based the present time and the appointment time

        //It has to pull the appointment information assosiated with the student's group.
        //       this includes group number, locaiton/roomnumber, time of appointment, 
        
        //list the group number and the group members
        //




        return View();
    }

    public IActionResult StudentProgress()
    {
        //this is the page that shows the ruberic
        // it needs to pull the classes that will be graded. possibly the classes that the students are enrolled in. jsut assume you are pulling all classes from the db
        //then from those classes it needs to dynamically pull the ruberic for each class in a list that can be clicked to take the user to that ruberic's details
       
        return View();
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


     
}
