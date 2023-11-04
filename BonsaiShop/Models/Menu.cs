using System;
using System.Collections.Generic;

namespace BonsaiShop.Models;

public partial class Menu
{
    public long MenuId { get; set; }

    public string? MenuName { get; set; }

    public string? Alias { get; set; }

    public string? Description { get; set; }

    public int? Position { get; set; }

    public int? Levels { get; set; }

    public long? ParrentId { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? IsShow { get; set; }
}
