using System.ComponentModel.DataAnnotations;

namespace Darla.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}