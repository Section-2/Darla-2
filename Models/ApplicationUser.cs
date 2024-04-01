using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Darla.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
