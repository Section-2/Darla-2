using System;
using System.Collections.Generic;

namespace Darla.Models;

public partial class UserPassword
{
    public string UserId { get; set; }

    public string UserPassword1 { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
