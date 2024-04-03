using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Darla.Models2
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
