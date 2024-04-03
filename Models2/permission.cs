using System;
using System.Collections.Generic;

namespace Darla.Models2;

public partial class permission
{
    public int permission_type { get; set; }

    public string? permission_description { get; set; }

    public virtual ICollection<user> users { get; set; } = new List<user>();
}
