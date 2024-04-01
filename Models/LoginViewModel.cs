using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;

namespace Darla.Models
{
    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }

        public string? ReturnUrl;
    }
}