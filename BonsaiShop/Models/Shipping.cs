using System;
using System.Collections.Generic;

namespace BonsaiShop.Models;

public partial class Shipping
{
    public long ShippingId { get; set; }

    public long? ProvinceId { get; set; }

    public long? DistrictId { get; set; }

    public decimal? ShipPrice { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Province? Province { get; set; }
}
