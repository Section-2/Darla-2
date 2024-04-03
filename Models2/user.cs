using System;
using System.Collections.Generic;

namespace Darla.Models2;

public partial class user
{
    public int user_id { get; set; }

    public string? net_id { get; set; }

    public string? first_name { get; set; }

    public string? last_name { get; set; }

    public int permission_type { get; set; }

    public virtual permission permission_typeNavigation { get; set; } = null!;
}
