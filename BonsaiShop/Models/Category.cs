using System;
using System.Collections.Generic;

namespace BonsaiShop.Models;

public partial class Category
{
    public long CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public string? Alias { get; set; }

    public string? Description { get; set; }

    public string? Detail { get; set; }

    public long? TypeId { get; set; }

    public string? SeoTitle { get; set; }

    public string? SeoKeyword { get; set; }

    public string? SeoDescription { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Blog> Blogs { get; set; } = new List<Blog>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual AllCode? Type { get; set; }
}
