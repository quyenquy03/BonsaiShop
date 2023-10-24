using System;
using System.Collections.Generic;

namespace BonsaiShop.Models;

public partial class Product
{
    public long ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public string Alias { get; set; } = null!;

    public string? Description { get; set; }

    public string? Detail { get; set; }

    public string? Image { get; set; }

    public decimal? Price { get; set; }

    public decimal? PriceSale { get; set; }

    public int? Quantity { get; set; }

    public long? CategoryId { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public bool? IsNew { get; set; }

    public bool? IsBestSeller { get; set; }

    public bool? IsActive { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
