using System;
using System.Collections.Generic;

namespace Darla.Models2;

public partial class Order
{
    public int OrderID { get; set; }

    public string Name { get; set; } = null!;

    public string Line1 { get; set; } = null!;

    public string? Line2 { get; set; }

    public string? Line3 { get; set; }

    public string City { get; set; } = null!;

    public string State { get; set; } = null!;

    public string? Zip { get; set; }

    public string Country { get; set; } = null!;

    public bool GiftWrap { get; set; }

    public virtual ICollection<CartLine> CartLines { get; set; } = new List<CartLine>();
}
