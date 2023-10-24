using System;
using System.Collections.Generic;

namespace BonsaiShop.Models;

public partial class AllCode
{
    public long Id { get; set; }

    public string? Type { get; set; }

    public string? KeyCode { get; set; }

    public string? Value { get; set; }

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

    public virtual ICollection<Menu> Menus { get; set; } = new List<Menu>();
}
