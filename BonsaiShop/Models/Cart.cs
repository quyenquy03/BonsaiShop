using System;
using System.Collections.Generic;

namespace BonsaiShop.Models;

public partial class Cart
{
    public long CartId { get; set; }

    public long? UserId { get; set; }

    public int? Status { get; set; }

    public DateTime? CreateAt { get; set; }

    public virtual User? User { get; set; }
}
