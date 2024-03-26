using System;
using System.Collections.Generic;

namespace Darla.Models;

public partial class Permission
{
    public int PermissionType { get; set; }

    public string PermissionDescription { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
