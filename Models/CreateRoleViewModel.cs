using System.ComponentModel.DataAnnotations;

namespace Darla.Models
{
    public class CreateRoleViewModel
    {
        [Required]
        public string RoleName { get; set; }
    }
}