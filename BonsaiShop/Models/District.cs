using System;
using System.Collections.Generic;

namespace BonsaiShop.Models;

public partial class District
{
    public long DistrictId { get; set; }

    public string? DistrictName { get; set; }

    public string? DistricType { get; set; }

    public long? ProvinceId { get; set; }

    public virtual ICollection<Commune> Communes { get; set; } = new List<Commune>();

    public virtual ICollection<FeeShip> FeeShips { get; set; } = new List<FeeShip>();

    public virtual Province? Province { get; set; }
}
