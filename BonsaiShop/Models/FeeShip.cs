using System;
using System.Collections.Generic;

namespace BonsaiShop.Models;

public partial class FeeShip
{
    public long FeeShipId { get; set; }

    public long? CommuneId { get; set; }

    public long? ProvinceId { get; set; }

    public long? DistrictId { get; set; }

    public decimal? ShipPrice { get; set; }

    public virtual Commune? Commune { get; set; }

    public virtual District? District { get; set; }

    public virtual Province? Province { get; set; }
}
