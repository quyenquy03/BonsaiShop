using System;
using System.Collections.Generic;

namespace BonsaiShop.Models;

public partial class Product
{
    public long ProductId { get; set; }

    public string? ProductName { get; set; }

    public string? ProductSlug { get; set; }

    public string? ProductDesc { get; set; }

    public string? ProductDetail { get; set; }

    public string? ProductImage { get; set; }

    public decimal? ProductPrice { get; set; }

    public int? ProductDisCount { get; set; }

    public int? ProductViewCount { get; set; }

    public long? CategoryId { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public bool? IsDeleted { get; set; }

    public bool? IsBestSeller { get; set; }

    public bool? IsActive { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
