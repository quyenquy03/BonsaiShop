using System;
using System.Collections.Generic;

namespace BonsaiShop.Models;

public partial class Province
{
    public long ProvinceId { get; set; }

    public string? ProvinceName { get; set; }

    public string? ProvinceType { get; set; }

    public string? ProvinceSlug { get; set; }

    public virtual ICollection<FeeShip> FeeShips { get; set; } = new List<FeeShip>();
}
