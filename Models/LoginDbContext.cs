using Darla.Models;
using Microsoft.AspNetCore.Hosting.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Darla.Models
{
    public class LoginDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public LoginDbContext(DbContextOptions<LoginDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Customizing the ASP.NET Identity model and overriding the defaults if needed

            builder.Entity<IdentityUserRole<string>>()
                   .HasOne<IdentityRole>()
                   .WithMany()
                   .HasForeignKey(ur => ur.RoleId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}