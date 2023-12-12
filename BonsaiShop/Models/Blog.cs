using System;
using System.Collections.Generic;

namespace BonsaiShop.Models;

public partial class Blog
{
    public long BlogId { get; set; }

    public string? BlogName { get; set; }

    public string? BlogSlug { get; set; }

    public long? CategoryId { get; set; }

    public string? BlogDesc { get; set; }

    public string? BlogDetail { get; set; }

    public string? BlogImage { get; set; }

    public int? BlogViewCount { get; set; }

    public string? SeoTitle { get; set; }

    public string? SeoKeyword { get; set; }

    public string? SeoDescription { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public bool? IsDeleted { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<BlogComment> BlogComments { get; set; } = new List<BlogComment>();

    public virtual Category? Category { get; set; }
}
