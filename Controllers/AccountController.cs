using Darla.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text.Encodings.Web;

namespace Darla.Controllers
{
    public class AccountController : Controller
    {
        //userManager will hold the UserManager instance
        private readonly UserManager<ApplicationUser> userManager;

        //signInManager will hold the SignInManager instance
        private readonly SignInManager<ApplicationUser> signInManager;

        //ISenderEmail will hold the EmailSender instance
        private readonly ISenderEmail emailSender;

        // UserManager, SignInManager and EmailSender services are injected into the AccountController
        // using constructor injection
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ISenderEmail emailSender)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailSender = emailSender;
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Copy data from RegisterViewModel to ApplicationUser
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };

                // Store user data in AspNetUsers database table
                var result = await userManager.CreateAsync(user, model.Password);

                // If user is successfully created, sign-in the user using
                // SignInManager and redirect to index action of HomeController
                if (result.Succeeded)
                {
                    //Then send the Confirmation Email to the User
                    await SendConfirmationEmail(model.Email, user);

                    // If the user is signed in and in the Admin role, then it is
                    // the Admin user that is creating a new user. 
                    // So, redirect the Admin user to ListUsers action of Administration Controller
                    if (signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                    {
                        return RedirectToAction("ListUsers", "Administration");
                    }

                    //If it is not Admin user, then redirect the user to RegistrationSuccessful View
                    return View("RegistrationSuccessful");
                }

                // If there are any errors, add them to the ModelState object
                // which will be displayed by the validation summary tag helper
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string? ReturnUrl = null)
        {
            ViewData["ReturnUrl"] = ReturnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string? ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                // Attempt to sign in the user using their username and password
                var user = await userManager.FindByNameAsync(model.Username);
                var userEmail = user.Email;

                if (user != null && !user.EmailConfirmed && (await userManager.CheckPasswordAsync(user, model.Password)))
                {
                    ModelState.AddModelError(string.Empty, "Email not confirmed yet");
                    return View();
                }

                var result = await signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    // Handle successful login
                    // Check if the ReturnUrl is not null and is a local URL
                    if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }
                    else
                    {
                        // Redirect to default page
                        return RedirectToAction("StudentDashboard", "Home");
                    }
                }
                if (result.RequiresTwoFactor)
                {
                    // Handle two-factor authentication case
                    // Generate a 2FA token, send that token to user Email and Phone Number
                    // and redirect to the 2FA verification view
                    var TwoFactorAuthenticationToken = await userManager.GenerateTwoFactorTokenAsync(user, "Email");

                    //Sending Email
                    await emailSender.SendEmailAsync(user.Email, "2FA Token", $"Your 2FA Token is {TwoFactorAuthenticationToken}", false);

                    return RedirectToAction("VerifyTwoFactorToken", "Account", new { Email = userEmail, ReturnUrl, model.RememberMe, TwoFactorAuthenticationToken });
                }
                if (result.IsLockedOut)
                {
                    // Handle locked out user
                    // You can redirect to a lockout view or display an error message
                    return View("Lockout");
                }
                else
                {
                    // Invalid login attempt
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        [HttpGet]
        public async Task<IActionResult> IsEmailAvailable(string Email)
        {
            //Check If the Email is Already in the Database
            var user = await userManager.FindByEmailAsync(Email);

            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"The email '{Email}' is already in use.");
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [HttpGet]
        public async Task<IActionResult> IsUsernameAvailable(string UserName)
        {
            //Check If the username is Already in the Database
            var user = await userManager.FindByNameAsync(UserName);

            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"The username '{UserName}' is already in use.");
            }
        }

        [HttpGet]
        public IActionResult InsufficientPrivileges(string? ReturnUrl = null)
        {
            ViewData["ReturnUrl"] = ReturnUrl;
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        private async Task SendForgotPasswordEmail(string? email, ApplicationUser? user)
        {
            // Generate the reset password token
            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            //save the token into the AspNetUserTokens database table
            await userManager.SetAuthenticationTokenAsync(user, "ResetPassword", "ResetPasswordToken", token);

            // Build the password reset link which must include the Callback URL
            // Build the password reset link
            var passwordResetLink = Url.Action("ResetPassword", "Account",
                    new { Email = email, Token = token }, protocol: HttpContext.Request.Scheme);

            //Send the Confirmation Email to the User Email Id
            await emailSender.SendEmailAsync(email, "Reset Your Password", $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(passwordResetLink)}'>clicking here</a>.", true);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Find the user by email
                var user = await userManager.FindByEmailAsync(model.Email);

                // If the user is found AND Email is confirmed
                if (user != null && await userManager.IsEmailConfirmedAsync(user))
                {
                    await SendForgotPasswordEmail(user.Email, user);

                    // Send the user to Forgot Password Confirmation view
                    return RedirectToAction("ForgotPasswordConfirmation", "Account");
                }

                // To avoid account enumeration and brute force attacks, don't
                // reveal that the user does not exist or is not confirmed
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string Token, string Email)
        {
            // If password reset token or email is null, most likely the
            // user tried to tamper the password reset link
            if (Token == null || Email == null)
            {
                ViewBag.ErrorTitle = "Invalid Password Reset Token";
                ViewBag.ErrorMessage = "The Link is Expired or Invalid";
                return View("Error");
            }
            else
            {
                ResetPasswordViewModel model = new ResetPasswordViewModel();
                model.Token = Token;
                model.Email = Email;
                return View(model);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Find the user by email
                var user = await userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    // reset the user password
                    var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);

                    if (result.Succeeded)
                    {
                        //Once the Password is Reset, remove the token from the database
                        await userManager.RemoveAuthenticationTokenAsync(user, "ResetPassword", "ResetPasswordToken");
                        return RedirectToAction("ResetPasswordConfirmation", "Account");
                    }

                    // Display validation errors. For example, password reset token already
                    // used to change the password or password complexity rules not met
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }

                // To avoid account enumeration and brute force attacks, don't
                // reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            // Display validation errors if model state is not valid
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        private async Task SendConfirmationEmail(string? email, ApplicationUser? user)
        {
            //Generate the Token
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

            //Build the Email Confirmation Link which must include the Callback URL
            var ConfirmationLink = Url.Action("ConfirmEmail", "Account", new { UserId = user.Id, Token = token }, protocol: HttpContext.Request.Scheme);

            //Send the Confirmation Email to the User Email Id
            await emailSender.SendEmailAsync(email, "Confirm Your Email", $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(ConfirmationLink)}'>clicking here</a>.", true);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string UserId, string Token)
        {
            if (UserId == null || Token == null)
            {
                ViewBag.Message = "The link is Invalid or Expired";
            }

            //Find the User By Id
            var user = await userManager.FindByIdAsync(UserId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"The User ID {UserId} is Invalid";
                return View("NotFound");
            }

            //Call the ConfirmEmailAsync Method which will mark the Email as Confirmed
            var result = await userManager.ConfirmEmailAsync(user, Token);

            if (result.Succeeded)
            {
                ViewBag.Message = "Thank you for confirming your email";
                return View();
            }

            ViewBag.Message = "Email cannot be confirmed";
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResendConfirmationEmail(bool IsResend = false)
        {
            if (IsResend)
            {
                ViewBag.Message = "Resend Confirmation Email";
            }
            else
            {
                ViewBag.Message = "Send Confirmation Email";
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendConfirmationEmail(string Email)
        {
            var user = await userManager.FindByEmailAsync(Email);
            if (user == null || await userManager.IsEmailConfirmedAsync(user))
            {
                // Handle the situation when the user does not exist or Email already confirmed.
                // For security, don't reveal that the user does not exist or Email is already confirmed
                return View("ConfirmationEmailSent");
            }

            //Then send the Confirmation Email to the User
            await SendConfirmationEmail(Email, user);

            return View("ConfirmationEmailSent");
        }

        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                //fetch the User Details
                var user = await userManager.GetUserAsync(User);

                if (user == null)
                {
                    //If User does not exists, redirect to the Login Page
                    return RedirectToAction("Login", "Account");
                }

                // ChangePasswordAsync Method changes the user password
                var result = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

                // The new password did not meet the complexity rules or the current password is incorrect.
                // Add these errors to the ModelState and rerender ChangePassword view
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return View();
                }

                // Upon successfully changing the password refresh sign-in cookie
                await signInManager.RefreshSignInAsync(user);

                //Then redirect the user to the ChangePasswordConfirmation view
                return RedirectToAction("ChangePasswordConfirmation", "Account");
            }

            return View(model);
        }

        [Authorize]
        [HttpGet]
        public IActionResult ChangePasswordConfirmation()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ManageTwoFactorAuthentication()
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            //First, we need to check whether the User Phone and Email is confirmed or not
            if (!user.EmailConfirmed)
            {
                ViewBag.ErrorTitle = "You cannot Enable/Disable Two Factor Authentication";
                ViewBag.ErrorMessage = "Your Email Not Yet Confirmed";
                return View("Error");
            }

            string Message;

            if (user.TwoFactorEnabled)
            {
                Message = "Disable 2FA";
            }
            else
            {
                Message = "Enable 2FA";
            }

            //Generate the Two Factor Authentication Token
            var TwoFactorToken = await userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultPhoneProvider);

            //Sending Email
            await emailSender.SendEmailAsync(user.Email, Message, $"Your Token to {Message} is {TwoFactorToken}", false);

            return View(); // View for the user to enter the token
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ManageTwoFactorAuthentication(string Token)
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            var result = await userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultPhoneProvider, Token);

            if (result)
            {
                // Token is valid
                if (user.TwoFactorEnabled)
                {
                    user.TwoFactorEnabled = false;
                    ViewBag.Message = "You have Sucessfully Disabled Two Factor Authentication";
                }
                else
                {
                    user.TwoFactorEnabled = true;
                    ViewBag.Message = "You have Sucessfully Enabled Two Factor Authentication";
                }

                await userManager.UpdateAsync(user);

                // Redirect to success page 
                return View("TwoFactorAuthenticationSuccessful");
            }
            else
            {
                // Handle invalid token
                ViewBag.ErrorTitle = "Unable to Enable/Disable Two Factor Authentication";
                ViewBag.ErrorMessage = "Either the Token is Expired or you entered some wrong information";
                return View("Error");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult VerifyTwoFactorToken(string Email, string ReturnUrl, bool RememberMe, string TwoFactorAuthenticationToken)
        {
            VerifyTwoFactorTokenViewModel model = new VerifyTwoFactorTokenViewModel()
            {
                RememberMe = RememberMe,
                Email = Email,
                ReturnUrl = ReturnUrl,
                Token = TwoFactorAuthenticationToken
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyTwoFactorToken(VerifyTwoFactorTokenViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt.");
                return View(model);
            }

            // Validate the 2FA token
            var result = await userManager.VerifyTwoFactorTokenAsync(user, "Email", model.TwoFactorCode);

            if (result)
            {
                // Sign in the user and redirect
                await signInManager.SignInAsync(user, isPersistent: model.RememberMe);

                // Check if the ReturnUrl is not null and is a local URL
                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }
                else
                {
                    // Redirect to default page
                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid verification code.");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateProfile(string? ReturnUrl = null)
        {
            // Retrieve the current user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Check if the user ID is null or empty
            if (string.IsNullOrEmpty(userId))
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            // Retrieve the user from the database using the UserManager
            var user = await userManager.FindByIdAsync(userId);

            // Check if the user exists
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            // Map user data to UpdateProfileViewModel
            var model = new UpdateProfileViewModel
            {
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };

            ViewData["ReturnUrl"] = ReturnUrl;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UpdateProfileViewModel model)
        {
            // Check if the model state is not valid
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Retrieve the current user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Retrieve the user from the database using the user ID
            var user = await userManager.FindByIdAsync(userId);

            // Check if the user exists
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            // Populate the user instance with the data from UpdateProfileViewModel
            user.UserName = model.UserName;
            user.Email = model.Email;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;

            // Update user data in the AspNetUsers Identity table
            var result = await userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                // Once user data updated, redirect to a success page or another action
                ViewBag.SuccessMessage = "Your Profile has been updated successfully";
                return View();
            }
            else
            {
                // If there are errors during update, add them to ModelState
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
        }
    }
}