using System;
using System.Collections.Generic;

namespace BonsaiShop.Models;

public partial class Commune
{
    public long CommuneId { get; set; }

    public string? CommuneName { get; set; }

    public string? CommuneType { get; set; }

    public long? DistrictId { get; set; }
}
