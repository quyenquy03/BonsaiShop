using System;
using System.Collections.Generic;

namespace BonsaiShop.Models;

public partial class CartDetail
{
    public long CartId { get; set; }

    public long ProductId { get; set; }

    public int? Quantity { get; set; }

    public decimal? Price { get; set; }

    public int? Status { get; set; }

    public virtual Cart Cart { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
