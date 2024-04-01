using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Darla.Models;

[Table("AspNetUsers")]
public partial class User
{
    public string UserId { get; set; }

    public string NetId { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public int PermissionType { get; set; }

    public virtual JudgeRoom? JudgeRoom { get; set; }

    public virtual Permission PermissionTypeNavigation { get; set; } = null!;

    public virtual StudentTeam? StudentTeam { get; set; }

    public virtual UserPassword? UserPassword { get; set; }
}
