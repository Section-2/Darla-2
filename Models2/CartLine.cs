using System;
using System.Collections.Generic;

namespace Darla.Models2;

public partial class CartLine
{
    public int CartLineId { get; set; }

    public int ProjectID { get; set; }

    public int Quantity { get; set; }

    public int? OrderID { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Project Project { get; set; } = null!;
}
