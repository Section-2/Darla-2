﻿using Darla.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Darla.Models2;

// StudentSubmission view

namespace Darla.Controllers
{
    public class StudentController : Controller
    {
        private IIntexRepository _repo;

        public StudentController(IIntexRepository temp)
        {
            _repo = temp;
        }
        
        public IActionResult StudentDashboard()
        {
            var userId = "this is a hard coded example";
            Console.WriteLine("You made it to student dashboard\n \n \n" + userId);

            var userFirstName = "Blake";
            List<string> teamMemberNames = new List<string> { "Blake McAvoy", "Hannah Johnson", "Elijah Aken", "Steven Armstrong" };
            var teamNumber = "202";
            var hardCode = "THIS IS A HARD CODED EXAMPLE";
            var uncomment = "UNCOMMENT THE OTHER METHOD LATER";

            // Pass the data to the view using ViewBag
            ViewBag.FirstName = userFirstName;
            ViewBag.TeamMemberNames = teamMemberNames;
            ViewBag.RoomSchedule = new { team_number = teamNumber, timeslot = hardCode };
            ViewBag.RoomAssignment = uncomment;

            return View();
        }

        // public IActionResult StudentDashboard()
        // {
        //     // assuming you will get the user's id from somewhere
        //     string userId = "55278449-2657-4dea-96cc-ed05914d0a1b";
        //     Console.WriteLine("You made it to student dashboard\n \n \n" + userId);
        //     
        //     // get the user's name
        //     var userFirstName = _repo.AspNetUsers
        //         .Where(st => st.Id == userId)
        //         .Select(st => st.FirstName)
        //         .FirstOrDefault();
        //     
        //     // get the team number
        //     var teamNumber = _repo.student_team
        //         .Where(st => st.user_id == userId)
        //         .Select(st => st.team_number)
        //         .FirstOrDefault();
        //     
        //     // get team member user ids
        //     List<string> teamMemberIds = _repo.student_team
        //         .Where(st => st.team_number == teamNumber)
        //         .Select(st => st.user_id)
        //         .ToList();
        //
        //     // get matching names for team ids
        //     List<string> teamMemberNames = _repo.AspNetUsers
        //         .Where(st => teamMemberIds.Contains(st.Id))
        //         .Select(st => $"{st.FirstName} {st.LastName}")
        //         .ToList();
        //
        //     // get room schedule for team
        //     RoomSchedule roomSchedule = _repo.room_schedule
        //         .FirstOrDefault(rs => rs.team_number == teamNumber);
        //
        //     // get room name for room id
        //     var roomAssignment = _repo.room
        //         .Where(st => st.room_id == roomSchedule.room_id)
        //         .Select(st => st.room_name)
        //         .FirstOrDefault();
        //
        //     // Pass the data to the view using ViewBag
        //     ViewBag.FirstName = userFirstName;
        //     ViewBag.TeamMemberNames = teamMemberNames;
        //     ViewBag.RoomSchedule = roomSchedule;
        //     ViewBag.RoomAssignment = roomAssignment;
        //     
        //     return View();
        // }

        private List<TeamSubmission> GetSubmissions(string userId)
        {

            var teamNumber = _repo.StudentTeams
                .FirstOrDefault(st => st.UserId == userId)?.TeamNumber;

            if (!teamNumber.HasValue)
            {
                throw new Exception("User is not part of a team.");
            }

            List<TeamSubmission> submissions = _repo.TeamSubmissions
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
        List<Rubric> rubrics = _repo.Rubrics.Where(r => r.ClassCode == classCode).ToList();

            // Assign the rubrics to the ViewBag
            ViewBag.Rubrics = rubrics;

            return View("StudentRubricDetails");
        }


        [HttpGet]
        public IActionResult StudentProgress()
        {  
            string userId = (string)TempData["UserId"];
            var teamNumber = _repo.StudentTeams
                .FirstOrDefault(st => st.UserId == userId)?.TeamNumber;

            if (!teamNumber.HasValue)
            {
                return View("Error", "User is not part of a team.");
            }

            // Throws error: no such column: r.decsription
            List<int> classes = _repo.Rubrics
                .Select(r => r.ClassCode)
                .Distinct()
                .ToList();

            List<TeamSubmission> submissions = GetSubmissions(userId);

            // Retrieve rubrics for each class
            Dictionary<int, List<Rubric>> rubricsByClass = classes.ToDictionary(
                classCode => classCode,
                classCode => _repo.Rubrics.Where(r => r.ClassCode == classCode).ToList()
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
            string userId = (string)TempData["UserId"];
            var teamNumber = _repo.StudentTeams
                                 .FirstOrDefault(st => st.UserId == userId)?.TeamNumber ??
                             0; // Provide a default value of 0 if TeamNumber is null

            var submission = _repo.TeamSubmissions
                .FirstOrDefault(s => s.TeamNumber == teamNumber);

            if (submission == null)
            {
                submission = new TeamSubmission
                {
                    TeamNumber = teamNumber,
                    GithubLink = githubLink,
                    VideoLink = videoLink,

                };
                _repo.AddTeamSubmission(submission);
            }
            else
            {
                submission.GithubLink = githubLink;
                submission.VideoLink = videoLink;

            }

            await _repo.SaveChangesAsync();

            TempData["SuccessMessage"] = "Submission updated successfully!";
            return View("StudentProgress");
        }
        // ready for _repo update

        //[HttpPost]
        //public async Task<IActionResult> Submit(string githubLink, string videoLink)
        //{

        //    string userId = (string)TempData["UserId"];
        //    var teamNumber = _intexRepo.student_team
        //                         .FirstOrDefault(st => st.user_id == userId)?.team_number ??
        //                     0; // Provide a default value of 0 if TeamNumber is null
        //    var submission = _intexRepo.team_submission
        //        .FirstOrDefault(s => s.team_number == teamNumber);
        //    if (submission == null)
        //    {
        //        submission = new team_submission
        //        {
        //            team_number = teamNumber,
        //            github_link = githubLink,
        //            video_link = videoLink,
        //            google_doc_link = docLink,
        //        };
        //        _intexRepo.AddTeamSubmission(submission);
        //    }
        //    else
        //    {
        //        submission.github_link = githubLink;
        //        submission.video_link = videoLink;
        //        submission.google_doc_link = docLink;
        //    }
        //    await _intexRepo.SaveChangesAsync();
        //    TempData["SuccessMessage"] = "Submission updated successfully!";
        //    return View("StudentProgress");
        //}


        public IActionResult GroupPeerEvals()
        {
            string userId = (string)TempData["UserId"]; // Hardcoded userId

            // Find the team number associated with this user
            var teamNumber = _repo.StudentTeams
                .Where(st => st.UserId == userId)
                .Select(st => st.TeamNumber)
                .FirstOrDefault();

            if (teamNumber == 0)
            {
                return View("Error");
            }

            // Get all user IDs that are part of the team, excluding the current user
            var teamMemberIds = _repo.StudentTeams
                .Where(st => st.TeamNumber == teamNumber && st.UserId != userId)
                .Select(st => st.UserId)
                .ToList();

            // Retrieve User objects that match the team member IDs
            var teamMemberUsers = _repo.Users
                .Where(u => teamMemberIds.Contains(u.UserId.ToString()))
                .ToList();

            // Assign the list of User objects to the ViewBag
            ViewBag.TeamMembers = teamMemberUsers;

            return View("StudentGroupPeerEvals");
        }


        [HttpGet]
        public IActionResult StudentPeerReview(string subjectId)
        {
            string userId = (string)TempData["UserId"];
            // Retrieve the User object (subject) with the given ID
            var subject = _repo.Users.FirstOrDefault(u => u.UserId == subjectId.ToString());


            // Retrieve a list of all PeerEvaluationQuestions from the repository or context
            var questions = _repo.PeerEvaluationQuestions.ToList();

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
            string evaluatorId = (string)TempData["UserId"]; // Hardcoded evaluatorId for testing

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
                    _repo.AddPeerEvaluation(newEvaluation);
                }

                await _repo.SaveChangesAsync();
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
